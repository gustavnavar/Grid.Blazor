using GridShared.Sorting;
using GridShared.Utility;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GridBlazor.Pages
{
    public partial class GridExtSortHeaderComponent<T>
    {
        protected IOrderedEnumerable<ColumnOrderValue> _sortedColumns;
        protected string _groupUrl;
        protected string _dropClass = "";

        [CascadingParameter(Name = "GridComponent")]
        protected internal GridComponent<T> GridComponent { get; set; }

        [Parameter]
        public ICGrid Grid { get; set; }

        protected override void OnParametersSet()
        {
            _sortedColumns = Grid.Settings.SortSettings.SortValues.OrderBy(r => r.Id);
            var queryBuilder = new CustomQueryStringBuilder(Grid.Settings.SortSettings.Query);
            var exceptQueryParameters = new List<string>();
            _groupUrl = queryBuilder.GetQueryStringExcept(exceptQueryParameters);
        }

        protected async Task TitleButtonClicked(ColumnOrderValue column)
        {
            await GridComponent.ChangeExtSorting(column);
        }

        protected async Task CancelButtonClicked(ColumnOrderValue column)
        {
            await GridComponent.RemoveExtSorting(column);
        }

        protected void HandleDragEnter()
        {
            _dropClass = "folderDragOver";
        }

        protected void HandleDragLeave()
        {
            _dropClass = "";
        }

        protected async Task HandleDrop()
        {
            _dropClass = "";
            if (Grid.Settings.SortSettings.SortValues.Any(r => r.ColumnName == GridComponent.Payload.ColumnName))
            {
                GridComponent.Payload = ColumnOrderValue.Null;
            }
            else
            {
                await GridComponent.AddExtSorting();
            }      
        }
    }
}
