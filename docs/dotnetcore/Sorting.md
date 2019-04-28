## GridMvc for ASP.NET Core MVC

# Sorting

[Index](Documentation.md)

You can enable sorting for all columns of a grid using the **Sortable** method of the **SGrid** object:
```razor
    @Html.Grid(Model, viewEngine).Columns(columns =>
    {
        columns.Add(foo => foo.Title);
        columns.Add(foo => foo.Description);
    }).Sortable()
```

[<- Custom columns](Custom_columns.md) | [Filtering ->](Filtering.md)