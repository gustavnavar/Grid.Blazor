using GridShared.Columns;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GridBlazor
{
    public class GridGroupRowsComponentBase<T> : ComponentBase
    {
        protected IDictionary<string, GridGroupRowsComponent<T>> _children;
        protected string _columnName;
        protected IGridColumn _column;
        protected IEnumerable<object> _columnValues;

        [CascadingParameter(Name = "GridComponent")]
        protected GridComponent<T> GridComponent { get; set; }

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
        public int I { get; set; }

        protected override void OnParametersSet()
        {
            if (Grid.Settings.SortSettings.SortValues.Count > Values.Count)
            {
                _columnName = Grid.Settings.SortSettings.SortValues.OrderBy(r => r.Id).ElementAt(Values.Count).ColumnName;
                _column = Grid.Columns.SingleOrDefault(r => r.Name == _columnName);
                _columnValues = Grid.GetValuesToDisplay(_columnName, ItemsToDisplay).Distinct();

                _children = new Dictionary<string, GridGroupRowsComponent<T>>();
                foreach (object columnValue in _columnValues)
                {
                    var child = new GridGroupRowsComponent<T>();
                    child.IsVisible = true;
                    _children.Add(columnValue != null ? columnValue.ToString(): "", child);
                }
            }
        }

        protected void HandleGrouping(string groupId)
        {
            GridGroupRowsComponent<T> child;
            if (_children.TryGetValue(groupId, out child))
            {
                child.IsVisible = !child.IsVisible;
            }
        }
    }
}
