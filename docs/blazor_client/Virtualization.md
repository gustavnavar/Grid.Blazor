## Blazor WASM with GridMvcCore back-end (REST API)

# Grid virtualization

[Index](Documentation.md)

From GridBlazor 4.0.0 on, it's possible to use virtualization instead of grid pagination.

The steps to use virtualization on a grid are:

1. Create a razor page on the client project to render the grid. The ```Virtualize``` `method of the ```GridClient``` object must be called. The ```ChangeVirtualizedHeight``` method is optional. The page file must have a .razor extension. An example of razor page is:

    ```razor
        @page "/virtualized"
        @using GridShared
        @using GridShared.Utility
        @using Microsoft.Extensions.Primitives
        @inject NavigationManager NavigationManager
        @inject HttpClient HttpClient

        @if (_task.IsCompleted)
        {
            <GridComponent T="Order" Grid="@_grid"></GridComponent>
        }
        else
        {
            <p><em>Loading...</em></p>
        }

        @code
        {
            private CGrid<Order> _grid;
            private Task _task;

            public static Action<IGridColumnCollection<Order>> Columns = c =>
            {
                c.Add(o => o.OrderID);
                c.Add(o => o.OrderDate, "OrderCustomDate").Format("{0:yyyy-MM-dd}");
                c.Add(o => o.Customer.CompanyName);
                c.Add(o => o.Customer.IsVip);
            };

            protected override async Task OnParametersSetAsync()
            {
                string url = NavigationManager.GetBaseUri() + "api/SampleData/GetVirtualizedOrdersGrid";
                var query = new QueryDictionary<StringValues>();

                var client = new GridClient<Order>(HttpClient, url, query, false, "ordersGrid", Columns)
                    .Virtualize(250)
                    .ChangeVirtualizedHeight(true);
                
                _grid = client.Grid;
                _task = client.UpdateGrid();
                await _task;
            }
        }
    ```

2. Create a controller action in the server project. An example of this type of controller action is: 

    ```c#
        [Route("api/[controller]")]
        public class SampleDataController : Controller
        {
            ...

            [HttpGet("[action]")]
            public ActionResult GetVirtualizedOrdersGrid()
            {
                var repository = new OrdersRepository(_context);
                var server = new GridServer<Order>(repository.GetAll(), Request.Query,
                    true, "ordersGrid", Virtualized.Columns);

                return Ok(server.ItemsToDisplay);
            }
        }
    ```

The ```Virtualize``` method has 2 optional parameters:

Parameter | Description
--------- | -----------
height | integer to define the grid height (optional). The default value is 450 (pixels)
width | string to define the grid width (optional). The default value is "auto"

**Notes**:
* Grid virtualization is compatible with:
    - Sorting
    - Filtering
    - Extended sorting
    - Totals
    - Items count
    - Excel export
    - Column re-arrangment
    - CRUD (CRUD forms are shown as modal windows automatically)
    
* Grid virtualization is not compatible with:
    - Subgrids
    - Selectable grids
    - Checkbox columns
    - Searching
    - Grouping

[<- Grid dimensions](Grid_dimensions.md)