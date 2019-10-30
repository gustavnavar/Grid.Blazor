## GridMvc for ASP.NET Core MVC

# Grouping

[Index](Documentation.md)

You can also configure grouping using the **Groupable** method of the **Grid** object:
```razor
    @await Html.Grid(Model).Columns(columns =>
    {
        columns.Add(foo => foo.Title);
        columns.Add(foo => foo.Description);
    }).Groupable(true).RenderAsync()
```

You can drag the column title and drop it on the sorting area. 
You can add multiple columns at the same time and select if sorting is ascending or descending column by column.
The grid items will be grouped by the values of the selected columns.

This is an example of a table of items using grouping:

![](../images/Grouping.png)


[<- Sorting](Sorting.md) | [Selecting row ->](Selecting_row.md)