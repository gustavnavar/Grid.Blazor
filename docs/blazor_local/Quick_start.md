## Blazor WASM with local data

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
                var server = new GridCoreServer<Order>(Orders, query, true, "ordersGrid", columns);

                var items = server.ItemsToDisplay;
                return items;
            }
        }
    ```

    **Notes**:
    * The method must have 2 parameters:
        * the first one is a lambda expression with the column definition of type **Action<IGridColumnCollection<T>>**
        * the second one is a dictionary to pass query parameters such as **grid-page**. It must be ot type **QueryDictionary<StringValues>**

    * You can use multiple methods of the **GridCoreServer** object to configure a grid on the server. For example:
        ```c#
            var server = new GridCoreServer<Order>(repository.GetAll(), Request.Query, true, "ordersGrid", columns, 10)
                .Sortable()
                .Filterable()
                .WithMultipleFilters();
        ```
    * The method returns an object including the model rows to be shown on the grid and other information requirired for paging, etc. The object type returned by the action must be **ItemsDTO<T>**.

2. You have to register the service in the **Program** class:

    ```c#
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            ...

            builder.Services.AddScoped<OrderService>();
            
            ...
        }
    ```

3. Add a reference to **GridBlazor**, **GridBlazor.Pages**, **GridShared** and **GridShared.Utility** in the **_Imports.razor** file of the root folder

    ```razor
        ...
        @using GridBlazor
        @using GridBlazor.Pages
        @using GridShared
        @using GridShared.Utility
        ...
    ```

4. Create a razor page to render the grid. The page file must have a .razor extension. An example of razor page is:

    ```razor
        @page "/gridsample"
        @using Microsoft.Extensions.Primitives
        @inject OrderService orderService

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

            protected override async Task OnParametersSetAsync()
            {
                Action<IGridColumnCollection<Order>> columns = c =>
                {
                    c.Add(o => o.OrderID);
                    c.Add(o => o.OrderDate, "OrderCustomDate").Format("{0:yyyy-MM-dd}");
                    c.Add(o => o.Customer.CompanyName);
                    c.Add(o => o.Customer.IsVip);
                };

                var query = new QueryDictionary<StringValues>();
                query.Add("grid-page", "2");

                var client = new GridClient<Order>(q => orderService.GetOrdersGridRows(columns, q), query, false, "ordersGrid", columns);
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

    * The **GridClient** object used on the razor page and the **GridCoreServer** object on the service must have compatible settings.

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

For more documentation about column options, please see: [Custom columns](Custom_columns.md).

[<- Installation](Installation.md) | [GridBlazor configuration ->](GridBlazor_configuration.md)