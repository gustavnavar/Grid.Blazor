using GridShared.Columns;
using GridShared.Utility;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GridBlazor.Pages
{
    public partial class GridGroupRowsComponent<T>
    {
        protected string _columnName;
        protected IGridColumn _column;
        protected IEnumerable<object> _columnValues;

        public QueryDictionary<GridGroupRowsComponent<T>> Children { get; private set; }

        [CascadingParameter(Name = "GridComponent")]
        protected GridComponent<T> GridComponent { get; private set; }

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

        protected override void OnParametersSet()
        {
            if (Grid.Settings.SortSettings.SortValues.Count > Values.Count)
            {
                _columnName = Grid.Settings.SortSettings.SortValues.OrderBy(r => r.Id).ElementAt(Values.Count).ColumnName;
                _column = Grid.Columns.SingleOrDefault(r => r.Name == _columnName);
                _columnValues = Grid.GetValuesToDisplay(_columnName, ItemsToDisplay).Distinct();

                Children = new QueryDictionary<GridGroupRowsComponent<T>>();
                foreach (object columnValue in _columnValues)
                {
                    var child = new GridGroupRowsComponent<T>();
                    child.IsVisible = true;
                    Children.Add(columnValue != null ? columnValue.ToString(): "", child);
                }
            }
        }

        protected void HandleGrouping(string groupId)
        {
            GridGroupRowsComponent<T> child;
            if (Children.TryGetValue(groupId, out child))
            {
                child.IsVisible = !child.IsVisible;
            }
        }
    }
}
