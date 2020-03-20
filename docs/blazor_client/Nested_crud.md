## Blazor client-side

# Nested CRUD

[Index](Documentation.md)

GridBlazor supports subgrids in CRUD forms. Aside to edit, view and delete fields for an grid item using CRUD, you can add subgrids on the CRUD forms. 
And these subgrids can also be configured with CRUD support, so you can add, edit, view and delete items that have a 1:N relationship with the parent item.

### Column definition

Fist of all the column definition of the main grid must include the ```SubGrid``` method for those columns that have a 1:N relationship. 

```c#
    c.Add(o => o.OrderDetails).Titled("Order Details").SubGrid(subgrid, ("OrderID", "OrderID"));
```

If you have more that one subgrid in the CRUD form, you can show all them on a tab group. In this case you have to use an additional paramenter in the ```SubGrid``` method:

```c#
    c.Add(o => o.OrderDetails).Titled("Order Details").SubGrid("tabGroup1", subgrid, ("OrderID", "OrderID"));
```

These are the paraments of the ```Subgrid``` method:

Parameter name | Type | Description 
-------------- | ---- | -----------
TabGroup (optional) | ```string``` | Name of the tab group that will show all the subgrids
SubGrids | ```Func<object[], bool, bool, bool, bool, Task<IGrid>>``` | a funtion that will create the subgrid for each item column
Keys | ```params (string, string)[]``` | this array contains pairs of strings with the names of the columns that define the 1:N relationship for both tables

### Subgrid definition for the CRUD form

Then you have to define the subgrid that you want to show on the CRUD forms.

```c#
    Func<object[], bool, bool, bool, bool, Task<IGrid>> subGrids = async (keys, create, read, update, delete) =>
    {
        var subGridQuery = new QueryDictionary<StringValues>();
        string subGridUrl = NavigationManager.BaseUri + "api/SampleData/GetOrderDetailsGridWithCrud?OrderId="
            + keys[0].ToString();

        Action<IGridColumnCollection<OrderDetail>> subGridColumns = c => ColumnCollections.OrderDetailColumnsCrud(c,
            NavigationManager.BaseUri);

        var subGridClient = new GridClient<OrderDetail>(HttpClient, subGridUrl, subGridQuery, false,
            "orderDetailsGrid" + keys[0].ToString(), subGridColumns, locale)
                .Sortable()
                .Filterable()
                .SetStriped(true)
                .Crud(create, read, update, delete, orderDetailService)
                .WithMultipleFilters()
                .WithGridItemsCount();

        await subGridClient.UpdateGrid();
        return subGridClient.Grid;
    };
```
This function is passed as parameter of the ```Subgrid``` method used on the first step. Of course subgrids must be configured with CRUD support using the ```Crud()``` method of the ```GridClient``` object.

[<- CRUD](Crud.md) | [Events and CRUD validation ->](Events.md)