## GridMvc for ASP.NET Core MVC

# Selecting row

[Index](Documentation.md)

The **GridServer** object used to create the grid in the controller has a method called **Selectable** to configure if a row can be selected. 
It's value can be **true** and **false**. 
Since the version 2.9.0 of the GridMvcCore nuget package the default value of the **Selectable** feature is **false** (it was **true** for earlier versions).

You can enable it as follows:
```c#
    var server = new GridServer<Order>(repository.GetAll(), Request.Query, false, "ordersGrid", columns)
        .Selectable(true);
```

Finally you have to add the javascript function called by the event. You can do this by using **onRowSelect** javascript function. Add the following script after **gridmvc.js** in your view:

```javascript
   <script>
        pageGrids.ordersGrid.onRowSelect(function (e) {
            alert("OrderId: " + e.row.OrderID);
        });
   </script>
```

Note that the name of the grid used in the **GridServer** constructor must be the same than the **pageGrids** attribute used in the javascript code. In this example the name is **ordersGrid**.

In this sample an alert is shown including a line of text with the selected row id.

In the GridComponent.Demo project you will find another example where the order details are shown on the view when a row is selected.

[<- Grouping](Grouping.md) | [Searching ->](Searching.md)