## GridBlazor for ASP.NET Core MVC

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

1. Create a service with a method to get all items for the grid. An example of this type of service is: 

    ```c#
        public class OrderService
        {
            ...

            public ItemsDTO<Order> GetOrdersGridRows(Action<IGridColumnCollection<Order>> columns,
                    QueryDictionary<StringValues> query)
            {
                var repository = new OrdersRepository(_context);
                var server = new GridServer<Order>(repository.GetAll(), new QueryCollection(query), 
                    true, "ordersGrid", columns, 10);
            
                // return items to displays
                return server.ItemsToDisplay;
            }
        }
    ```

    **Notes**:
    * The method must have 2 parameters:
        * the first one is a lambda expression with the column definition of type **Action<IGridColumnCollection<T>>**
        * the second one is a dictionary to pass query parameters such as **grid-page**. It must be ot type **QueryDictionary<StringValues>**

    * You can use multiple methods of the **GridServer** object to configure a grid on the server. For example:
        ```c#
            var server = new GridServer<Order>(repository.GetAll(), Request.Query, true, "ordersGrid", columns, 10)
                .Sortable()
                .Filterable()
                .WithMultipleFilters();
        ```
    * The method returns an object including the model rows to be shown on the grid and other information requirired for paging, etc. The object type returned by the action must be **ItemsDTO<T>**.

    * You can use one of the following methods to get **ItemsDTO<T>** object:
        * ```server.ItemsToDisplay``` returns the object using the standard ```ToList``` method of the ```IQueryable<T>``` object supplied by ```System.Linq``` package
        * ```await server.GetItemsToDisplayAsync(Func<IQueryable<T>, Task<IList<T>>> toListAsync)``` returns the object using a custom```toListAsync``` method of the ```IQueryable<T>``` object. This method can be supplied by an ORM like EF Core. In this case we can call ```await server.GetItemsToDisplayAsync(async x => await x.ToListAsync())```

2. You have to register the service in the **Startup** class:

    ```c#
        public void ConfigureServices(IServiceCollection services)
        {
            ...

            services.AddScoped<OrderService>();
            
            ...
        }
    ```

3. Add a reference to **GridBlazor**, **GridBlazor.Pages**, **GridShared** and **GridShared.Utility** in the **_Imports.razor** file of the Blazor components folder

    ```razor
        ...
        @using GridBlazor
        @using GridBlazor.Pages
        @using GridShared
        @using GridShared.Utility
        ...
    ```

4. Create a Blazor component to render the grid. The component file must have a .razor extension. An example of a Blazor component is:

    ```razor
        @using GridMvc.Demo.Models
        @using GridMvc.Demo.Services
        @using Microsoft.Extensions.Primitives
        @using System.Globalization
        @using System.Threading.Tasks
        @inject IOrderService orderService

        @if (_task.IsCompleted)
        {
            <div class="row">
                <div class="col-md-12">
                    <GridComponent T="Order" Grid="@_grid"></GridComponent>
                </div>
            </div>
        }
        else
        {
            <p><em>Loading...</em></p>
        }

        @code
        {
            private CGrid<Order> _grid;
            private Task _task;

            protected override async Task OnParametersSetAsync()
            {
                var locale = CultureInfo.CurrentCulture;

                Action<IGridColumnCollection<Order>> columns = c =>
                {
                    c.Add(o => o.OrderId);
                    c.Add(o => o.CustomerID);
                    c.Add(o => o.Customer.CompanyName);
                    c.Add(o => o.OrderDate);
                };

                var query = new QueryDictionary<StringValues>();

                var client = new GridClient<Order>(q => orderService.GetOrdersGridRows(columns, q),
                    query, false, "ordersGrid", columns, locale);

                _grid = client.Grid;

                // Set new items to grid
                _task = client.UpdateGrid();
                await _task;
            }
        }
        
    ```

    **Notes**:
    * You must create a **GridClient** object in the **OnParametersSetAsync** of the razor page. This object contains a parameter of **CGrid** type called **Grid**. 

    * You can use multiple methods of the **GridClient** object to configure a grid. For example:
        ```c#
            var client = new GridClient<Order>(q => orderService.GetOrdersGridRows(columns, q), query, false, "ordersGrid", columns)
                .SetRowCssClasses(item => item.Customer.IsVip ? "success" : string.Empty)
                .Sortable()
                .Filterable()
                .WithMultipleFilters();
        ```

    * The **GridClient** object used on the razor page and the **GridServer** object on the service must have compatible settings.

    * You must call the **UpdateGrid** method of the **GridClient** object at the end of the **OnParametersSetAsync** of the razor page because it will request for the required rows to the server

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

5. Create a controller for the view that will contain the Blazor component:

    ```c#
        public ActionResult BlazorComponentView()
        {
            return View();
        }
    ```

6. Create a Razor view to call the Blazor component that contains the grid:

    ```razor
        @using GridMvc.Demo.BlazorComponents

        <div>
            @(await Html.RenderComponentAsync<OrdersComponent>(RenderMode.ServerPrerendered))
        </div>

        <script src="~/_framework/blazor.server.js" autostart="false"></script>

        <script type="text/javascript">
            Blazor.start({
                configureSignalR: function (builder) {
                    builder.withUrl("/_blazor");
                }
            });
        </script>
    ```
    
    **Notes**:
    * You must call the Blazor component with the ```RenderComponentAsync``` html helper.

    * You must call the ```blazor.server.js``` javascript with no autostart at the end of the view.

    * Finally you have to start the javascript manually to configure the correct Url to be used (/_blazor): 
        ```c#
            <script type="text/javascript">
                Blazor.start({
                    configureSignalR: function (builder) {
                        builder.withUrl("/_blazor");
                    }
                });
            </script>
        ```
 

For more documentation about column options, please see: [Custom columns](Custom_columns.md).

[<- Installation](Installation.md) | [GridBlazor configuration ->](GridBlazor_configuration.md)