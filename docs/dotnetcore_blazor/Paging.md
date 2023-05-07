## GridBlazor for ASP.NET Core MVC

# Paging

[Index](Documentation.md)

You must configure the page size in the contructor of the **GridServer** object in the service method returning the rows to enable paging:

```c#
    var server = new GridServer<Order>(repository.GetAll(), Request.Query,
                true, "ordersGrid", columns, 10);
```

**PageSize** is an optional parameter of the **GridServer** constructor. If you don't want to enable paging you must call the contructor with no parameter:

```c#
    var server = new GridServer<Order>(repository.GetAll(), Request.Query,
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

## Hide the "Go To Page" section

This field used to go to an specific page writing its number is shown by default. You can hide it using the **GoToVisibility** method of the **GridClient**:

```c#
    var client = new GridClient<Order>(q => orderService.GetOrdersGridRows(ColumnCollections.OrderColumns, q),
            query, false, "ordersGrid", ColumnCollections.OrderColumns, locale)
        .GoToVisibility(false)
```

[<- Keyboard navigation](Keyboard_navigation.md) | [Custom columns ->](Custom_columns.md)