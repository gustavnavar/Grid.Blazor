using GridShared;
using GridShared.Columns;
using GridShared.Filtering;
using GridShared.Grouping;
using GridShared.Searching;
using GridShared.Sorting;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

namespace GridBlazor.Columns
{
    public abstract class GridColumnBase<T> : GridStyledColumn, IGridColumn<T>, ICGridColumn
    {
        public Type ComponentType { get; private set; }
        public IList<Action<object>> Actions { get; private set; }
        public object Object { get; private set; }

        protected Func<T, string> ValueConstraint;
        protected string ValuePattern;

        #region IGridColumn<T> Members

        public bool EncodeEnabled { get; protected set; }
        public bool SanitizeEnabled { get; set; }

        public string Width { get; set; }

        public bool SortEnabled { get; protected set; }

        public string Title { get; set; }

        public string Name { get; set; }

        public string FieldName { get; protected set; }

        public bool IsSorted { get; set; }
        public GridSortDirection? Direction { get; set; }

        public bool Hidden { get; protected set; }

        public bool CrudHidden { get; protected set; } = false;

        public bool IsPrimaryKey { get; protected set; } = false;

        public bool IsSumEnabled { get; internal set; } = false;

        public bool IsAverageEnabled { get; internal set; } = false;

        public bool IsMaxEnabled { get; internal set; } = false;

        public bool IsMinEnabled { get; internal set; } = false;

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
            AddCssClass(cssClasses);
            return this;
        }

        public IGridColumn<T> RenderValueAs(Func<T, string> constraint)
        {
            ValueConstraint = constraint;
            return this;
        }

        public IGridColumn<T> RenderComponentAs(Type componentType)
        {
            return RenderComponentAs(componentType, null, null);
        }

        public IGridColumn<T> RenderComponentAs(Type componentType, IList<Action<object>> actions)
        {
            return RenderComponentAs(componentType, actions, null);
        }

        public IGridColumn<T> RenderComponentAs(Type componentType, object obj)
        {
            return RenderComponentAs(componentType, null, obj);
        }

        public IGridColumn<T> RenderComponentAs(Type componentType, IList<Action<object>> actions, object obj)
        {
            if(componentType.GetInterfaces().Any(r => r.Equals(typeof(ICustomGridComponent<T>)))
                && componentType.IsSubclassOf(typeof(ComponentBase)))
            {
                ComponentType = componentType;
                Actions = actions;
                Object = obj;
            }      
            return this;
        }

        public IGridColumn<T> RenderComponentAs<TComponent>()
        {
            return RenderComponentAs<TComponent>(null, null);
        }

        public IGridColumn<T> RenderComponentAs<TComponent>(IList<Action<object>> actions)
        {
            return RenderComponentAs<TComponent>(actions, null);
        }

        public IGridColumn<T> RenderComponentAs<TComponent>(object obj)
        {
            return RenderComponentAs<TComponent>(null, obj);
        }

        public IGridColumn<T> RenderComponentAs<TComponent>(IList<Action<object>> actions, object obj)
        {
            return RenderComponentAs(typeof(TComponent), actions, obj);
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

        public IGridColumn<T> SetCrudHidden(bool enabled)
        {
            CrudHidden = enabled;
            return this;
        }

        public IGridColumn<T> SetPrimaryKey(bool enabled)
        {
            IsPrimaryKey = enabled;
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

        public abstract IGridCell GetCell(object instance);

        public string GetFormatedValue(object value)
        {
            string textValue;
            if (!string.IsNullOrEmpty(ValuePattern))
                textValue = string.Format(ValuePattern, value);
            else
                textValue = value.ToString();
            return textValue;
        }

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

        public abstract IColumnGroup<T> Group { get; }

        public abstract IGridCell GetValue(T instance);

        #endregion
    }
}