## GridMvc for ASP.NET Core MVC

# Subgrids

[Index](Documentation.md)

You can enable the subgrids support for your grid. Subgrids allows to view records for those tables that have a 1 to N relationship with the parent table of the main grid.

![](../images/Subgrids.png)

We asume that you already configured the parent grid and it's working as expected as described in the [Quick start](Quick_start.md) section.

**IMPORTANT**: You must use the [Client side object model](Client_side_object_model.md) to enable subgrids

You can enable subgrids for the parent grid using the **SubGrid** method of the **SGrid** object on a view:

```razor
    @await Html.Grid(Model).Columns(columns =>
    {
        columns.Add(c => c.OrderID);
        columns.Add(c => c.Title);
        columns.Add(c => c.Date);
    }).Named("ordersGrid").SubGrid("OrderID").RenderAsync()
```

Or you can also use the **SubGrid** method of the **GridServer** object from an action controller:

```c#
    public ActionResult Index()
    {
        IQueryable<Foo> items = fooRepository.GetAll();
        Action<IGridColumnCollection<Foo>> columns = c =>
        {
            columns.Add(c => c.OrderID);
            columns.Add(c => c.Title);
            columns.Add(c => c.Date);
        };
        var server = new GridServer<Order>(items, Request.Query, false, "ordersGrid", columns)
            .SubGrid("OrderID");

        return View(server.Grid);
    }
```

## SubGrid parameter

Parameter | Type | Description | Example
--------- | ---- | ----------- | -------
keys | params string[] | variable number of strings with the names of required columns to find records for the subgrid | SubGrid(new string[] { "OrderID" })

The following script on the view will enable paging, sorting, filtering and subgrids

```javascript
    <script>
        $(function () {
            pageGrids.ordersGrid.ajaxify({
                getPagedData: "/Home/GetOrdersGridRows",
                getSubGridData: "/Home/GetOrderDetailsGrid"
            });
        });
    </script>
```

**getPagedData** must contain the name of the controller action to get rows for the main grid. 
And **getSubGridData** must contain the name of the controller action to get rows for the subgrids.

Both actions must implement http POST method because we are using the [Client side object model](Client_side_object_model.md).

Finally we have to create the new controller action to get the subgrid rows:

```c#
    [HttpPost]
    public ActionResult GetOrderDetailsGrid(int OrderId)
    {
        Action<IGridColumnCollection<OrderDetail>> columns = c =>
        {
            c.Add(o => o.OrderID);
            c.Add(o => o.ProductID);
            c.Add(o => o.Product.ProductName);
            c.Add(o => o.Quantity).Format("{0:F}");
            c.Add(o => o.UnitPrice).Format("{0:F}");
        };

        var requestCulture = HttpContext.Features.Get<IRequestCultureFeature>();
        var locale = requestCulture.RequestCulture.UICulture.TwoLetterISOLanguageName;
        var orderDetails = (new OrderDetailsRepository(_context)).GetForOrder(OrderId);

        var server = new GridServer<OrderDetail>(orderDetails, Request.Query,
            false, "orderDetailsGrid" + OrderId.ToString(), columns, 10, locale)
                .SetRowCssClasses(item => item.Quantity > 10 ? "success" : string.Empty)
                .Sortable()
                .Filterable()
                .WithMultipleFilters()
                .WithGridItemsCount();

        return PartialView("_SubGrid", server.Grid);
    }
```

It's important to note that:
- the parameters used for the action are the same than the **keys** array parameter used on the **SubGrid** method for tha parent grid. In this example is **OrderId**.
- the grid name parameter we use must be unique for each subgrid. In this example we use the name **"orderDetailsGrid" + OrderId.ToString()**.

This action returns a **PartialView** with the subgrid model and the name of the view we will use for it.

Finally we will create the view for subgrids. The easiest way is to use a tag helper:


```razor
    @using GridMvc
    @model ISGrid
    @addTagHelper *, GridMvc

    <grid model="@Model" />
```

[<- Render button, checkbox, etc. in a grid cell](Render_button_checkbox_etc_in_a_grid_cell.md) | [Passing grid state as parameter ->](Passing_grid_state_as_parameter.md)