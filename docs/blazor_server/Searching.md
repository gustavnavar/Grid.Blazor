## Blazor server-side

# Searching

[Index](Documentation.md)

You can enable the searching option for your grid. Searching allows to search for a text on all columns at the same time.

![](../images/Searching.png)

You can enable searching for all columns of a grid using the **Searchable** method for both **GridClient** and **GridServer** objects:

* razor page
    ```c#
        var client = new GridClient<Order>(q => orderService.GetOrdersGridRows(columns, q), query, false, "ordersGrid", Columns, locale)
            .Searchable(true, false, true)
    ```

* service method
    ```c#
        var server = new GridServer<Order>(repository.GetAll(), Request.Query, true, "ordersGrid", columns, 10)
            .Searchable(true, false, true)
    ```


## Searching parameters

Parameter | Description | Example
--------- | ----------- | -------
enable (optional) | bool to enable searching on the grid | Searchable(true, ...)
onlyTextColumns (optional) | bool to enable searching on all collumns or just on string ones | Searchable(..., true, ...)
hiddenColumns (optional) | bool to enable searching on hidden columns | Searchable(..., true)

```enable``` default value is ```true```, ```onlyTextColumns``` default value is ```true```, and ```hiddenColumns``` default value is ```false```.

Searching on boolean columns has benn disabled because EF Core 3.0 is not supporting it yet.

[<- Selecting row](Selecting_row.md) | [Filtering ->](Filtering.md)