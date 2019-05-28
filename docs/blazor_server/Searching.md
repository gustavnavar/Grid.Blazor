## Blazor server-side

# Searching

[Index](Documentation.md)

You can enable the searching option for your grid. Searching allows to search for a text on all columns at the same time.

![](../images/Searching.png)

You can enable searching for all columns of a grid using the **Searchable** method for both **GridClient** and **GridServer** objects:

* razor page
    ```c#
        var client = new GridClient<Order>(q => orderService.GetOrdersGridRows(columns, q), query, false, "ordersGrid", Columns, locale)
            .Searchable()
    ```

* service method
    ```c#
        var server = new GridServer<Order>(repository.GetAll(), Request.Query, true, "ordersGrid", columns, 10)
            .Searchable()
    ```


## Searching parameters

Parameter | Description | Example
--------- | ----------- | -------
enable | bool to enable searching on the grid | Searchable(true, ...)
onlyTextColumns | bool to enable searching on all collumns or just on string ones | Searchable(..., true)


**IMPORTANT**: If you get an **InvalidOperationException** while searching with a message similar to:
```text
Error generated for warning 'Microsoft.EntityFrameworkCore.Query.QueryClientEvaluationWarning: The LINQ expression 'where ...' could not be translated and will be evaluated locally.'. This exception can be suppressed or logged by passing event ID 'RelationalEventId.QueryClientEvaluationWarning' to the 'ConfigureWarnings' method in 'DbContext.OnConfiguring' or 'AddDbContext'.
``` 
Then you must enable query client evaluation on the ORM (EF Core). You can enable it adding the following line to the **Startup.cs** file:
```c#
    options.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.QueryClientEvaluationWarning)); 
```
Keep in mind that enabling query client evaluation on the ORM (EF Core) can have a big impact on performance.


[<- Sorting](Sorting.md) | [Filtering ->](Filtering.md)