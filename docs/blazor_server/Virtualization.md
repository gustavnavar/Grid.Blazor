## Blazor server-side

# Grid virtualization

[Index](Documentation.md)

From GridBlazor 4.0.0 on, it's possible to use virtualization instead of grid pagination.

The steps to use virtualization on a grid are:

1. Create a razor page to render the grid. The ```Virtualize``` `method of the ```GridClient``` object must be called. The ```ChangeVirtualizedHeight``` method is optional. The page file must have a .razor extension. An example of razor page is:

    ```razor
        @page "/virtualized"
        @using GridShared
        @using GridShared.Utility
        @using Microsoft.Extensions.Primitives
        @inject IOrderService orderService

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
                var query = new QueryDictionary<StringValues>();

                var client = new GridClient<Order>(orderService.GetVirtualizedOrdersAsync, query, false, "ordersGrid", Columns)
                    .Virtualize(250)
                    .ChangeVirtualizedHeight(true);
                
                _grid = client.Grid;
                _task = client.UpdateGrid();
                await _task;
            }
        }
    ```

2. Create a service with a method to get all items for the grid. An example of this type of service is:  

    ```c#
        public async Task<ItemsDTO<Order>> GetVirtualizedOrdersAsync(QueryDictionary<StringValues> query)
        {
            using (var context = new NorthwindDbContext(_options))
            {
                var repository = new OrdersRepository(context);

                var server = new GridCoreServer<Order>(repository.GetAll(), query, true, "ordersGrid", Virtualized.Columns);

                // return items to displays
                var items = await server.GetItemsToDisplayAsync(async x => await x.ToListAsync());
                return items;
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
    
* Grid virtualization is not compatible with:
    - Subgrids
    - Selectable grids
    - Checkbox columns
    - Searching
    - Grouping
    - CRUD

[<- Grid dimensions](Grid_dimensions.md)