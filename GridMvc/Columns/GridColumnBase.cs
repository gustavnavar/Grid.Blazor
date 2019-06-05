using GridShared;
using GridShared.Columns;
using GridShared.Filtering;
using GridShared.Searching;
using GridShared.Sorting;
using GridShared.Totals;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;

namespace GridMvc.Columns
{
    public abstract class GridColumnBase<T> : IGridColumn<T>, ISGridColumn, ITotalsColumn<T>
    {
        protected Func<T, string> ValueConstraint;
        public string ValuePattern { get; protected set; }

        #region IGridColumn<T> Members

        public bool EncodeEnabled { get; protected set; }
        public bool SanitizeEnabled { get; set; }

        public string Width { get; set; }

        public bool SortEnabled { get; protected set; }

        public string Title { get; set; }

        public string Name { get; set; }

        public bool IsSorted { get; set; }
        public GridSortDirection? Direction { get; set; }

        public bool IsSumEnabled { get; internal set; } = false;

        public bool IsAverageEnabled { get; internal set; } = false;

        public bool IsMaxEnabled { get; internal set; } = false;

        public bool IsMinEnabled { get; internal set; } = false;

        public decimal SumValue { get; set; }

        public decimal AverageValue { get; set; }

        public object MaxValue { get; set; }

        public object MinValue { get; set; }

        public string SumString { get; set; }

        public string AverageString { get; set; }

        public string MaxString { get; set; }

        public string MinString { get; set; }

        public IGridColumn<T> Titled(string title)
        {
            Title = title;
            return this;
        }

        public IGridColumn<T> Encoded(bool encode)
        {
            EncodeEnabled = encode;
            return this;
        }

        IGridColumn<T> IColumn<T>.SetWidth(string width)
        {
            Width = width;
            return this;
        }

        IGridColumn<T> IColumn<T>.SetWidth(int width)
        {
            Width = width.ToString(CultureInfo.InvariantCulture) + "px";
            return this;
        }

        public IGridColumn<T> Css(string cssClasses)
        {
            if (string.IsNullOrEmpty(cssClasses))
                return this;
            var headerStyledRender = HeaderRenderer as GridStyledRenderer;
            if (headerStyledRender != null)
                headerStyledRender.AddCssClass(cssClasses);

            var cellStyledRender = CellRenderer as GridStyledRenderer;
            if (cellStyledRender != null)
                cellStyledRender.AddCssClass(cssClasses);
            return this;
        }       

        public IGridColumn<T> RenderValueAs(Func<T, string> constraint)
        {
            ValueConstraint = constraint;
            return this;
        }

        public IGridColumn<T> Format(string pattern)
        {
            ValuePattern = pattern;
            return this;
        }

        public IGridColumn<T> Sum(bool enabled)
        {
            IsSumEnabled = enabled;
            return this;
        }

        public IGridColumn<T> Average(bool enabled)
        {
            IsAverageEnabled = enabled;
            return this;
        }

        public IGridColumn<T> Max(bool enabled)
        {
            IsMaxEnabled = enabled;
            return this;
        }

        public IGridColumn<T> Min(bool enabled)
        {
            IsMinEnabled = enabled;
            return this;
        }

        public abstract IGrid ParentGrid { get; }

        public virtual IGridColumn<T> Sanitized(bool sanitize)
        {
            SanitizeEnabled = sanitize;
            return this;
        }

        public IGridColumn<T> SetInitialFilter(GridFilterType type, string value)
        {
            var filter = new ColumnFilterValue
                {
                    FilterType = type,
                    FilterValue = value,
                    ColumnName = Name
                };
            InitialFilterSettings = filter;
            return this;
        }

        public abstract IGridColumn<T> SortInitialDirection(GridSortDirection direction);

        public abstract IGridColumn<T> ThenSortBy<TKey>(Expression<Func<T, TKey>> expression);
        public abstract IGridColumn<T> ThenSortByDescending<TKey>(Expression<Func<T, TKey>> expression);

        public abstract IEnumerable<IColumnOrderer<T>> Orderers { get; }
        public abstract IGridColumn<T> Sortable(bool sort);

        public abstract IGridColumnHeaderRenderer HeaderRenderer { get; set; }
        public abstract IGridCellRenderer CellRenderer { get; set; }
        public abstract IGridCell GetCell(object instance);

        public abstract bool FilterEnabled { get; set; }

        public ColumnFilterValue InitialFilterSettings { get; set; }

        public abstract IGridColumn<T> Filterable(bool showColumnValuesVariants);


        public abstract IGridColumn<T> SetFilterWidgetType(string typeName);
        public abstract IGridColumn<T> SetFilterWidgetType(string typeName, object widgetData);

        public abstract IGridColumn<T> SetCellCssClassesContraint(Func<T, string> contraint);
        public abstract string GetCellCssClasses(object item);

        public abstract IColumnFilter<T> Filter { get; }
        public abstract string FilterWidgetTypeName { get; }
        public object FilterWidgetData { get; protected set; }

        public abstract IColumnSearch<T> Search { get; }

        public abstract IColumnTotals<T> Totals { get; }

        #endregion

        public abstract IGridCell GetValue(T instance);
    }
}