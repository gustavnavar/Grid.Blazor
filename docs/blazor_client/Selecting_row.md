## Blazor client-side

# Selecting row

[Index](Documentation.md)

The **GridClient** object has a method called **Selectable** to configure if a row can be selected. 
It's value can be **true** and **false**. 
Since the version 1.1.0 of the GridBlazor nuget package the default value of the **Selectable** feature is **false** (it was **true** for earlier versions).

There are optional parameters to control selection behavior:

- Auto Select First Row:
    There is an optional boolean parameter to control if the first row should automatically be selected when a page loads.
    It's value can be **true** and **false**. 
    By default this parameter's value is **false**. 
- Allow Multi Select:
    There is an optional boolean paramter to control if multiple rows can be selected. 
    It's value can be **true** and **false**.
    By default this parameter's value is **false**.
    You can select multiple rows while pressing the [Ctrl] key.

You can enable it as follows:
```c#
    var client = new GridClient<Order>(httpClient, url, query, false, "ordersGrid", Columns)
        .Selectable(true, true, true);
```

You have to add the **OnRowClicked** attribute on the component. For example, the razor page can contain the following line:
```razor
    <GridComponent T="Order" Grid="@_grid" OnRowClicked="@OrderDetails"></GridComponent>
```

Then you have to add the function called by the event. In this example its name is **OrderDetails**:
```c#
    protected void OrderDetails(object item)
    {
        Order order = null;
        if (item.GetType() == typeof(Order))
        {
            order = (Order)item;
        }
        Console.WriteLine("Order Id: " + (order == null ? "NULL" : order.OrderID.ToString()));
    }
```

In this sample a line of text with the selected row id is writen on the console log.

When items are selected in grid, collection of selected items are available using SelectedItems property. SelectedItems property is of type IEnumerable<object>.

```c#
    var items = client.Grid.SelectedItems.ToList<T>();
```

In the GridBlazor.Demo.Client project you will find another example where the order details are shown on a component when a row is selected.

## Selecting rows for subgrids

GridBlazor 1.3.30 and newer versions implement ```OnRowClickedActions``` to allow row click for all subgrids.

You have to create a ```List<Action<object>>()``` and add all row click method for nested subgrids in the same order they are nested:

```c#
    _rowClickActions = new List<Action<object>>();
    _rowClickActions.Add(OrderInfo);
    _rowClickActions.Add(OrderDetailInfo);
```

And finally you have to pass the list as parameter of the ```GridComponent```:

```c#
    <GridComponent T="Order" Grid="@_grid" OnRowClickedActions="@_rowClickActions" />
```

[<- Grouping](Grouping.md) | [Searching ->](Searching.md)