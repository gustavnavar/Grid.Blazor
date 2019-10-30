using GridShared;
using GridShared.Columns;
using GridShared.Filtering;
using GridShared.Grouping;
using GridShared.Searching;
using GridShared.Sorting;
using GridShared.Totals;
using GridShared.Utility;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace GridMvc.Columns
{
    /// <summary>
    ///     Default implementation of Grid column
    /// </summary>
    public class GridColumn<T, TDataType> : GridColumnBase<T>
    {
        /// <summary>
        ///     Expression to member, used for this column
        /// </summary>
        private readonly Func<T, TDataType> _constraint;

        /// <summary>
        ///     Filters and orderers collection for this columns
        /// </summary>
        private readonly IColumnFilter<T> _filter;

        /// <summary>
        ///     Searchers collection for this columns
        /// </summary>
        private readonly IColumnSearch<T> _search;

        /// <summary>
        ///     Totals collection for this columns
        /// </summary>
        private readonly IColumnTotals<T> _totals;

        /// <summary>
        ///     Groupers collection for this columns
        /// </summary>
        private readonly IColumnGroup<T> _group;

        /// <summary>
        ///     Expression to class, used for this column
        /// </summary>
        private Func<T, string> _cellCssClassesContraint;

        /// <summary>
        ///     Parent grid of this column
        /// </summary>
        private readonly ISGrid _grid;

        private readonly List<IColumnOrderer<T>> _orderers = new List<IColumnOrderer<T>>();

        private string _filterWidgetTypeName;


        public GridColumn(Expression<Func<T, TDataType>> expression, ISGrid grid)
        {
            #region Setup defaults

            EncodeEnabled = true;
            SortEnabled = false;
            SanitizeEnabled = true;

            Hidden = false;

            _filterWidgetTypeName = PropertiesHelper.GetUnderlyingType(typeof(TDataType)).FullName;
            _grid = grid;

            #endregion

            if (expression != null)
            {
                var expr = expression.Body as MemberExpression;
                if (expr == null)
                    throw new ArgumentException(
                        string.Format("Expression '{0}' must be a member expression", expression),
                        "expression");

                _constraint = expression.Compile();
                _orderers.Insert(0, new OrderByGridOrderer<T, TDataType>(expression));
                _filter = new DefaultColumnFilter<T, TDataType>(expression);
                _search = new DefaultColumnSearch<T, TDataType>(expression);
                _totals = new DefaultColumnTotals<T, TDataType>(expression);
                _group = new DefaultColumnGroup<T, TDataType>(expression);
                //Generate unique column name:
                Name = PropertiesHelper.BuildColumnNameFromMemberExpression(expr);
                Title = Name; //Using the same name by default
            }
        }

        public override IEnumerable<IColumnOrderer<T>> Orderers
        {
            get { return _orderers; }
        }

        public override bool FilterEnabled { get; set; }


        public override IColumnFilter<T> Filter
        {
            get { return _filter; }
        }

        public override string FilterWidgetTypeName
        {
            get { return _filterWidgetTypeName; }
        }

        public override IColumnSearch<T> Search
        {
            get { return _search; }
        }

        public override IColumnTotals<T> Totals
        {
            get { return _totals; }
        }

        public override IColumnGroup<T> Group
        {
            get { return _group; }
        }

        public override IGrid ParentGrid
        {
            get { return _grid; }
        }

        public override IGridColumn<T> SetFilterWidgetType(string typeName, object widgetData)
        {
            SetFilterWidgetType(typeName);
            if (widgetData != null)
                FilterWidgetData = widgetData;
            return this;
        }

        public override IGridColumn<T> SetFilterWidgetType(string typeName)
        {
            if (!string.IsNullOrEmpty(typeName))
                _filterWidgetTypeName = typeName;
            return this;
        }

        public override IGridColumn<T> SortInitialDirection(GridSortDirection direction)
        {
            if (string.IsNullOrEmpty(_grid.Settings.SortSettings.ColumnName))
            {
                IsSorted = true;
                Direction = direction;

                // added to enable initial sorting
                _grid.Settings.SortSettings.ColumnName = Name;
                _grid.Settings.SortSettings.Direction = direction;
            }
            return this;
        }

        public override IGridColumn<T> ThenSortBy<TKey>(Expression<Func<T, TKey>> expression)
        {
            _orderers.Add(new ThenByColumnOrderer<T, TKey>(expression, GridSortDirection.Ascending));
            return this;
        }

        public override IGridColumn<T> ThenSortByDescending<TKey>(Expression<Func<T, TKey>> expression)
        {
            _orderers.Add(new ThenByColumnOrderer<T, TKey>(expression, GridSortDirection.Descending));
            return this;
        }

        public override IGridColumn<T> Sortable(bool sort)
        {
            if (sort && _constraint == null)
            {
                return this; //cannot enable sorting for column without expression
            }
            SortEnabled = sort;
            return this;
        }

        public override IGridCell GetCell(object instance)
        {
            return GetValue((T)instance);
        }

        public override IGridCell GetValue(T instance)
        {
            string textValue;
            if (ValueConstraint != null)
            {
                textValue = ValueConstraint(instance);
            }
            else
            {
                if (_constraint == null)
                {
                    throw new InvalidOperationException("You need to specify render expression using RenderValueAs");
                }


                TDataType value = default(TDataType);

                var nullReferece = false;
                try
                {
                    value = _constraint(instance);
                }
                catch (NullReferenceException)
                {
                    nullReferece = true;
                    // specified expression throws NullReferenceException
                    // example: x=>x.Child.Property, when Child is NULL
                }

                if (nullReferece || value == null)
                    textValue = string.Empty;
                else
                    textValue = GetFormatedValue(value);
            }
            if (!EncodeEnabled && SanitizeEnabled)
            {
                textValue = _grid.Sanitizer.Sanitize(textValue);
            }
            return new GridCell(textValue) { Encode = EncodeEnabled };
        }

        public override IGridColumn<T> Filterable(bool enable)
        {
            if (enable && _constraint == null)
            {
                return this; //cannot enable filtering for column without expression
            }
            FilterEnabled = enable;
            return this;
        }

        public override IGridColumn<T> SetCellCssClassesContraint(Func<T, string> contraint)
        {
            _cellCssClassesContraint = contraint;
            return this;
        }

        public override string GetCellCssClasses(object item)
        {
            if (_cellCssClassesContraint == null)
                return string.Empty;
            var typed = (T)item;
            if (typed == null)
                throw new InvalidCastException(string.Format("The item must be of type '{0}'", typeof(T).FullName));
            return _cellCssClassesContraint(typed);
        }
    }
}