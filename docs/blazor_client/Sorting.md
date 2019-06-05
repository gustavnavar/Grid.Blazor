## Blazor client-side

# Sorting

[Index](Documentation.md)

You can enable sorting for all columns of a grid using the **Sortable** method for both **GridClient** and **GridServer** objects:
* Client project
    ```c#
        var client = new GridClient<Order>(url, query, false, "ordersGrid", Columns, locale)
            .Sortable();
    ```

* Server project
    ```c#
        var server = new GridServer<Order>(repository.GetAll(), Request.Query, true, "ordersGrid", columns, 10)
            .Sortable();
    ```

[<- Totals](Totals.md) | [Searching ->](Searching.md)