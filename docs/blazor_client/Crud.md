## Blazor client-side

# CRUD

[Index](Documentation.md)

GridBlazor supports CRUD forms to add, edit, view and delete items for Blazor client-side projects.

These are the supported features:
- Full screen forms
- Auto-generated forms with field type detection based on column definition
- Lists for drop-drown fields
- Custom forms
- Support of grid models including 1:N relationships
- Support of entities with multiple foreign keys

## Auto-generated forms

You can enable CRUD using the **Crud** method of the **GridClient** object:
```c#   
    var client = new GridClient<Order>(url, query, false, "ordersGrid", c => 
            ColumnCollections.OrderColumnsWithCrud(c, NavigationManager.BaseUri), locale)
        .Crud(true, orderService)
```

**Note**: All 4 crud forms can be enabled at the same time with the **Crud(true, ...)** method, but you can enable one by one using  the **Crud(true, false, true, false, ...)** method.

The parameter **crudDataService** of the **Crud** method must be a class that implements the **ICrudDataService<T>** interface on the front-end project. 

This interface has 4 methods:
- ```Task<T> Get(params object[] keys);```
- ```Task Insert(T item);```
- ```Task Update(T item);```
- ```Task Delete(params object[] keys);```
one for each CRUD operation.

This is an example of those 4 methods:
```c#
    public class OrderService : ICrudDataService<Order>
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUri;

        public OrderService(HttpClient httpClient, NavigationManager navigationManager)
        {
            _httpClient = httpClient;
            _baseUri = navigationManager.BaseUri;
        }

        public async Task<Order> Get(params object[] keys)
        {
            int orderId;
            int.TryParse(keys[0].ToString(), out orderId);
            return await _httpClient.GetJsonAsync<Order>(_baseUri + $"api/Order/{orderId}");
        }

        public async Task Insert(Order item)
        {
            await _httpClient.PostJsonAsync(_baseUri + $"api/Order", item);
        }

        public async Task Update(Order item)
        {
            await _httpClient.PutJsonAsync(_baseUri + $"api/Order/{item.OrderID}", item);
        }

        public async Task Delete(params object[] keys)
        {
            int orderId;
            int.TryParse(keys[0].ToString(), out orderId);
            await _httpClient.SendJsonAsync(HttpMethod.Delete, _baseUri + $"api/Order/{orderId}", null);
        }
    }
```

You will need a controller supporting 4 web services to perform the CRUD operation on the back-end. This is an example of this type of controller:
  
```c#
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : Controller
    {
        private readonly NorthwindDbContext _context;

        public OrderController(NorthwindDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] Order order)
        {
            if (ModelState.IsValid)
            {
                if (order == null)
                {
                    return BadRequest();
                }

                var repository = new OrdersRepository(_context);
                try
                {
                    await repository.Insert(order);
                    repository.Save();

                    return NoContent();
                }
                catch (Exception e)
                {
                    return BadRequest(new
                    {
                        message = e.Message.Replace('{', '(').Replace('}', ')')
                    });
                }
            }
            return BadRequest(new
            {
                message = "ModelState is not valid"
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetOrder(int id)
        {
            var repository = new OrdersRepository(_context);
            Order order = await repository.GetById(id);
            if (order == null)
            {
                return NotFound();
            }
            return Ok(order);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateOrder(int id, [FromBody] Order order)
        {
            if (ModelState.IsValid)
            {
                if (order == null || order.OrderID != id)
                {
                    return BadRequest();
                }

                var repository = new OrdersRepository(_context);
                try
                {
                    await repository.Update(order);
                    repository.Save();

                    return NoContent();
                }
                catch (Exception e)
                {
                    return BadRequest(new
                    {
                        message = e.Message.Replace('{', '(').Replace('}', ')')
                    });
                }
            }
            return BadRequest(new
            {
                message = "ModelState is not valid"
            });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var repository = new OrdersRepository(_context);
            Order order = await repository.GetById(id);

            if (order == null)
            {
                return NotFound();
            }

            try
            {
                repository.Delete(order);
                repository.Save();

                return NoContent();
            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    message = e.Message.Replace('{', '(').Replace('}', ')')
                });
            }
        }
    }
```

### Column definition

The column definition must include the primary key using the **SetPrimaryKey(true)** method. 

If the grid model includes foreign keys, the column definition should include them using the **SetSelectField** in order to get the options for the ```<select>``` element.

The **SetSelectField** method has 3 required parameters:
Parameter | Description
--------- | -----------
enabled | boolean to configure if the field is shown as a ```<select>``` html element
expression | function to get the selected value (it must return an string value)
url | absolute path for the web service to get the values and titles to be shown in the drop-down element (it must return an ```IEnumerable<SelectItem>```)

This is an example of back-end web service to get values and titles for a drop-down:

```c#
    [HttpGet("[action]")]
    public ActionResult GetAllEmployees()
    {
        var repository = new EmployeeRepository(_context);
        return Ok(repository.GetAll()
            .Select(r => new SelectItem(r.EmployeeID.ToString(), r.EmployeeID.ToString() + " - "
                + r.FirstName + " " + r.LastName))
            .ToList());
    }
```

Other fields that you want to be shown as dropdowns with a closed list can also be configured with the **SetSelectField** method.

All fields to be included in the CRUD forms but not in the grid as columns should be configured as hidden (e.g. **Add(o => o.RequiredDate, true)**).

And finally all columns included in the grid but not in the CRUD forms should be configured as "CRUD hidden" using the **SetCrudHidden(true)** method.

This is an example of column definition:

```c#
    public static Action<IGridColumnCollection<Order>, string> OrderColumnsWithCrud = (c, path) =>
    {
        c.Add(o => o.OrderID).SetPrimaryKey(true);
        c.Add(o => o.CustomerID, true).SetSelectField(true, o => o.Customer.CustomerID + " - " 
            + o.Customer.CompanyName, path + $"api/SampleData/GetAllCustomers");
        c.Add(o => o.EmployeeID, true).SetSelectField(true, o => o.Employee.EmployeeID.ToString() + " - " 
            + o.Employee.FirstName + " " + o.Employee.LastName, path + $"api/SampleData/GetAllEmployees");
        c.Add(o => o.ShipVia, true).SetSelectField(true, o => o.Shipper == null ? "" : o.Shipper.ShipperID.ToString() 
            + " - " + o.Shipper.CompanyName, path + $"api/SampleData/GetAllShippers");
        c.Add(o => o.OrderDate, "OrderCustomDate").Titled(SharedResource.OrderCustomDate).Format("{0:yyyy-MM-dd}");
        c.Add(o => o.Customer.CompanyName).Titled(SharedResource.CompanyName).SetCrudHidden(true);
        c.Add(o => o.Customer.ContactName).Titled(SharedResource.ContactName).SetCrudHidden(true);
        c.Add(o => o.Freight).Titled(SharedResource.Freight).Format("{0:F}");
        c.Add(o => o.Customer.IsVip).Titled(SharedResource.IsVip).RenderValueAs(o => o.Customer.IsVip ? "Yes" : "No").SetCrudHidden(true);
        c.Add(o => o.RequiredDate, true).Format("{0:yyyy-MM-dd}");
        c.Add(o => o.ShippedDate, true).Format("{0:yyyy-MM-dd}");
        c.Add(o => o.ShipName, true);
        c.Add(o => o.ShipAddress, true);
        c.Add(o => o.ShipCity, true);
        c.Add(o => o.ShipPostalCode, true);
        c.Add(o => o.ShipRegion, true);
        c.Add(o => o.ShipCountry, true);
    };
```

This is an example of a grid using CRUD:

![](../images/Crud.png)

And this is an auto-genereated edit form:

![](../images/Crud_edit.png)

## Custom forms (Optional)

If you want to use custom forms you can enable them using the **SetCreateComponent**, **SetReadComponent**, **SetUpdateComponent** and **SetDeleteComponent**  methods of the **GridClient** object:

```c#
    var client = new GridClient<Order>(url, query, false, "ordersGrid", ColumnCollections.OrderColumnsWithCustomCrud, locale)
        .Crud(true, orderService)
        .SetCreateComponent<OrderCreateComponent>()
        .SetReadComponent<OrderReadComponent>()
        .SetUpdateComponent<OrderUpdateComponent>()
        .SetDeleteComponent<OrderDeleteComponent>();
```

You can define all custom forms or just some of them. If you don't define a custom form for one of the enabled operations an auto-generated form will be used instead.

And finally you will have to create a Blazor component for the custom form. This is an example of edit form:

```razor
@using GridBlazor
@using GridBlazor.Resources
@using GridBlazorClientSide.Shared.Models
@inherits GridUpdateComponentBase<Order>

<h1>@Strings.Add Order</h1>
<EditForm Model="@Item" OnValidSubmit="@UpdateItem">
    <DataAnnotationsValidator />
    <ValidationSummary />

    <div class="form-horizontal">
        <div class="form-group">
            <label for="OrderID" class="control-label col-md-2">OrderID: </label>
            <div class="col-md-5">
                <InputNumber id="OrderID" class="form-control" readonly="readonly" @bind-Value="Item.OrderID" />
            </div>
        </div>

        <div class="form-group">
            <label for="CustomerID" class="control-label col-md-2">Customer Id: </label>
            <div class="col-md-5">
                <InputText id="CustomerID" class="form-control" @bind-Value="Item.CustomerID" />
            </div>
        </div>

        <div class="form-group">
            <label for="EmployeeID" class="control-label col-md-2">Employee Id: </label>
            <div class="col-md-5">
                <InputNumber id="EmployeeID" class="form-control" @bind-Value="Item.EmployeeID" />
            </div>
        </div>

        <div class="form-group">
            <label for="ShipVia" class="control-label col-md-2">Ship Via: </label>
            <div class="col-md-5">
                <InputNumber id="ShipVia" class="form-control" @bind-Value="Item.ShipVia" />
            </div>
        </div>

        <div class="form-group">
            <label for="RequiredDate" class="control-label col-md-2">Required Date: </label>
            <div class="col-md-5">
                <InputDate id="RequiredDate" class="form-control" @bind-Value="Item.RequiredDate" />
            </div>
        </div>

        <div class="form-group">
            <label for="ShippedDate" class="control-label col-md-2">Shipped Date: </label>
            <div class="col-md-5">
                <InputDate id="ShippedDate" class="form-control" @bind-Value="Item.ShippedDate" />
            </div>
        </div>

        <div class="form-group">
            <label for="ShipName" class="control-label col-md-2">Ship Name: </label>
            <div class="col-md-5">
                <InputText id="ShipName" class="form-control" @bind-Value="Item.ShipName" />
            </div>
        </div>

        <div class="form-group">
            <label for="ShipAddress" class="control-label col-md-2">Ship Address: </label>
            <div class="col-md-5">
                <InputText id="ShipAddress" class="form-control" @bind-Value="Item.ShipAddress" />
            </div>
        </div>

        <div class="form-group">
            <label for="ShipCity" class="control-label col-md-2">Ship City: </label>
            <div class="col-md-5">
                <InputText id="ShipCity" class="form-control" @bind-Value="Item.ShipCity" />
            </div>
        </div>

        <div class="form-group">
            <label for="ShipPostalCode" class="control-label col-md-2">Ship Postal Code: </label>
            <div class="col-md-5">
                <InputText id="ShipPostalCode" class="form-control" @bind-Value="Item.ShipPostalCode" />
            </div>
        </div>

        <div class="form-group">
            <label for="ShipRegion" class="control-label col-md-2">Ship Region: </label>
            <div class="col-md-5">
                <InputText id="ShipRegion" class="form-control" @bind-Value="Item.ShipRegion" />
            </div>
        </div>

        <div class="form-group">
            <label for="ShipCountry" class="control-label col-md-2">Ship Country: </label>
            <div class="col-md-5">
                <InputText id="ShipCountry" class="form-control" @bind-Value="Item.ShipCountry" />
            </div>
        </div>

        <div class="form-group">
            <label for="Freight" class="control-label col-md-2">Freight: </label>
            <div class="col-md-5">
                <input id="Freight" name="Freight" class="form-control" @bind="Item.Freight" />
            </div>
        </div>

        <div class="form-group">
            <div class="col-md-5">
                <button type="submit" class="btn btn-primary btn-md">@Strings.Save</button>
                <button type="button" class="btn btn-primary btn-md" @onclick="BackButtonClicked">@Strings.Back</button>
            </div>
        </div>
    </div>
</EditForm>
``` 

**Note**: The Blazor component must be to inherited from the **GridUpdateComponentBase<T>** class.

If you want to use a drop-down list for a field you have to define it as it was for auto-generated forms.

[<- Front-end back-end API](API.md)