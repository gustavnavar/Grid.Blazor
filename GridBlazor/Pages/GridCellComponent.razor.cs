using GridShared;
using GridShared.Columns;
using GridShared.Utility;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Net.Http.Json;
using System.Reflection;
using System.Threading.Tasks;

namespace GridBlazor.Pages
{
    public partial class GridCellComponent<T>
    {
        private const string TdStyle = "display:none;";

        protected string _cssStyles;
        protected string _cssClass;
        protected MarkupString _cell;
        protected Type _componentType;
        protected RenderFragment _cellRender;

        public object ChildComponent { get; private set; } = null;

        public IEnumerable<SelectItem> SelectItems { get; private set; }

        [CascadingParameter(Name = "GridComponent")]
        protected internal GridComponent<T> GridComponent { get; set; }

        [Parameter]
        public IGridColumn Column { get; set; }

        [Parameter]
        public object Item { get; set; }

        [Parameter]
        public int RowId { get; set; }

        [Parameter]
        public string TdClass { get; set; } = "grid-cell";

        protected override async Task OnParametersSetAsync()
        {
            _componentType = Column.ComponentType;
            _cell = default;
            _cellRender = default;
            if (_componentType != null)
                _cellRender = CreateComponent(GridComponent, _componentType, Column, Item,
                    RowId, false, new VariableReference(ChildComponent));
            else
                _cell = (MarkupString)Column.GetCell(Item).ToString();
            if (Column.Hidden)
                _cssStyles = ((GridStyledColumn)Column).GetCssStylesString() + " " + TdStyle;
            else
                _cssStyles = ((GridStyledColumn)Column).GetCssStylesString();
            _cssClass = ((GridStyledColumn)Column).GetCssClassesString() + " " + TdClass;

            if (GridComponent.Grid.Direction == GridDirection.RTL)
                _cssStyles = string.Concat(_cssStyles, " text-align:right;direction:rtl;").Trim();

            if (!string.IsNullOrWhiteSpace(Column.Width))
                _cssStyles = string.Concat(_cssStyles, " width:", Column.Width, ";").Trim();

            string columnCssClasses = Column.GetCellCssClasses(Item);
            if (!string.IsNullOrWhiteSpace(columnCssClasses))
                _cssClass += " " + columnCssClasses;

            var isSelectField = ((IGridColumn<T>)Column).IsSelectField;
            var isSelectColumn = ((IGridColumn<T>)Column).IsSelectColumn;
            if (isSelectField.IsSelectKey)
            {
                try
                {
                    if (isSelectField.SelectItemExpr != null)
                    {
                        ((IGridColumn<T>)Column).SelectItems = isSelectField.SelectItemExpr.Invoke();
                    }
                    else if (isSelectField.SelectItemExprAsync != null)
                    {
                        ((IGridColumn<T>)Column).SelectItems = await isSelectField.SelectItemExprAsync.Invoke();
                    }
                    else
                    {
                        ((IGridColumn<T>)Column).SelectItems = await GridComponent.Grid.HttpClient.GetFromJsonAsync<IEnumerable<SelectItem>>(isSelectField.Url);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    ((IGridColumn<T>)Column).SelectItems = new List<SelectItem>();
                }
            }
            else if (isSelectColumn.IsSelectKey)
            {
                try
                {
                    if (isSelectColumn.SelectItemExpr != null)
                    {
                        ((IGridColumn<T>)Column).SelectItemExpr = async c => await Task.FromResult(isSelectColumn.SelectItemExpr(c));
                    }
                    else if (isSelectColumn.SelectItemExprAsync != null)
                    {
                        ((IGridColumn<T>)Column).SelectItemExpr = isSelectColumn.SelectItemExprAsync;
                    }
                    else
                    {
                        ((IGridColumn<T>)Column).SelectItemExpr = async c => await GridComponent.Grid.HttpClient.GetFromJsonAsync<IEnumerable<SelectItem>>(isSelectColumn.Url(c));
                    }

                    SelectItems = await ((IGridColumn<T>)Column).SelectItemExpr((T)Item);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    ((IGridColumn<T>)Column).SelectItemExpr = async c => await Task.FromResult(new List<SelectItem>());
                }
            }
        }

        private void SetValue(object value, IGridColumn column)
        {
            var names = column.FieldName.Split('.');
            PropertyInfo pi;
            object obj = Item;
            for (int i = 0; i < names.Length - 1; i++)
            {
                pi = obj.GetType().GetProperty(names[i]);
                obj = pi.GetValue(obj, null);
            }
            pi = obj.GetType().GetProperty(names[names.Length - 1]);
            pi.SetValue(obj, value, null);
        }

        private async Task ChangeValue(ChangeEventArgs e, IGridColumn column, string typeAttr = null)
        {
            if (string.IsNullOrWhiteSpace(e.Value.ToString()))
            {
                SetValue(null, column);
            }
            else
            {
                if (typeAttr == "week")
                {
                    var value = DateTimeUtils.FromIso8601WeekDate(e.Value.ToString());
                    SetValue(value, column);
                }
                else if (typeAttr == "month")
                {
                    var value = DateTimeUtils.FromMonthDate(e.Value.ToString());
                    SetValue(value, column);
                }
                else
                {
                    var (type, _) = ((IGridColumn<T>)column).GetTypeAndValue((T)Item);
                    var typeConverter = TypeDescriptor.GetConverter(type);
                    if (typeConverter != null)
                    {
                        try
                        {
                            object value = null;
                            // if is number type
                            if (type == typeof(decimal) || type == typeof(float) || type == typeof(double) || type == typeof(byte) || type == typeof(short) || type == typeof(int) || type == typeof(long) ||
                                type == typeof(decimal?) || type == typeof(float?) || type == typeof(double?) || type == typeof(byte?) || type == typeof(short?) || type == typeof(int?) || type == typeof(long?))
                            {
                                string thousandSeparator = ",";
                                if (thousandSeparator == CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)
                                {
                                    thousandSeparator = "."; // separators are inverted compared to EN, like is in DE and some other european languages
                                }
                                string valueText = e.Value.ToString();
                                if (valueText.Contains(thousandSeparator))
                                {
                                    valueText = valueText.Replace(thousandSeparator, ""); // removes thousands separator if exist so that parsing can be correctly done
                                }
                                value = typeConverter.ConvertFrom(valueText);
                            }
                            else
                            {
                                value = typeConverter.ConvertFrom(e.Value.ToString());
                            }
                            SetValue(value, column);
                        }
                        catch (Exception)
                        {
                            SetValue(null, column);
                        }
                    }
                }
            }

            if (((IGridColumn<T>)column).AfterChangeValue != null)
                await ((IGridColumn<T>)column).AfterChangeValue((T)Item, GridMode.Update);
        }

        public static RenderFragment CreateComponent(GridComponent<T> gridComponent, Type componentType,
            IGridColumn column, object item, int? rowId, bool crud, VariableReference reference) => builder =>
        {
            builder.OpenComponent<CascadingValue<GridComponent<T>>>(0);
            builder.AddAttribute(1, "Value", gridComponent);
            builder.AddAttribute(2, "Name", "GridComponent");
            builder.AddAttribute(3, "ChildContent", CreateComponent(componentType, column, item, rowId, crud, reference));
            builder.CloseComponent();
        };

        private static RenderFragment CreateComponent(Type componentType, IGridColumn column,
            object item, int? RowId, bool crud, VariableReference reference)  => builder =>
        {
            if (componentType != null)
            {
                builder.OpenComponent(0, componentType);
                builder.AddAttribute(1, "Item", item);
                var gridProperty = componentType.GetProperty("Grid");
                if (gridProperty != null && gridProperty.PropertyType == typeof(CGrid<T>))
                    builder.AddAttribute(2, "Grid", (CGrid<T>)column.ParentGrid);
                gridProperty = componentType.GetProperty("Actions");
                if (gridProperty != null)
                    builder.AddAttribute(3, "Actions", crud ? column.CrudActions : column.Actions);
                gridProperty = componentType.GetProperty("Functions");
                if (gridProperty != null)
                    builder.AddAttribute(4, "Functions", crud ? column.CrudFunctions : column.Functions);
                gridProperty = componentType.GetProperty("Object");
                if (gridProperty != null)
                    builder.AddAttribute(5, "Object", crud ? column.CrudObject : column.Object);
                gridProperty = componentType.GetProperty("RowId");
                if (gridProperty != null && RowId.HasValue)
                    builder.AddAttribute(6, "RowId", RowId.Value);
                builder.AddComponentReferenceCapture(7, r => reference.Variable = r);
                builder.CloseComponent();
            }
        };
    }
}
