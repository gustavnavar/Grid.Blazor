using GridShared.Columns;
using GridShared.DataAnnotations;
using GridShared.Sorting;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace GridBlazor.Columns
{
    /// <summary>
    ///     Default grid columns builder. Creates the columns from expression
    /// </summary>
    internal class DefaultColumnBuilder<T> : IColumnBuilder<T>
    {
        private readonly IGridAnnotationsProvider _annotations;
        private readonly CGrid<T> _grid;

        public DefaultColumnBuilder(CGrid<T> grid, IGridAnnotationsProvider annotations)
        {
            _grid = grid;
            _annotations = annotations;
        }

        #region IColumnBuilder<T> Members

        public IGridColumn<T> CreateColumn<TDataType>(Expression<Func<T, TDataType>> constraint, bool hidden)
        {
            return CreateColumn(constraint, null, hidden);
        }

        public IGridColumn<T> CreateColumn<TDataType>(Expression<Func<T, TDataType>> constraint, IComparer<TDataType> comparer,
            bool hidden)
        {
            bool isExpressionOk = constraint == null || constraint.Body as MemberExpression != null;
            if (isExpressionOk)
            {
                var column = new GridColumn<T, TDataType>(constraint, comparer, _grid);
                column.Hidden = hidden;
                return column;
            }
            throw new NotSupportedException(string.Format("Expression '{0}' not supported by grid", constraint));
        }

        /// <summary>
        ///     Creates column from property info using reflection
        /// </summary>
        public IGridColumn<T> CreateColumn(PropertyInfo pi)
        {
            if (!_annotations.IsColumnMapped(pi))
                return null; //grid column not mapped

            IGridColumn<T> column;
            GridColumnAttribute columnOpt = _annotations.GetAnnotationForColumn<T>(pi);
            if (columnOpt != null)
            {
                column = CreateColumn(pi, false);
                ApplyColumnAnnotationSettings(column, columnOpt);
            }
            else
            {
                GridHiddenColumnAttribute columnHiddenOpt = _annotations.GetAnnotationForHiddenColumn<T>(pi);
                if (columnHiddenOpt != null)
                {
                    column = CreateColumn(pi, true);
                    ApplyHiddenColumnAnnotationSettings(column, columnHiddenOpt);
                }
                else
                {
                    column = CreateColumn(pi, false);
                    ApplyColumnAnnotationSettings(column, new GridColumnAttribute());
                }
            }
            return column;
        }

        public bool DefaultSortEnabled { get; set; }
        public bool DefaultFilteringEnabled { get; set; }

        #endregion

        private IGridColumn<T> CreateColumn(PropertyInfo pi, bool hidden)
        {
            Type entityType = typeof (T);
            Type columnType = typeof(GridColumn<,>).MakeGenericType(entityType, pi.PropertyType);

            //Build expression

            ParameterExpression parameter = Expression.Parameter(entityType, "e");
            MemberExpression expressionProperty = Expression.Property(parameter, pi);

            Type funcType = typeof (Func<,>).MakeGenericType(entityType, pi.PropertyType);
            LambdaExpression lambda = Expression.Lambda(funcType, expressionProperty, parameter);

            var column = Activator.CreateInstance(columnType, lambda, _grid) as IGridColumn<T>;
            if (column != null)
            {
                column.Hidden = hidden;
                column.Sortable(DefaultSortEnabled);
                column.Filterable(DefaultFilteringEnabled);
            }
            return column;
        }

        private void ApplyColumnAnnotationSettings(IGridColumn<T> column, GridColumnAttribute options)
        {
            column.Encoded(options.EncodeEnabled)
                  .Sanitized(options.SanitizeEnabled)
                  .Filterable(options.FilterEnabled)
                  .Sortable(options.SortEnabled);
            column.SetPrimaryKey(options.Key);
            GridSortDirection? initialDirection = options.GetInitialSortDirection();
            if (initialDirection.HasValue)
                column.SortInitialDirection(initialDirection.Value);

            AutoCompleteTerm? autoCompleteTaxonomy = options.GetAutocompleteTaxonomy();
            if (autoCompleteTaxonomy.HasValue)
            {
                column.SetAutoCompleteTaxonomy(autoCompleteTaxonomy.Value);
            }

            if (!string.IsNullOrEmpty(options.FilterWidgetType))
                column.SetFilterWidgetType(options.FilterWidgetType);

            if (!string.IsNullOrEmpty(options.Format))
                column.Format(options.Format);
            if (!string.IsNullOrEmpty(options.Title))
                column.Titled(options.Title);
            if (!string.IsNullOrEmpty(options.Width))
                column.Width = options.Width;
        }

        private void ApplyHiddenColumnAnnotationSettings(IGridColumn<T> column, GridHiddenColumnAttribute options)
        {
            column.Encoded(options.EncodeEnabled).Sanitized(options.SanitizeEnabled);
            if (!string.IsNullOrEmpty(options.Format))
                column.Format(options.Format);
        }
    }
}