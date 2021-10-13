## Blazor WASM with OData back-end

# Paging

[Index](Documentation.md)

You must configure the page size in the contructor of the **GridODataClient** object in the client project to enable paging:

```c#
    var client = new GridODataClient<Order>(HttpClient, url, query, false, "ordersGrid", columns, 10, locale);
```

**PageSize** is an optional parameter of the **GridODataClient** constructor. If you don't want to enable paging you must call the contructor with no parameter:

```c#
    var client = new GridODataClient<Order>(HttpClient, url, query, false, "ordersGrid", columns);
```


## Change page size on the Razor page

You can configure the grid to enable changes on page size from the Razor page.

You have to enable paging as shown in the section before.

Then you have to use the **ChangePageSize** method of the **GridClient** object on the Razor page:

```c#
    var client = new GridODataClient<Order>(HttpClient, url, query, false, "ordersGrid", columns, 10, locale)
            .ChangePageSize(true);
```

A user can change the page size writing the new size and pressing the "Tab" or "Enter" keys.

[<- Keyboard navigation](Keyboard_navigation.md) | [Custom columns ->](Custom_columns.md)