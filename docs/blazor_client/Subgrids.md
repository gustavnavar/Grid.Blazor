## Blazor WASM with GridCore back-end (REST API)

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
            string subGridUrl = NavigationManager.GetBaseUri() + "api/SampleData/GetOrderDetailsGrid?OrderId=" + keys[0];
            var subGridQuery = new QueryDictionary<StringValues>();
            var subGridClient = new GridClient<OrderDetail>(httpClient, subGridUrl, subGridQuery, false, 
                "orderDetailsGrid" + keys[0].ToString(), OrderDetailColumns, locale)
                    .SetRowCssClasses(item => item.Quantity > 10 ? "success" : string.Empty)
                    .Sortable()
                    .Filterable()
                    .WithMultipleFilters()
                    .WithGridItemsCount();

            await subGridClient.UpdateGrid();
            return subGridClient.Grid;
        };
    }
```
The new element is a function to create the subgrids. The parameter **keys** must be an array of objects that the function will use to create the required url for the back-end web service. It's important to declare all variables needed by the contructor of the **GridClient** object inside the function block to avoid sharing parameters among subgrids. 

Then we have to modify the **GridClient** we used to create the main grid adding a **SubGrid** method:

```razor
    protected override async Task OnParametersSetAsync()
    {
        ...

        var client = new GridClient<Order>(httpClient, url, query, false, "ordersGrid", OrderColumns, locale)
            .SetRowCssClasses(item => item.Customer.IsVip ? "success" : string.Empty)
            .Sortable()
            .Filterable()
            .WithMultipleFilters()
            .Searchable(true, false)
            .WithGridItemsCount()
            .SubGrid(subGrids, ("OrderID","OrderID"));

    }
```

## SubGrid parameters

Parameter | Type | Description
--------- | ---- | -----------
subGrids | Func<object[], Task<ICGrid>> | function that creates subgrids defined in the step before
allOpended | bool (optional) | boolean to configure if all subgrids are opened or closed when the grid is initialized or updated. The default value is false.
keys | params (string, string)[] | variable number of tuples of strings with the names of required columns to find records for the subgrid (foreign keys). The first value of the tuple is the name of property of the parent grid and the second value is the name of property of the child grid.

Finally we have to add an action to the back-end controller to get rows for subgrids. An example of this type of action is: 

```c#
    [Route("api/[controller]")]
    public class SampleDataController : Controller
    {
        ...

        [HttpGet("[action]")]
        public ActionResult GetOrderDetailsGrid(int OrderId)
        {
            var orderDetails = (new OrderDetailsRepository(_context)).GetForOrder(OrderId);

            var server = new GridCoreServer<OrderDetail>(orderDetails, Request.Query,
                    false, "orderDetailsGrid" + OrderId.ToString(), GridSample.OrderDetailColumns)
                        .WithPaging(10)
                        .Sortable()
                        .Filterable()
                        .WithMultipleFilters();

            var items = server.ItemsToDisplay;
            return Ok(items);
        }
    }
```

This action is very similar to the one we used for the main grid. The only difference is that this one has parameters to get the subgrid rows. It can be only one parameter or more depending on your database model.
In our example it is only one element for the **OrderId** field.
We use it to get the subgrid rows calling the **GetForOrder** method from the repository.

Note that the grid name parameter we use must be unique for each subgrid. In this example we use the name **"orderDetailsGrid" + OrderId.ToString()**.

[<- Render button, checkbox, etc. in a grid cell](Render_button_checkbox_etc_in_a_grid_cell.md) | [Passing grid state as parameter ->](Passing_grid_state_as_parameter.md)