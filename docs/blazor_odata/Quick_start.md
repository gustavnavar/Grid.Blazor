## Blazor WASM with OData back-end

# Quick start with GridBlazor

[Index](Documentation.md)

Imagine that you have to retrieve a collection of model items in your project. For example if your model class is:
    
```c#
    public class Order
    {
        [Key]
        public int OrderID { get; set; }
        public string CustomerID { get; set; }
        public DateTime? OrderDate { get; set; }
        public virtual Customer Customer { get; set; }
        ...
    }
```

The steps to build a grid razor page using **GridBlazor** are:

1. Add a reference to **GridBlazor**, **GridBlazor.Pages**, **GridShared** and **GridShared.Utility** in the **_Imports.razor** file of the client project's root folder

    ```razor
        ...
        @using GridBlazor
        @using GridBlazor.Pages
        @using GridShared
        @using GridShared.Utility
        ...
    ```

2. Create a razor page on the client project to render the grid using the **GridOdataClient** object. The page file must have a .razor extension. An example of razor page is:

    ```razor
        @page "/gridsample"
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

            Action<IGridColumnCollection<Order>> columns = c =>
            {
                c.Add(o => o.OrderID);
                c.Add(o => o.OrderDate, "OrderCustomDate").Format("{0:yyyy-MM-dd}");
                c.Add(o => o.Customer.CompanyName);
                c.Add(o => o.Customer.IsVip);
            };

            protected override async Task OnParametersSetAsync()
            {
                string url = NavigationManager.GetBaseUri() + "odata/orders";
                var query = new QueryDictionary<StringValues>();

                var client = new GridODataClient<Order>(HttpClient, url, query, false, "ordersGrid", columns, 10);
                _grid = client.Grid;

                // Set new items to grid
                _task = client.UpdateGrid();
                await _task;
            }
        }
    ```

3. Create an **ODataController** action in the server project. An example of this type of controller action is: 

    ```c#
        public class OrdersController : ODataController
        {
            private readonly NorthwindDbContext _context;

            public OrdersController(NorthwindDbContext context)
            {
               _context = context;
            }

            ...

            [EnableQuery]
            public IActionResult Get()
            {
                var repository = new OrdersRepository(_context);
                var orders = repository.GetAll();
                return Ok(orders);
            }
        }
    ```

**Notes**:
* You must create a **GridODataClient** object in the **OnParametersSetAsync** of the Blazor page. This object contains a parameter of **CGrid** type called **Grid**. 

* You can use multiple methods of the **GridODataClient** object to configure a grid. For example:
    ```c#
        var client = new GridODataClient<Order>(HttpClient, url, query, false, "ordersGrid", columns, 10, locale)
            .SetRowCssClasses(item => item.Customer.IsVip ? "success" : string.Empty)
            .Sortable()
            .Filterable()
            .WithMultipleFilters();
    ```

* You must call the **UpdateGrid** method of the **GridODataClient** object at the end of the **OnParametersSetAsync** of the razor page because it will request for the required rows to the server

* If you need to update the component out of ```OnParametersSetAsync``` method you must use a reference to the component:
    ```c#
        <GridComponent @ref="Component" T="Order" Grid="@_grid"></GridComponent>
    ```

    and then call the ```UpdateGrid``` method:
    ```c#
        await Component.UpdateGrid();
    ```

* The **GridComponent** tag must contain at least these 2 attributes:
    * **T**: type of the model items
    * **Grid**: grid object that has to be created in the **OnParametersSetAsync** method of the razor page

For more documentation about column options, please see: [Custom columns](Custom_columns.md).

[<- Installation](Installation.md) | [GridBlazor configuration ->](GridBlazor_configuration.md)