using GridShared.Columns;
using GridShared.Utility;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GridBlazor.Pages
{
    public partial class GridGroupRowsComponent<T>
    {
        protected string _columnName;
        protected IGridColumn _column;
        protected IEnumerable<object> _columnValues;

        public QueryDictionary<(GridGroupRowsComponent<T> Component, string Label)> Children { get; private set; }

        [CascadingParameter(Name = "GridComponent")]
        protected internal GridComponent<T> GridComponent { get; private set; }

        [Parameter]
        public ICGrid Grid { get; set; }

        [Parameter]
        public IList<Tuple<string, object>> Values { get; set; }

        [Parameter]
        public bool HasSubGrid { get; set; }

        [Parameter]
        public bool RequiredTotalsColumn { get; set; }

        [Parameter]
        public IEnumerable<object> ItemsToDisplay { get; set; }

        [Parameter]
        public bool IsVisible { get; set; }

        [Parameter]
        public int RowId { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            if (Grid.Settings.SortSettings.SortValues.Count > Values.Count)
            {
                _columnName = Grid.Settings.SortSettings.SortValues.OrderBy(r => r.Id).ElementAt(Values.Count).ColumnName;
                _column = Grid.Columns.SingleOrDefault(r => r.Name == _columnName);
                var group = (Grid as ICGrid<T>).GetGroup(_columnName);
                _columnValues = (Grid as ICGrid<T>).GetGroupValues(group, ItemsToDisplay).Distinct();

                Children = new QueryDictionary<(GridGroupRowsComponent<T> Component, string Label)>();
                foreach (object columnValue in _columnValues)
                {
                    var child = new GridGroupRowsComponent<T>();
                    child.IsVisible = true;
                    string label = _column?.Title + ": ";
                    if (columnValue != null && group != null && group.GroupLabel != null)
                        label += await group.GroupLabel(columnValue);
                    else
                        label += _column?.GetFormatedValue(columnValue);
                    try
                    {
                        string key = columnValue != null ? columnValue.ToString() : "";
                        Children.Add(key, (child, label));
                    }
                    catch (ArgumentException)
                    {
                        // do nothing because key is already in dictionary
                    }
                }
            }
        }

        protected void HandleGrouping(string key)
        {
            (GridGroupRowsComponent<T> Component, string Label) child;
            if (Children.TryGetValue(key, out child))
            {
                child.Component.IsVisible = !child.Component.IsVisible;
            }
        }
    }
}
