## Blazor WASM with GridCore back-end (gRPC)

# Subgrids

[Index](Documentation.md)

You can enable the subgrids support for your grid. Subgrids allows to view records for those tables that have a 1 to N relationship with the parent table of the main grid.

![](../images/Subgrids.png)

We asume that you already configured the parent grid and it's working as expected as described in the [Quick start](Quick_start.md) section.

We have to add the subgrid column definition on **code** section of the same razor page we used to render the main grid. It must be an **static** object:

```razor
    @code
    {
        ...
        
        public static Action<IGridColumnCollection<OrderDetail>> OrderDetailColumns = c =>
        {
            c.Add(o => o.OrderID);
            c.Add(o => o.ProductID);
            c.Add(o => o.Product.ProductName);
            c.Add(o => o.Quantity).Format("{0:F}");
            c.Add(o => o.UnitPrice).Format("{0:F}");
        };

        ...
    }
```

We have to add the following element on **OnParametersSetAsync** method of the same razor page we used to render the main grid:

```razor
    protected override async Task OnParametersSetAsync()
    {
        ...
        
        Func<object[], Task<ICGrid>> subGrids = async keys =>
        {
            int orderId;
            int.TryParse(keys[0].ToString(), out orderId);
            var subGridQuery = new QueryDictionary<string>();
            var subGridClient = new GridClient<OrderDetail>(q => gridClientService.GetOrderDetailsGrid(q, orderId), subGridQuery, false,
                "orderDetailsGrid" + keys[0].ToString(), OrderDetailColumns, locale)
                    .Sortable()
                    .Filterable()
                    .SetStriped(true)
                    .WithMultipleFilters()
                    .WithGridItemsCount();

            await subGridClient.UpdateGrid();
            return subGridClient.Grid;
        };
    }
```

The new element is a function to create the subgrids. The parameter **keys** must be an array of objects that the function will use to create the required url for the back-end dRPC service. It's important to declare all variables needed by the contructor of the **GridClient** object inside the function block to avoid sharing parameters among subgrids. 

Then we have to modify the **GridClient** we used to create the main grid adding a **SubGrid** method:

```razor
    protected override async Task OnParametersSetAsync()
    {
        ...

        var client = new GridClient<Order>(gridClientService.GetOrdersGridRows, query, false, "ordersGrid", Columns, locale)
            .Sortable()
            .Filterable()
            .SetStriped(true)
            .WithMultipleFilters()
            .WithGridItemsCount()
            .SubGrid(subGrids, ("OrderID","OrderID"));
        ...
    }
```

## SubGrid parameters

Parameter | Type | Description
--------- | ---- | -----------
subGrids | Func<object[], Task<ICGrid>> | function that creates subgrids defined in the step before
allOpended | bool (optional) | boolean to configure if all subgrids are opened or closed when the grid is initialized or updated. The default value is false.
keys | params (string, string)[] | variable number of tuples of strings with the names of required columns to find records for the subgrid (foreign keys). The first value of the tuple is the name of property of the parent grid and the second value is the name of property of the child grid.


Finally we have to add a method in the gRPC service to get rows for subgrids. An example of this type of method is: 

* in the **Server** project:
    ```c#
        public async ValueTask<ItemsDTO<OrderDetail>> GetOrderDetailsGrid(Request request)
        {
            var orderDetails = (new OrderDetailsRepository(_context)).GetForOrder(request.Id);

            var server = new GridCoreServer<OrderDetail>(orderDetails, request.Query,
                    false, "orderDetailsGrid" + request.Id.ToString(), ColumnCollections.OrderDetailColumns)
                        .WithPaging(10)
                        .Sortable()
                        .Filterable()
                        .WithMultipleFilters()
                        .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");

            var items = await server.GetItemsToDisplayAsync(async x => await x.ToListAsync());
            return items;
        }
     ```

* in the **Client** project:
    ```c#
        public async Task<ItemsDTO<OrderDetail>> GetOrderDetailsGrid(QueryDictionary<string> query, int orderId)
        {
            var handler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler());
            using (var channel = GrpcChannel.ForAddress(_baseUri, new GrpcChannelOptions() { HttpClient = new HttpClient(handler) }))
            {
                var service = channel.CreateGrpcService<IGridService>();
                return await service.GetOrderDetailsGrid(new Request(orderId, query));
            }
        }
     ```

    **Notes**:
    * This method must have ONLY 1 serializable parameter to avoid gRPC errors:
        * a dictionary to pass query parameters such as **grid-page**. It must be of type ```QueryDictionary<string>```. gRPC forces to use a dictionary of strings. All other protocols use a dictionary of ```StringValues```, but gRPC does not serialize it by default.
        * it can be any other serializable object that includes ```QueryDictionary<string>``` as an attribute

Note that the grid name parameter we use must be unique for each subgrid. In this example we use the name ```"orderDetailsGrid" + OrderId.ToString()```.

[<- Render button, checkbox, etc. in a grid cell](Render_button_checkbox_etc_in_a_grid_cell.md) | [Passing grid state as parameter ->](Passing_grid_state_as_parameter.md)