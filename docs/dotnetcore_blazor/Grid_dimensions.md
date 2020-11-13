## GridBlazor for ASP.NET Core MVC

# Grid dimensions

[Index](Documentation.md)

The default grid configuration allows the broswer to calculate its height and width. 

You can configure the column width using the ```SetWidth``` method of the column definition object as follows:
 ```c#
    Columns.Add(o => o.CompanyName).SetWidth(220);
    Columns.Add(o => o.ContactName).SetWidth("20%");
    Columns.Add(o => o.Address).SetWidth("25em");
      
```

But you can also configure the grid height and width using the ```SetTableLayout``` method of the ```GridClient``` object:
 
```c#
    var client = GridClient<Order>(q => orderService.GetOrdersGridRows(columns, q),
         query, false, "ordersGrid", columns, locale)
        .SetTableLayout(TableLayout.Fixed, "1200px", "400px");
```

The ```SetTableLayout``` method has 3 parameters, one of them is required:

Parameter | Description
--------- | -----------
tableLayout | enum to enable fixed dimensions (required)
width | string to define the grid width (optional). The default value is "auto"
height | string to define the grid height (optional). The default value is "auto"

It's recommended to configure the width of all collumns using the ```SetWidth``` method. 
If you don't do it, the default column width (12em) will be applied.

Scrollbars will be added automatically by the grid component in case it will be necessary.

[<- Export to Excel](Excel_export.md)