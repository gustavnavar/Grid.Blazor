## Blazor server-side

# Paging

[Index](Documentation.md)

You must configure the page size in the contructor of the **GridCoreServer** object in the service method returning the rows to enable paging:

```c#
    var server = new GridCoreServer<Order>(repository.GetAll(), Request.Query,
                true, "ordersGrid", columns, 10);
```

**PageSize** is an optional parameter of the **GridCoreServer** constructor. If you don't want to enable paging you must call the contructor with no parameter:

```c#
    var server = new GridCoreServer<Order>(repository.GetAll(), Request.Query,
                true, "ordersGrid", columns);
```

No configuration for the static page size is required on the **GridClient** object

## Change page size on the Razor page

You can configure the grid to enable changes on page size from the Razor page.

You have to enable paging as shown in the section before.

Then you have to use the **ChangePageSize** method of the **GridClient** object on the Razor page:

```c#
    var client = new GridClient<Order>(q => orderService.GetOrdersGridRows(ColumnCollections.OrderColumns, q),
            query, false, "ordersGrid", ColumnCollections.OrderColumns, locale)
            .ChangePageSize(true);
```

A user can change the page size writing the new size and pressing the "Tab" or "Enter" keys.

[<- Keyboard navigation](Keyboard_navigation.md) | [Custom columns ->](Custom_columns.md)