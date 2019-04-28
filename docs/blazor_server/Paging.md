## Blazor server-side

# Paging

[Index](Documentation.md)

You must configure the page size in the contructor of the **GridServer** object in the service method returning the rows to enable paging:

```c#
    var server = new GridServer<Order>(repository.GetAll(), Request.Query,
                true, "ordersGrid", GridSample.Columns, 10);
```

**PageSize** is an optional parameter of the **GridServer** constructor. If you don't want to enable paging you must call the contructor with no parameter:

```c#
    var server = new GridServer<Order>(repository.GetAll(), Request.Query,
                true, "ordersGrid", GridSample.Columns);
```

No configuration for paging is required on the **GridClient** object

[<- GridBlazor configuration](GridBlazor_configuration.md) | [Custom columns ->](Custom_columns.md)