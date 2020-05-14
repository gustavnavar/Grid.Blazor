using GridBlazor.Columns;
using GridBlazor.Resources;
using GridShared.Columns;
using GridShared.Utility;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
                        column.UpdateComponentType, column, Item, null, true));
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

        private void ChangeValue(ChangeEventArgs e, IGridColumn column, string typeAttr = null)
        {
            if (string.IsNullOrWhiteSpace(e.Value.ToString()))
            {
                ChangeValue(null, column);
            }
            else
            {
                if (typeAttr == "week")
                {
                    var value = DateTimeUtils.FromIso8601WeekDate(e.Value.ToString());
                    ChangeValue(value, column);
                }
                else if (typeAttr == "month")
                {
                    var value = DateTimeUtils.FromMonthDate(e.Value.ToString());
                    ChangeValue(value, column);
                }
                else
                {
                    var (type, _) = ((IGridColumn<T>)column).GetTypeAndValue(Item);
                    var typeConverter = TypeDescriptor.GetConverter(type);

                    if (typeConverter.IsValid(e.Value))
                    {
                        var value = typeConverter.ConvertFrom(e.Value);
                        ChangeValue(value, column);
                    }
                    else
                    {
                        ChangeValue(null, column);
                    }
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
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
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