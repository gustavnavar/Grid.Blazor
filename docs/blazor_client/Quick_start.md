## Blazor client-side

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

1. Add a reference to **GridBlazor** in the **_Imports.razor** file of the client project's root folder

    ```razor
        ...
        @using GridBlazor
        ...
    ```

2. Create a razor page on the client project to render the grid. The page file must have a .razor extension. An example of razor page is:

    ```razor
        @page "/gridsample"
        @using GridShared
        @using GridShared.Utility
        @using Microsoft.Extensions.Primitives
        @inject IUriHelper UriHelper

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

            protected override async Task OnInitAsync()
            {
                string url = UriHelper.GetBaseUri() + "api/SampleData/GetOrdersGridForSample";

                var query = new QueryDictionary<StringValues>();
                query.Add("grid-page", "2");

                var client = new GridClient<Order>(url, query, false, "ordersGrid", Columns);
                _grid = client.Grid;

                // Set new items to grid
                _task = client.UpdateGrid();
                await _task;
            }
        }
    ```

3. Create a controller action in the server project. An example of this type of controller action is: 

    ```c#
        [Route("api/[controller]")]
        public class SampleDataController : Controller
        {
            ...

            [HttpGet("[action]")]
            public ActionResult GetOrdersGridForSample()
            {
                var repository = new OrdersRepository(_context);
                var server = new GridServer<Order>(repository.GetAll(), Request.Query,
                    true, "ordersGrid", GridSample.Columns, 10);

                // return items to displays
                return Ok(server.ItemsToDisplay);
            }
        }
    ```

**Notes**:
* It is important to declare the **Columns** lamba expression as *static* in the razor page, because it will be used by the server's web service.

* You must create a **GridClient** object in the **OnInitAsync** of the razor page. This object contains a parameter of **CGrid** type called **Grid**. 

* You can use multiple methods of the **GridClient** object to configure a grid. For example:
    ```c#
        var client = new GridClient<Order>(url, query, false, "ordersGrid", Columns, locale)
            .SetRowCssClasses(item => item.Customer.IsVip ? "success" : string.Empty)
            .Sortable()
            .Filterable()
            .WithMultipleFilters();
    ```

* You must call the **UpdateGrid** method of the **Grid** object at the end of the **OnInitAsync** of the razor page because it will request for the required rows to the server

* The **GridComponent** tag must contain at least these 2 attributes:
    * **T**: type of the model items
    * **Grid**: grid object that has to be created in the **OnInitAsync** method of the razor page

* You should use a **GridServer** object in the server controller action. 

* You can use multiple methods of the **GridServer** object to configure a grid on the server. For example:
    ```c#
        var server = new GridServer<Order>(repository.GetAll(), Request.Query, true, "ordersGrid", columns, 10)
                .Sortable()
                .Filterable()
                .WithMultipleFilters();
    ```

* The **GridClient** object on the client project and the **GridServer** object on the server project must have compatible settings.

* The server action returns a json including the model rows to be shown on the grid and other information requirired for paging, etc. The object type returned by the action must be **ItemsDTO<T>**.

For more documentation about column options, please see: [Custom columns](Custom_columns.md).

[<- Installation](Installation.md) | [GridBlazor configuration ->](GridBlazor_configuration.md)