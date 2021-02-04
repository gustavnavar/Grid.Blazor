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

## Showing the Update form just after inserting a row

You can configure CRUD to show the Update form just after inserting a new row with the Create form. 
It make sense to do it when you have nested grids and you want to create rows for the nested subgrid in the same step as creating the parent row.
You can do it using the ```SetEditAfterInsert``` method of the ```GridClient``` object

The configuration for this type of grid is as follows:

```c#
    var client = new GridClient<Order>(HttpClient, url, query, false, "ordersGrid", orderColumns, locale)
        .Crud(true, orderService)
        .SetEditAfterInsert(true);
```

## Hiding the parent CRUD form buttons when opening a child CRUD form

When you have 2 nested CRUD forms, "Save" and "Back" buttons for both forms are shown on the screen by default. 
This can cause some problems for users not knowing which "Save" or "Back" button to press.
You can avoid it hidding the parent CRUD form buttons, so the user has to save or close the child form before doing any action on the parent form.

In order to get this behavior you have to configure the following events for child grid:
- AfterCreateForm: call a function to hide the parent form buttons
- AfterReadForm: call a function to hide the parent form buttons
- AfterUpdateForm: call a function to hide the parent form buttons
- AfterDeleteForm: call a function to hide the parent form buttons
- AfterInsert: call a function to show the parent form buttons
- AfterUpdate: call a function to show the parent form buttons
- AfterDelete: call a function to show the parent form buttons
- AfterBack: call a function to show the parent form buttons

Events are explained in more detail in the [next section](Events.md)

You can hide or show CRUD form buttons using the ```ShowCrudButtons``` and ```HideCrudButtons``` methods of the parent ```GridComponent``` object.

This is an example implementing this feature:

```c#
<GridComponent @ref="_gridComponent" T="Order" Grid="@_grid"></GridComponent>

@code
{
    private GridComponent<Order> _gridComponent;
    private bool _areEventsLoaded = false;
    ...

    protected override async Task OnParametersSetAsync()
    {
        var locale = CultureInfo.CurrentCulture;

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
                    .WithGridItemsCount()
                    .AddToOnAfterRender(OnAfterOrderDetailRender);

            await subGridClient.UpdateGrid();
            return subGridClient.Grid;
        };

        var query = new QueryDictionary<StringValues>();
        string url = NavigationManager.BaseUri + "api/SampleData/OrderColumnsWithSubgridCrud";

        var client = new GridClient<Order>(HttpClient, url, query, false, "ordersGrid", c =>
            ColumnCollections.OrderColumnsWithNestedCrud(c, NavigationManager.BaseUri, subGrids), locale)
            .Sortable()
            .Filterable()
            .SetStriped(true)
            .Crud(true, orderService)
            .WithMultipleFilters()
            .WithGridItemsCount();

        _grid = client.Grid;

        // Set new items to grid
        _task = client.UpdateGrid();
        await _task;
    }

    private async Task OnAfterOrderDetailRender(GridComponent<OrderDetail> gridComponent, bool firstRender)
    {
        if (firstRender)
        {
            gridComponent.AfterInsert += AfterInsertOrderDetail;
            gridComponent.AfterUpdate += AfterUpdateOrderDetail;
            gridComponent.AfterDelete += AfterDeleteOrderDetail;
            gridComponent.AfterBack += AfterBack;

            gridComponent.AfterCreateForm += AfterFormOrderDetail;
            gridComponent.AfterReadForm += AfterFormOrderDetail;
            gridComponent.AfterUpdateForm += AfterFormOrderDetail;
            gridComponent.AfterDeleteForm += AfterFormOrderDetail;

            await Task.CompletedTask;
        }
    }

    private async Task AfterInsertOrderDetail(GridCreateComponent<OrderDetail> component, OrderDetail item)
    {
        _gridComponent.ShowCrudButtons();
        await Task.CompletedTask;
    }

    private async Task AfterUpdateOrderDetail(GridUpdateComponent<OrderDetail> component, OrderDetail item)
    {
        _gridComponent.ShowCrudButtons();
        await Task.CompletedTask;
    }

    private async Task AfterDeleteOrderDetail(GridDeleteComponent<OrderDetail> component, OrderDetail item)
    {
        _gridComponent.ShowCrudButtons();
        await Task.CompletedTask;
    }

    private async Task AfterBack(GridComponent<OrderDetail> component, OrderDetail item)
    {
        _gridComponent.ShowCrudButtons();
        await Task.CompletedTask;
    }

    private async Task AfterFormOrderDetail(GridComponent<OrderDetail> gridComponent, OrderDetail item)
    {
        _gridComponent.HideCrudButtons();
        await Task.CompletedTask;
    }
}
```


[<- CRUD](Crud.md) | [Events, exceptions and CRUD validation ->](Events.md)