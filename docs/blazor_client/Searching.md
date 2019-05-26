## Blazor client-side

# Searching

[Index](Documentation.md)

You can enable the searching option for your grid. Searching allows to search for a text on all columns at the same time.

**IMPORTANT**: Searching can have a big impact on performance. It can force us to enable query client evaluation on the ORM (EF Core). You can enable it adding the following line to the **Startup.cs** file:
```c#
    options.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.QueryClientEvaluationWarning)); 
```

![](../images/Searching.png)

You can enable searching for all columns of a grid using the **Searchable** method for both **GridClient** and **GridServer** objects:

* Client project
    ```c#
        var client = new GridClient<Order>(url, query, false, "ordersGrid", Columns, locale)
            .Searchable(true, false);
    ```

* Server project
    ```c#
        var server = new GridServer<Order>(repository.GetAll(), Request.Query, true, "ordersGrid", columns, 10)
            .Searchable(true, false);
    ```

## Searching parameters

Parameter | Description | Example
--------- | ----------- | -------
enable | bool to enable searching on the grid | Searchable(true, ...)
onlyTextColumns | bool to enable searching on all collumns or just on string ones | Searchable(..., true)

[<- Sorting](Sorting.md) | [Filtering ->](Filtering.md)