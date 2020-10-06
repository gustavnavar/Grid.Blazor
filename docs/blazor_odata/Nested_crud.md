## Blazor client-side with OData back-end

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
    Func<object[], bool, bool, bool, bool, Task<IGrid>> subgrid = async (keys, create, read, update, delete) =>
    {
        var subGridQuery = new QueryDictionary<StringValues>();

        Action<IGridColumnCollection<OrderDetail>, string> subgridColumns = (c, baseUri) =>
        {
            c.Add(o => o.OrderID, true).SetPrimaryKey(true).SetWidth(100);
            c.Add(o => o.ProductID).SetPrimaryKey(true).Titled("ProdId").SetWidth(100)
               .SetSelectField(true, o => o.Product.ProductID.ToString() + " - " + o.Product.ProductName, () => GetAllProducts(baseUri));
            c.Add(o => o.Product.ProductName).Titled("ProdName").SetCrudHidden(true).SetWidth(250);
            c.Add(o => o.Quantity).Titled("Quant").SetWidth(100).Format("{0:F}");
            c.Add(o => o.UnitPrice).Titled("Unit Price").SetWidth(100).Format("{0:F}");
            c.Add(o => o.Discount).Titled("Disc").SetWidth(100).Format("{0:F}");
        };

        string subgridUrl = NavigationManager.BaseUri + $"odata/Orders({ keys[0].ToString()})/OrderDetails";
        var subgridClient = new GridODataClient<OrderDetail>(HttpClient, subgridUrl, subGridQuery, false,
            "orderDetailsGrid" + keys[0].ToString(), c => subgridColumns(c, NavigationManager.BaseUri), 10, locale)
                .Sortable()
                .Filterable()
                .SetStriped(true)
                .ODataCrud(create, read, update, delete)
                .WithMultipleFilters()
                .WithGridItemsCount();

        await subgridClient.UpdateGrid();
        return subgridClient.Grid;
    };
```
This function is passed as parameter of the ```Subgrid``` method used on the first step. Of course subgrids must be configured with CRUD support using the ```ODataCrud()``` method of the ```GridODataClient``` object.

## Showing the Update form just after inserting a row

You can configure CRUD to show the Update form just after inserting a new row with the Create form. 
It make sense to do it when you have nested grids and you want to create rows for the nested subgrid in the same step as creating the parent row.
You can do it using the ```SetEditAfterInsert``` method of the ```GridODataClient``` object

The configuration for this type of grid is as follows:

```c#
    var client = new GridODataClient<Order>(HttpClient, url, query, false, "ordersGrid", orderColumns, locale)
        .ODataCrud(true)
        .SetEditAfterInsert(true);
```

[<- CRUD](Crud.md) | [Events, exceptions and CRUD validation ->](Events.md)