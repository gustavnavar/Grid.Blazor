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
    ///     Колонка, которая выводит содержимое свойства модели
    /// </summary>
    public class HiddenGridColumn<T, TDataType> : GridColumnBase<T>
    {
        private readonly Func<T, TDataType> _constraint;
        private readonly ISGrid _grid;

        public HiddenGridColumn(Expression<Func<T, TDataType>> expression, ISGrid grid)
        {
            _grid = grid;
            SortEnabled = false;

            Hidden = true;

            if (expression != null)
            {
                var expr = expression.Body as MemberExpression;
                if (expr == null)
                    throw new ArgumentException(
                        string.Format("Expression '{0}' must be a member expression", expression),
                        "expression");

                _constraint = expression.Compile();

                FieldName = PropertiesHelper.BuildColumnNameFromMemberExpression(expr);
                Name = FieldName;
            }
        }

        public override IEnumerable<IColumnOrderer<T>> Orderers
        {
            get { throw new InvalidOperationException("You cannot sort hidden field"); }
        }

        public override bool FilterEnabled
        {
            get { return false; }
            set { }
        }

        public override IColumnFilter<T> Filter
        {
            get { return null; }
        }

        public override string FilterWidgetTypeName
        {
            get { return PropertiesHelper.GetUnderlyingType(typeof(TDataType)).FullName; }
        }

        public override IColumnSearch<T> Search
        {
            get { return null; }
        }

        public override IColumnTotals<T> Totals
        {
            get { return null; }
        }

        public override IColumnGroup<T> Group
        {
            get { return null; }
        }

        //public override bool IsSorted { get; set; }
        //public override GridSortDirection? Direction { get; set; }

        public override IGrid ParentGrid
        {
            get { return _grid; }
        }

        public override IGridColumn<T> SetFilterWidgetType(string typeName, object widgetData)
        {
            return this; //Do nothing
        }

        public override IGridColumn<T> SetFilterWidgetType(string typeName)
        {
            return this; //Do nothing
        }

        public override IGridColumn<T> SortInitialDirection(GridSortDirection direction)
        {
            return this; //Do nothing
        }

        public override IGridColumn<T> ThenSortBy<TKey>(Expression<Func<T, TKey>> expression)
        {
            return this; //Do nothing
        }

        public override IGridColumn<T> ThenSortByDescending<TKey>(Expression<Func<T, TKey>> expression)
        {
            return this; //Do nothing
        }

        public override IGridColumn<T> Sortable(bool sort)
        {
            return this; //Do nothing
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

        public override IGridColumn<T> Filterable(bool showColumnValuesVariants)
        {
            return this;
        }

        public override IGridCell GetCell(object instance)
        {
            return GetValue((T)instance);
        }

        public override IGridColumn<T> SetCellCssClassesContraint(Func<T, string> contraint)
        {
            return this;
        }

        public override string GetCellCssClasses(object item)
        {
            return string.Empty;
        }
    }
}