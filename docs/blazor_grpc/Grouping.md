## Blazor WASM with GridCore back-end (gRPC)

# Grouping

[Index](Documentation.md)

You can enable grouping for all columns of a grid using the **Groupable** method for both **GridClient** and **GridCoreServer** objects:
* Client project
    ```c#
        var client = new GridClient<Order>(gridClientService.GetOrdersGridRows, query, false, "ordersGrid", Columns, locale)
            .Groupable();
    ```

* Server project
    ```c#
        var server = new GridCoreServer<Order>(repository.GetAll(), Request.Query, true, "ordersGrid", columns, 10)
            .Groupable();
    ```

You can drag the column title and drop it on the sorting area. 
You can add multiple columns at the same time and select if sorting is ascending or descending column by column.
The grid items will be grouped by the values of the selected columns.

This is an example of a table of items using grouping:

![](../images/Grouping.png)


[<- Sorting](Sorting.md) | [Rearrange columns ->](RearrangeColumns.md)