using GridBlazor.Columns;
using GridBlazor.Resources;
using GridShared.Columns;
using GridShared.Utility;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace GridBlazor.Pages
{
    public partial class GridUpdateComponent<T> : ICustomGridComponent<T>
    {
        private int _sequence = 0;
        private bool _shouldRender = false;
        private QueryDictionary<RenderFragment> _renderFragments;
        private IEnumerable<string> _tabGroups;

        public string Error { get; set; } = "";

        [CascadingParameter(Name = "GridComponent")]
        protected GridComponent<T> GridComponent { get; set; }

        [Parameter]
        public T Item { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            _renderFragments = new QueryDictionary<RenderFragment>();
            foreach (var column in GridComponent.Grid.Columns)
            {
                // Name must have a non empty value
                if (string.IsNullOrWhiteSpace(column.Name))
                    column.Name = Guid.NewGuid().ToString();

                if (((ICGridColumn)column).SubGrids != null)
                {
                    var values = ((ICGridColumn)column).GetSubGridKeyValues(Item);
                    var grid = await ((ICGridColumn)column).SubGrids(values.Values.ToArray(), true, true, true, true) as ICGrid;
                    grid.FixedValues = values;
                    _renderFragments.Add(column.Name, CreateSubGridComponent(grid));
                }
                else if (column.UpdateComponentType != null)
                {
                    _renderFragments.Add(column.Name, GridCellComponent<T>.CreateComponent(_sequence,
                        column.UpdateComponentType, column, Item, null));
                }
            }
            _tabGroups = GridComponent.Grid.Columns
                .Where(r => !string.IsNullOrWhiteSpace(r.TabGroup) && _renderFragments.Keys.Any(s => s.Equals(r.Name)))
                .Select(r => r.TabGroup).Distinct();

            _shouldRender = true;
        }

        private RenderFragment CreateSubGridComponent(ICGrid grid) => builder =>
        {
            Type gridComponentType = typeof(GridComponent<>).MakeGenericType(grid.Type);
            builder.OpenComponent(++_sequence, gridComponentType);
            builder.AddAttribute(++_sequence, "Grid", grid);
            builder.CloseComponent();
        };

        protected override bool ShouldRender()
        {
            return _shouldRender;
        }

        protected override void OnAfterRender(bool firstRender)
        {
            _shouldRender = false;
        }
        
        private void ChangeValue(object value, IGridColumn column)
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

        protected void ChangeString(ChangeEventArgs e, IGridColumn column)
        {
            ChangeValue(e.Value.ToString(), column);
        }

        protected void ChangeDateTime(ChangeEventArgs e, IGridColumn column)
        {
            if (string.IsNullOrWhiteSpace(e.Value.ToString()))
            {
                ChangeValue(null, column);
            }
            else
            {
                DateTime value;
                if (DateTime.TryParse(e.Value.ToString(), out value))
                {
                    ChangeValue(value, column);
                }
                else
                {
                    ChangeValue(null, column);
                }
            }
        }

        protected void ChangeDateTimeOffset(ChangeEventArgs e, IGridColumn column)
        {
            if (string.IsNullOrWhiteSpace(e.Value.ToString()))
            {
                ChangeValue(null, column);
            }
            else
            {
                DateTimeOffset value;
                if (DateTimeOffset.TryParse(e.Value.ToString(), out value))
                {
                    ChangeValue(value, column);
                }
                else
                {
                    ChangeValue(null, column);
                }
            }
        }

        protected void ChangeTimeSpan(ChangeEventArgs e, IGridColumn column)
        {
            if (string.IsNullOrWhiteSpace(e.Value.ToString()))
            {
                ChangeValue(null, column);
            }
            else
            {
                TimeSpan value;
                if (TimeSpan.TryParse(e.Value.ToString(), out value))
                {
                    ChangeValue(value, column);
                }
                else
                {
                    ChangeValue(null, column);
                }
            }
        }

        protected void ChangeInt32(ChangeEventArgs e, IGridColumn column)
        {
            if (string.IsNullOrWhiteSpace(e.Value.ToString()))
            {
                ChangeValue(null, column);
            }
            else
            {
                Int32 value;
                if (Int32.TryParse(e.Value.ToString(), out value))
                {
                    ChangeValue(value, column);
                }
                else
                {
                    ChangeValue(null, column);
                }
            }
        }

        protected void ChangeDouble(ChangeEventArgs e, IGridColumn column)
        {
            if (string.IsNullOrWhiteSpace(e.Value.ToString()))
            {
                ChangeValue(null, column);
            }
            else
            {
                Double value;
                if (Double.TryParse(e.Value.ToString(), out value))
                {
                    ChangeValue(value, column);
                }
                else
                {
                    ChangeValue(null, column);
                }
            }
        }

        protected void ChangeDecimal(ChangeEventArgs e, IGridColumn column)
        {
            if (string.IsNullOrWhiteSpace(e.Value.ToString()))
            {
                ChangeValue(null, column);
            }
            else
            {
                Decimal value;
                if (Decimal.TryParse(e.Value.ToString(), out value))
                {
                    ChangeValue(value, column);
                }
                else
                {
                    ChangeValue(null, column);
                }
            }
        }

        protected void ChangeByte(ChangeEventArgs e, IGridColumn column)
        {
            if (string.IsNullOrWhiteSpace(e.Value.ToString()))
            {
                ChangeValue(null, column);
            }
            else
            {
                Byte value;
                if (Byte.TryParse(e.Value.ToString(), out value))
                {
                    ChangeValue(value, column);
                }
                else
                {
                    ChangeValue(null, column);
                }
            }
        }

        protected void ChangeSingle(ChangeEventArgs e, IGridColumn column)
        {
            if (string.IsNullOrWhiteSpace(e.Value.ToString()))
            {
                ChangeValue(null, column);
            }
            else
            {
                Single value;
                if (Single.TryParse(e.Value.ToString(), out value))
                {
                    ChangeValue(value, column);
                }
                else
                {
                    ChangeValue(null, column);
                }
            }
        }

        protected void ChangeInt64(ChangeEventArgs e, IGridColumn column)
        {
            if (string.IsNullOrWhiteSpace(e.Value.ToString()))
            {
                ChangeValue(null, column);
            }
            else
            {
                Int64 value;
                if (Int64.TryParse(e.Value.ToString(), out value))
                {
                    ChangeValue(value, column);
                }
                else
                {
                    ChangeValue(null, column);
                }
            }
        }

        protected void ChangeInt16(ChangeEventArgs e, IGridColumn column)
        {
            if (string.IsNullOrWhiteSpace(e.Value.ToString()))
            {
                ChangeValue(null, column);
            }
            else
            {
                Int16 value;
                if (Int16.TryParse(e.Value.ToString(), out value))
                {
                    ChangeValue(value, column);
                }
                else
                {
                    ChangeValue(null, column);
                }
            }
        }

        protected void ChangeUInt64(ChangeEventArgs e, IGridColumn column)
        {
            if (string.IsNullOrWhiteSpace(e.Value.ToString()))
            {
                ChangeValue(null, column);
            }
            else
            {
                UInt64 value;
                if (UInt64.TryParse(e.Value.ToString(), out value))
                {
                    ChangeValue(value, column);
                }
                else
                {
                    ChangeValue(null, column);
                }
            }
        }

        protected void ChangeUInt32(ChangeEventArgs e, IGridColumn column)
        {
            if (string.IsNullOrWhiteSpace(e.Value.ToString()))
            {
                ChangeValue(null, column);
            }
            else
            {
                UInt32 value;
                if (UInt32.TryParse(e.Value.ToString(), out value))
                {
                    ChangeValue(value, column);
                }
                else
                {
                    ChangeValue(null, column);
                }
            }
        }

        protected void ChangeUInt16(ChangeEventArgs e, IGridColumn column)
        {
            if (string.IsNullOrWhiteSpace(e.Value.ToString()))
            {
                ChangeValue(null, column);
            }
            else
            {
                UInt16 value;
                if (UInt16.TryParse(e.Value.ToString(), out value))
                {
                    ChangeValue(value, column);
                }
                else
                {
                    ChangeValue(null, column);
                }
            }
        }

        protected void ChangeBool(ChangeEventArgs e, IGridColumn column)
        {
            if (string.IsNullOrWhiteSpace(e.Value.ToString()))
            {
                ChangeValue(null, column);
            }
            else
            {
                bool value;
                if (bool.TryParse(e.Value.ToString(), out value))
                {
                    ChangeValue(value, column);
                }
                else
                {
                    ChangeValue(null, column);
                }
            }
        }

        protected void ChangeGuid(ChangeEventArgs e, IGridColumn column)
        {
            if (string.IsNullOrWhiteSpace(e.Value.ToString()))
            {
                ChangeValue(null, column);
            }
            else
            {
                Guid value;
                if (Guid.TryParse(e.Value.ToString(), out value))
                {
                    ChangeValue(value, column);
                }
                else
                {
                    ChangeValue(null, column);
                }
            }
        }

        protected async Task UpdateItem()
        {
            try
            {
                await GridComponent.UpdateItem(this);
            }
            catch (GridException e)
            {
                _shouldRender = true;
                Error = string.IsNullOrWhiteSpace(e.Code) ? e.Message :  e.Code + " - " + e.Message;
            }
            catch (Exception)
            {
                _shouldRender = true;
                Error = Strings.UpdateError;
            }
        }

        protected void BackButtonClicked()
        {
            GridComponent.BackButton();
        }

    }
}