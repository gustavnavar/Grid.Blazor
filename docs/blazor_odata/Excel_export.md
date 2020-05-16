## Blazor client-side with OData back-end

# Export to Excel

[Index](Documentation.md)

Grids can be exported to an Excel file. You have to use the ```SetExcelExport``` method of the ```GridClient``` object:
 
```c#
    var client = new GridODataClient<Order>(HttpClient, url, query, false, "ordersGrid", columns, 10)
        .SetExcelExport(true);
```

This is an example of a grid with an export to Excel button:

![](../images/Excel.png)


[<- Button components on the grid](Button_components.md)