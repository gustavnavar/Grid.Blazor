## Blazor WASM with GridCore back-end (gRPC)

# Quick start with GridBlazor

[Index](Documentation.md)

We will follow the "Code-first gRPC" approach, as explained [here](https://docs.microsoft.com/en-us/aspnet/core/grpc/code-first?view=aspnetcore-6.0).

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

The steps to build a grid razor page using **GridBlazor** using gRPC to communicate with the back-end are:

1. Use **DataContract** and **DataMember** attributes for all model items defined in the **Shared** project. 

```c#
    [DataContract]
    public class Order
    {
        [Key]
        [DataMember(Order = 1)]
        public int OrderID { get; set; }
        [DataMember(Order = 2)]
        public string CustomerID { get; set; }
        [DataMember(Order = 3)]
        public DateTime? OrderDate { get; set; }
        [DataMember(Order = 4)]
        public virtual Customer Customer { get; set; }
        ...
    }
```

2. Create an interface in the **Shared** project for the gRPC service used by **Client** and **Server** projects. 
The interface must use the **ServiceContract** attribute:
```c#
    [ServiceContract]
    public interface IGridService
    {
        ValueTask<ItemsDTO<Order>> GetOrdersGrid(QueryDictionary<string> query);
    }
```

3. Create a service implementing this interface in the **Server** project. An example of this type of service is: 

    ```c#
        public class GridServerService : IGridService
        {
            ...

            public async ValueTask<ItemsDTO<Order>> GetOrdersGrid(QueryDictionary<string> query)
            {
                var repository = new OrdersRepository(_context);
                var server = new GridCoreServer<Order>(repository.GetAll(), query, true, "ordersGrid", Index.Columns)
                        .WithPaging(10);

                var items = await server.GetItemsToDisplayAsync(async x => await x.ToListAsync());
                return items;
            }
        }
    ```

    **Notes**:
    * The method must have ONLY 1 serializable parameter to avoid gRPC errors:
        * a dictionary to pass query parameters such as **grid-page**. It must be of type **QueryDictionary<string>**. gRPC forces to use a dictionary of strings. All other protocols use a dictionary of ```StringValues```, but gRPC does not serialize it by default.
        * it can be any other serializable object that includes **QueryDictionary<string>** as an attribute

    * You can use multiple methods of the **GridCoreServer** object to configure a grid on the server. For example:
        ```c#
            var server = new GridCoreServer<Order>(repository.GetAll(), query, true, "ordersGrid", Index.Columns)
                .WithPaging(10)
                .Sortable()
                .Filterable()
                .WithMultipleFilters();
        ```

    * The **GridCoreServer** constructor can use the same column definition that we will create in the last step, if it is defined as static

    * The method returns an object including the model rows to be shown on the grid and other information requirired for paging, etc. The object type returned by the action must be **ItemsDTO<T>**.
    
    * You must use the following method to get **ItemsDTO<T>** object:
        * ```await server.GetItemsToDisplayAsync(Func<IQueryable<T>, Task<IList<T>>> toListAsync)``` returns the object using a custom```toListAsync``` method of the ```IQueryable<T>``` object. This method can be supplied by an ORM like EF Core. In this case we can call ```await server.GetItemsToDisplayAsync(async x => await x.ToListAsync())```

4. You have to register the following items
   - Code first Grpc with ```AddCodeFirstGrpc``` method
   - IGridService as a scoped service
   - GrpcWeb middleware after routing
   - a gRPC end point for IGridService
   in the **Startup** class:

    ```c#
        public void ConfigureServices(IServiceCollection services)
        {
            ...

            services.AddCodeFirstGrpc(options =>
            {
                options.ResponseCompressionLevel = CompressionLevel.Optimal;
            });

            services.AddScoped<IGridService, GridServerService>();
            
            ...
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            ...

            app.UseRouting();
            app.UseGrpcWeb(new GrpcWebOptions() { DefaultEnabled = true });

            app.UseEndpoints(endpoints =>
            {
                ...

                endpoints.MapGrpcService<IGridService>();
                endpoints.MapFallbackToFile("index.html");
            });
        }
    ```

5. Create a service in the **Client** project to call the back-end service defined in the step 3 (for this example is ```GetOrdersGrid```). An example of this type of service is: 

    ```c#
        public class GridClientService : IGridClientService
        {
            private readonly string _baseUri;

            public GridClientService(NavigationManager navigationManager)
            {
                _baseUri = navigationManager.BaseUri;
            }

            public async Task<ItemsDTO<Order>> GetOrdersGridRows(QueryDictionary<string> query)
            {
                var handler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler());
                using (var channel = GrpcChannel.ForAddress(_baseUri, new GrpcChannelOptions() { HttpClient = new HttpClient(handler) }))
                {
                    var service = channel.CreateGrpcService<IGridService>();
                    return await service.GetOrdersGrid(query);
                }
            }
        }

        public interface IGridClientService
        {
            Task<ItemsDTO<Order>> GetOrdersGridRows(QueryDictionary<string> query);
        }
    ```

6. You have to register the previous service in the **Program** class of the **Client** project:

    ```c#
        public static async Task Main(string[] args)
        {
            ...

            builder.Services.AddScoped<IGridClientService, GridClientService>();
            
            ...
        }
    ```

7. Add a reference to **GridBlazor**, **GridBlazor.Pages**, **GridShared** and **GridShared.Utility** in the **_Imports.razor** file of the **Client** project's root folder

    ```razor
        ...
        @using GridBlazor
        @using GridBlazor.Pages
        @using GridShared
        @using GridShared.Utility
        ...
    ```

8. Create a razor page on the **Client** project to render the grid. The page file must have a .razor extension. An example of razor page is:

    ```razor
        @page "/"
        @using GridBlazorGrpc.Client.ColumnCollections
        @using GridBlazorGrpc.Shared.Models
        @using Microsoft.Extensions.Primitives
        @using System.Globalization
        @using System.Threading.Tasks
        @inject IGridClientService gridClientService

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

            public static Action<IGridColumnCollection<Order>> Columns = c =>
            {
                c.Add(o => o.OrderID).Titled("Number");
                c.Add(o => o.OrderDate).Titled("Date");
                c.Add(o => o.Customer.CompanyName).Titled("Company Name");
                c.Add(o => o.Customer.ContactName).Titled("Contact Name");
                c.Add(o => o.Freight);
            };

            protected override async Task OnParametersSetAsync()
            {
                var locale = CultureInfo.CurrentCulture;
                var query = new QueryDictionary<string>();
                var client = new GridClient<Order>(gridClientService.GetOrdersGridRows, query, false, "ordersGrid", Columns, locale);

                _grid = client.Grid;
                _task = client.UpdateGrid();
                await _task;
            }
        }
    ```

**Notes**:
* It is important to declare the **Columns** lamba expression as *static* in the razor page, because it will be used by the server's gRPC service.

* You must create a **GridClient** object in the **OnParametersSetAsync** of the Blazor page. This object contains a parameter of **CGrid** type called **Grid**. 

* You can use multiple methods of the **GridClient** object to configure a grid. For example:
    ```c#
        var client = new GridClient<Order>(gridClientService.GetOrdersGridRows, query, false, "ordersGrid", Columns, locale)
            .Sortable()
            .Filterable()
            .SetStriped(true)
            .WithMultipleFilters();
    ```

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

* You must use a **GridCoreServer** object in the server gRPC method.

* You can use multiple methods of the **GridCoreServer** object to configure a grid on the server. For example:
    ```c#
        var server = new GridCoreServer<Order>(repository.GetAll(), Request.Query, true, "ordersGrid", columns, 10)
                .Sortable()
                .Filterable()
                .WithMultipleFilters();
    ```

* The **GridClient** object on the client project and the **GridCoreServer** object on the server project must have compatible settings.

* The server gRPC service returns a ```protobuf``` message including the model rows to be shown on the grid and other information requirired for paging, etc. The object type returned by the service must be **ItemsDTO<T>**.

For more documentation about column options, please see: [Custom columns](Custom_columns.md).

[<- Installation](Installation.md) | [GridBlazor configuration ->](GridBlazor_configuration.md)