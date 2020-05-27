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
    public partial class GridCreateComponent<T> : ICustomGridComponent<T>
    {
        private int _sequence = 0;
        private bool _shouldRender = false;
        protected QueryDictionary<bool> _isTooltipVisible;
        private QueryDictionary<RenderFragment> _renderFragments;
        private IEnumerable<string> _tabGroups;

        public string Error { get; set; } = "";
        public QueryDictionary<string> ColumnErrors { get; set; } = new QueryDictionary<string>();

        [CascadingParameter(Name = "GridComponent")]
        protected GridComponent<T> GridComponent { get; set; }

        [Parameter]
        public T Item { get; set; }

        protected override void OnParametersSet()
        {
            _renderFragments = new QueryDictionary<RenderFragment>();
            _isTooltipVisible = new QueryDictionary<bool>();
            foreach (var column in GridComponent.Grid.Columns)
            {
                // Name must have a non empty value
                if (string.IsNullOrWhiteSpace(column.Name))
                    column.Name = Guid.NewGuid().ToString();

                if (column.CreateComponentType != null)
                {
                    _renderFragments.Add(column.Name, GridCellComponent<T>.CreateComponent(_sequence,
                        column.CreateComponentType, column, Item, null, true));
                }

                _isTooltipVisible.AddParameter(column.Name, false);
            }
            _tabGroups = GridComponent.Grid.Columns
                .Where(r => !string.IsNullOrWhiteSpace(r.TabGroup) && _renderFragments.Keys.Any(s => s.Equals(r.Name)))
                .Select(r => r.TabGroup).Distinct();

            _shouldRender = true;
        }

        protected override bool ShouldRender()
        {
            return _shouldRender;
        }

        protected override void OnAfterRender(bool firstRender)
        {
            _shouldRender = false;
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

        private void ChangeValue(ChangeEventArgs e, IGridColumn column, string typeAttr = null)
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
                    var (type, _) = ((IGridColumn<T>)column).GetTypeAndValue(Item);
                    var typeConverter = TypeDescriptor.GetConverter(type);
                    if (typeConverter != null)
                    {
                        try
                        {
                            var value = typeConverter.ConvertFrom(e.Value.ToString());
                            SetValue(value, column);
                        }
                        catch (Exception)
                        {
                            SetValue(null, column);
                        }
                    }
                }
            }
        }

        public void DisplayTooltip(string columnName)
        {
            _isTooltipVisible.AddParameter(columnName, true);
            _shouldRender = true;
            StateHasChanged();
        }

        public void HideTooltip(string columnName)
        {
            _isTooltipVisible.AddParameter(columnName, false);
            _shouldRender = true;
            StateHasChanged();
        }

        protected async Task CreateItem()
        {
            try
            {
                Error = "";
                ColumnErrors = new QueryDictionary<string>();
                await GridComponent.CreateItem(this);
            }
            catch (GridException e)
            {
                _shouldRender = true;
                Error = string.IsNullOrWhiteSpace(e.Code) ? e.Message : e.Code + " - " + e.Message;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                _shouldRender = true;
                Error = Strings.CreateError;
            } 
        }

        protected void BackButtonClicked()
        {
            GridComponent.BackButton();
        }

    }
}