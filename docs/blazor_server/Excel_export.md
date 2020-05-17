## Blazor server-side

# Export to Excel

[Index](Documentation.md)

Grids can be exported to an Excel file. You have to use the ```SetExcelExport``` method of the ```GridClient``` object:
 
```c#
    var client = new GridClient<Order>(q => orderService.GetOrdersGridRows(columns, q), query, false, "ordersGrid", columns)
        .SetExcelExport(true);
```

This is an example of a grid with an export to Excel button:

![](../images/Excel.png)


[<- Button components on the grid](Button_components.md)