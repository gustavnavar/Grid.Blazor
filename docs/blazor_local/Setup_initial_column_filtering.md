## Blazor WASM with local data

# Setup initial column filtering

[Index](Documentation.md)

Sometimes you need to setup initial column filtering, just after the grid is first time loaded. After that a user can use this pre-filtered grid or clear/change filter settings.

You can do this, using the **SetInitialFilter** method:

```c#
    columns.Add(o => o.Customer.CompanyName)
        .Titled("Company Name")
        .ThenSortByDescending(o => o.OrderID)
        .SetInitialFilter(GridFilterType.StartsWith, "a")
```

[<- Creating custom filter widget](Creating_custom_filter_widget.md) | [Localization ->](Localization.md)