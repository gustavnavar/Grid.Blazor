## Blazor server-side

# Sorting

[Index](Documentation.md)

You can enable sorting for all columns of a grid using the **Sortable** method for both **GridClient** and **GridServer** objects:
* razor page
    ```c#
        var client = new GridClient<Order>(q => orderService.GetOrdersGridRows(columns, q), query, false, "ordersGrid", Columns, locale)
            .Sortable()
    ```

* service method
    ```c#
        var server = new GridServer<Order>(repository.GetAll(), Request.Query, true, "ordersGrid", columns, 10)
            .Sortable()
    ```

[<- Custom columns](Custom_columns.md) | [Searching ->](Searching.md)