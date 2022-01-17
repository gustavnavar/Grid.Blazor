## Blazor WASM with OData back-end

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

## Showing nested grids at the same time as inserting a row

You can configure CRUD to show nested grids and thier CRUD forms at the same time as inserting a new row with the Create form. 
It's another way to create rows for the nested subgrid in the same step as creating the parent row.

This way in more complicated than the previos one. 

First you must create a service for memory persistance of nested entities until the parent entity has been created. This service must implement ```IMemoryDataService<T>``` interface for the nested entities. This is a sample:

```c#
    public class OrderDetailMemoryService : IMemoryDataService<OrderDetail>
    {
        private readonly Action<IGridColumnCollection<OrderDetail>> _columns;
        private readonly IEnumerable<SelectItem> _products;

        public IList<OrderDetail> Items { get; private set; }

        public OrderDetailMemoryService(Action<IGridColumnCollection<OrderDetail>> columns,
            IEnumerable<SelectItem> products)
        {
            _columns = columns;
            _products = products;
            Items = new List<OrderDetail>();
        }

        public ItemsDTO<OrderDetail> GetGridRows(QueryDictionary<StringValues> query)
        {
            var server = new GridCoreServer<OrderDetail>(Items, query, true, "Grid", _columns)
                    .Sortable()
                    .WithPaging(10)
                    .Filterable()
                    .WithMultipleFilters()
                    .Searchable(true, false, false);

            // return items to displays
            var items = server.ItemsToDisplay;
            return items;
        }

        public async Task<OrderDetail> Get(params object[] keys)
        {
            int orderID;
            int productID;
            int.TryParse(keys[0].ToString(), out orderID);
            int.TryParse(keys[1].ToString(), out productID);
            var item = Items.SingleOrDefault(o => o.OrderID == orderID && o.ProductID == productID);
            return await Task.FromResult(item);
        }

        public async Task Insert(OrderDetail item)
        {
            var it = Items.SingleOrDefault(o => o.OrderID == item.OrderID && o.ProductID == item.ProductID);
            if (it == null)
            {
                item.Product = new Product();
                item.Product.ProductID = item.ProductID;
                item.Product.ProductName = _products.SingleOrDefault(r => r.Value == item.ProductID.ToString())?.Title;
                Items.Add(item);
                await Task.CompletedTask;
            }
        }

        public async Task Update(OrderDetail item)
        {
            var it = Items.SingleOrDefault(o => o.OrderID == item.OrderID && o.ProductID == item.ProductID);
            if (it != null)
            {
                Items.Remove(it);
                if (item.Product == null)
                    item.Product = new Product();
                item.Product.ProductID = item.ProductID;
                item.Product.ProductName = _products.SingleOrDefault(r => r.Value == item.ProductID.ToString())?.Title;
                Items.Add(item);
                await Task.CompletedTask;
            }
        }

        public async Task Delete(params object[] keys)
        {
            var item = await Get(keys);
            if (item != null)
            {
                Items.Remove(item);
            }
        }
    }
```

Then you have to define the subgrid that you want to show on the CRUD forms. The subgrid definition must include the service defined in the previous step that will be user when the parent entity is inserted. It manages the subgrids in client memory before the parent entity is inserted. It's configured in the constructor of the ```GridODataClient<T>``` class.

This is a sample:
```c#
    Func<object[], bool, bool, bool, bool, Task<IGrid>> subgrids = async (keys, create, read, update, delete) =>
    {
        var subGridQuery = new QueryDictionary<StringValues>();

        Action<IGridColumnCollection<OrderDetail>> subgridColumns = c =>
        {
            c.Add(o => o.OrderID, true).SetPrimaryKey(true).SetWidth(100);
            c.Add(o => o.ProductID).SetPrimaryKey(true).Titled("ProdId").SetWidth(100)
                .SetSelectField(true, o => o.Product.ProductID.ToString() + " - " + o.Product.ProductName, 
                    () => GetAllProducts(NavigationManager.BaseUri));
            c.Add(o => o.Product.ProductName).Titled("ProdName").SetCrudHidden(true).SetWidth(250);
            c.Add(o => o.Quantity).Titled("Quant").SetWidth(100).Format("{0:F}");
            c.Add(o => o.UnitPrice).Titled("Unit Price").SetWidth(100).Format("{0:F}");
            c.Add(o => o.Discount).Titled("Disc").SetWidth(100).Format("{0:F}");
        };
        
        string subgridUrl = NavigationManager.BaseUri + $"odata/Orders({keys[0].ToString()})/OrderDetails";

        var products = await GetAllProducts(NavigationManager.BaseUri);
        _orderDetailMemoryService = new OrderDetailMemoryService(subgridColumns, products);

        var subgridClient = new GridODataClient<OrderDetail>(HttpClient, subgridUrl, _orderDetailMemoryService, subGridQuery, 
            false, "orderDetailsGrid" + keys[0].ToString(), subgridColumns, 10, locale)
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

Then the parent column definition must include a ```true``` value for the ```showCreateSubGrids``` parameter of the ```SubGrid``` method.  This is a sample:
```c#
    c.Add(o => o.OrderDetails).Titled("Order Details").SubGrid(true, "tabGroup1", subgrids, ("OrderID", "OrderID"));
```

And finally you have to configure the following events for parent grid to automatically insert the child entities after the parent has been inserted. The child entities can be read from the ```MemoryDataService<T>``` service. This is a sample: 
```c#
    protected override void OnAfterRender(bool firstRender)
    {
        if (_gridComponent != null && !_areEventsLoaded)
        {
            _gridComponent.AfterInsert += AfterInsert;
            _areEventsLoaded = true;
        }
    }

    private async Task<bool> AfterInsert(GridCreateComponent<Order> component, Order item)
    {
        foreach (var orderDetail in _orderDetailMemoryService.Items)
        {
            orderDetail.OrderID = item.OrderID;
            orderDetail.Product = null;
            var response = await HttpClient.PostAsJsonAsync<OrderDetail>(NavigationManager.BaseUri 
                + $"odata/OrderDetails", orderDetail, new JsonSerializerOptions().AddOdataSupport());
            if (!response.IsSuccessStatusCode)
            {
                return await Task.FromResult(false);
            }
        }
        return await Task.FromResult(true);
    }
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

            Action<IGridColumnCollection<OrderDetail>, string> subgridColumns = (c, baseUri) =>
            {
                c.Add(o => o.OrderID, true).SetPrimaryKey(true).SetWidth(100);
                c.Add(o => o.ProductID).SetPrimaryKey(true).Titled("ProdId").SetWidth(100).SetSelectField(true, o => o.Product.ProductID.ToString() + " - " + o.Product.ProductName, () => GetAllProducts(baseUri));
                c.Add(o => o.Product.ProductName).Titled("ProdName").SetCrudHidden(true).SetWidth(250);
                c.Add(o => o.Quantity).Titled("Quant").SetWidth(100).Format("{0:F}");
                c.Add(o => o.UnitPrice).Titled("Unit Price").SetWidth(100).Format("{0:F}");
                c.Add(o => o.Discount).Titled("Disc").SetWidth(100).Format("{0:F}");
            };

            string subgridUrl = NavigationManager.BaseUri + $"odata/Orders({keys[0].ToString()})/OrderDetails";

            var subgridClient = new GridODataClient<OrderDetail>(HttpClient, subgridUrl, subGridQuery, false,
                "orderDetailsGrid" + keys[0].ToString(), c => subgridColumns(c, NavigationManager.BaseUri), 10, locale)
                    .Sortable()
                    .Filterable()
                    .SetStriped(true)
                    .ODataCrud(create, read, update, delete)
                    .WithMultipleFilters()
                    .WithGridItemsCount()
                    .AddToOnAfterRender(OnAfterOrderDetailRender)

            await subGridClient.UpdateGrid();
            return subGridClient.Grid;
        };

        var query = new QueryDictionary<StringValues>

        Action<IGridColumnCollection<Order>, string> columns = (c, baseUri) =>
        {
            c.Add(o => o.OrderID).SetPrimaryKey(true).Titled(SharedResource.Number).SetTooltip("Order ID is ... ").SetWidth(100);
            c.Add(o => o.CustomerID, true).SetSelectField(true, o => o.Customer.CustomerID + " - " + o.Customer.CompanyName, () => GetAllCustomers(baseUri));
            c.Add(o => o.EmployeeID, true).SetSelectField(true, o => o.Employee.EmployeeID.ToString() + " - " + o.Employee.FirstName + " " + o.Employee.LastName, () => GetAllEmployees(baseUri));
            c.Add(o => o.ShipVia, true).SetSelectField(true, o => o.Shipper == null ? "" : o.Shipper.ShipperID.ToString() + " - " + o.Shipper.CompanyName, () => GetAllShippers(baseUri));
            c.Add(o => o.OrderDate, "OrderCustomDate").Titled(SharedResource.OrderCustomDate).SetInputType(InputType.Month).SetFilterWidgetType("Month").Format("{0:yyyy-MM}").SetWidth(120);
            c.Add(o => o.Customer.CompanyName).Titled(SharedResource.CompanyName).SetWidth(250).SetCrudHidden(true).SetReadOnlyOnUpdate(true);
            c.Add(o => o.Customer.ContactName).Titled(SharedResource.ContactName).SetCrudHidden(true);
            c.Add(o => o.Freight).Titled(SharedResource.Freight).SetWidth(150).Format("{0:F}");
            c.Add(o => o.Customer.IsVip).Titled(SharedResource.IsVip).SetWidth(90).Css("hidden-xs").RenderValueAs(o => o.Customer.IsVip ? Strings.BoolTrueLabel : Strings.BoolFalseLabel).SetCrudHidden(true);
            c.Add(o => o.RequiredDate, true).Format("{0:yyyy-MM-dd}");
            c.Add(o => o.ShippedDate, true).Format("{0:yyyy-MM-dd}");
            c.Add(o => o.ShipName, true);
            c.Add(o => o.ShipAddress, true);
            c.Add(o => o.ShipCity, true);
            c.Add(o => o.ShipPostalCode, true);
            c.Add(o => o.ShipRegion, true);
            c.Add(o => o.ShipCountry, true);
            c.Add(o => o.OrderDetails).Titled("Order Details").SubGrid(subgrids, ("OrderID", "OrderID"));
        };
		
        string url = NavigationManager.BaseUri + "odata/Orders";

        var client = new GridODataClient<Order>(HttpClient, url, query, false, "ordersGrid",
            c => columns(c, NavigationManager.BaseUri), 10, locale, new List<string> { "Employee", "Shipper" })
                .Sortable()
                .Filterable()
                .SetStriped(true)
                .ODataCrud(true)
                .WithMultipleFilters()
                .WithGridItemsCount();
        
        _grid = client.Grid;

        // Set new items to grid
        _task = client.UpdateGrid();
        await _task;
    }

    Func<string, Task<IEnumerable<SelectItem>>> GetAllCustomers = async (baseUri) =>
    {
        string url = baseUri + $"odata/Customers?$select=CustomerID,CompanyName";
        ODataDTO<Customer> response = await new HttpClient().GetFromJsonAsync<ODataDTO<Customer>>(url);
        if (response == null || response.Value == null)
        {
            return new List<SelectItem>();
        }
        else
        {
            return response.Value
                .Select(r => new SelectItem(r.CustomerID, r.CustomerID + " - " + r.CompanyName))
                .ToList();
        }
    };

    Func<string, Task<IEnumerable<SelectItem>>> GetAllEmployees = async (baseUri) =>
    {
        string url = baseUri + $"odata/Employees?$select=EmployeeID,FirstName,LastName";
        ODataDTO<Employee> response = await new HttpClient().GetFromJsonAsync<ODataDTO<Employee>>(url);
        if (response == null || response.Value == null)
        {
            return new List<SelectItem>();
        }
        else
        {
            return response.Value
                .Select(r => new SelectItem(r.EmployeeID.ToString(), r.EmployeeID.ToString() + " - " + r.FirstName + " " + r.LastName))
                .ToList();
        }
    };

    Func<string, Task<IEnumerable<SelectItem>>> GetAllShippers = async (baseUri) =>
    {
        string url = baseUri + $"odata/Shippers?$select=ShipperID,CompanyName";
        ODataDTO<Shipper> response = await new HttpClient().GetFromJsonAsync<ODataDTO<Shipper>>(url);
        if (response == null || response.Value == null)
        {
            return new List<SelectItem>();
        }
        else
        {
            return response.Value
                .Select(r => new SelectItem(r.ShipperID.ToString(), r.ShipperID.ToString() + " - " + r.CompanyName))
                .ToList();
        }
    };

    Func<string, Task<IEnumerable<SelectItem>>> GetAllProducts = async (baseUri) =>
    {
        string url = baseUri + $"odata/Products?$select=ProductID,ProductName";
        ODataDTO<Product> response = await new HttpClient().GetFromJsonAsync<ODataDTO<Product>>(url);
        if (response == null || response.Value == null)
        {
            return new List<SelectItem>();
        }
        else
        {
            return response.Value
                .Select(r => new SelectItem(r.ProductID.ToString(), r.ProductID.ToString() + " - " + r.ProductName))
                .ToList();
        }
    };

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