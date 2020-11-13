## GridMvc for ASP.NET Core MVC

# Passing grid state as parameter

[Index](Documentation.md)

You can get the current grid state any time.
Grid state contains the page number and all filter, searching and sorting information.

So you can pass it as a parameter to another view, for example it you want to edit a row of the grid.
Then you can pass it back to the view containing the grid, so the grid can be created in the same state as it was left.

Some of these examples are showed in the [GridMvc.Demo project](https://github.com/gustavnavar/Grid.Blazor/tree/master/GridMvc.Demo)

## Get grid state and pass it to another view

* The easiest way to send the grid state to another view is creating a custom column including a link to the new view.
    **IGridColumnCollection** has a property to get the **SGrid** object from wich you can call the **GetState** method:
    ```razor
        public class HomeController : Controller
        {
            ...
        
            public ActionResult Index()
            {
                Action<IGridColumnCollection<OrderDetail>> columns = c =>
                {
                    c.Add().Encoded(false).Sanitized(false)
                        .RenderValueAs(o => $"<b><a href='/Home/Edit/{o.OrderID}?gridState={c.Grid.GetState()}'>Edit</a></b>");
                    c.Add(o => o.OrderID);
                    c.Add(o => o.Title);
                    c.Add(o => o.Date);
                };
                ...
            }
            ...
        }
    
    ```

* If you use the [Client side object model](Client_side_object_model.md) you can create a custom column including a button calling a javascript function:
    ```razor
        public class HomeController : Controller
        {
            ...
        
            public ActionResult Index()
            {
                Action<IGridColumnCollection<OrderDetail>> columns = c =>
                {
                    c.Add().Encoded(false).Sanitized(false)
                        .RenderValueAs(o => $"<button type='button' onclick='editOrder({o.OrderID});'>Edit</button>");
                    c.Add(o => o.OrderID);
                    c.Add(o => o.Title);
                    c.Add(o => o.Date);
                };
                ...
            }
            ...
        }
    
    ```

    The javascript function must call the **getState()** function of the javascript **Grid** object to get the grid state and pass it as a parameter to the new view:
    ```javascript
        function editOrder(id) {
            var ordersGridState = pageGrids.ordersGrid.getState();
            window.location.replace("/Home/Edit/" + id + "?gridState=" + ordersGridState);
        }
    ```

* In the case of a view containing more than one grid, you must use the [Client side object model](Client_side_object_model.md) to pass more than one state.
In this case the javascript can be:
    ```javascript
        function editOrder(id) {
            var ordersGridState = pageGrids.ordersGrid.getState();
            var customersGridState = pageGrids.customersGrid.getState();
            window.location.replace("/Home/Edit/" + id + "?ordersGridState=" + ordersGridState + "&customersGridState=" + customersGridState );
        }
    ```

## Initializing grid with a state passed from another view

In all cases the controller must get the string grid state parameter and if it's not null or empty create a **QueryCollection** object.
If the grid state parameter is null or empty you must continue using the query from the **Request** object.

* If the **GridServer** constructor is used to create the grid, the static method **StringExtensions.GetQuery** will convert the string to a **Dictionary<string, StringValues>** object. 
Then it's possible to create a **QueryCollection** object that will be used in the **GridServer** contructor:
    ```c#
        public class HomeController : Controller
        {
            ...
        
            public ActionResult Index(string gridState = "")
            {
                Action<IGridColumnCollection<OrderDetail>> columns = c =>
                {
                    c.Add().Encoded(false).Sanitized(false)
                        .RenderValueAs(o => $"<button type='button' onclick='editOrder({o.OrderID});'>Edit</button>");
                    c.Add(o => o.OrderID);
                    c.Add(o => o.Title);
                    c.Add(o => o.Date);
                };

                IQueryCollection query = Request.Query;
                if (!string.IsNullOrWhiteSpace(gridState))
                {
                    try
                    {
                        query = new QueryCollection(StringExtensions.GetQuery(gridState));
                    }
                    catch (Exception)
                    {
                        // do nothing, gridState was not a valid state
                    }
                }

                var server = new GridServer<Order>(_orderRepository.GetAll(), query, false, "ordersGrid", columns, 10);

                return View(server.Grid);
            }
            ...
        }
     ```

* If you use the **SGrid** constructor to create the grid, the static method **StringExtensions.GetQuery** will convert the string to a **Dictionary<string, StringValues>** object. 
Then it's possible to create a **QueryCollection** object that will be used in the **SGrid** contructor:
    ```c#
        public class HomeController : Controller
        {
            ...
        
            public ActionResult Index(string gridState = "")
            {
                IQueryCollection query = Request.Query;
                if (!string.IsNullOrWhiteSpace(gridState))
                {
                    try
                    {
                        query = new QueryCollection(StringExtensions.GetQuery(gridState));
                    }
                    catch (Exception)
                    {
                        // do nothing, gridState was not a valid state
                    }
                }

                var grid = new SGrid<Order>(_orderRepository.GetAll(), query);

                return View(grid);
            }
            ...
        }
     ```

* If you use the **HtmlHelper** to create the grid in the view, the static method **StringExtensions.GetQuery** will convert the string to a **Dictionary<string, StringValues>** object. 
Then it's possible to create a **QueryCollection** object and pass it to the view using **ViewData** object:
    ```c#
        public class HomeController : Controller
        {
            ...
        
            public ActionResult Index(string gridState = "")
            {
                IQueryCollection query = Request.Query;
                if (!string.IsNullOrWhiteSpace(gridState))
                {
                    try
                    {
                        query = new QueryCollection(StringExtensions.GetQuery(gridState));
                    }
                    catch (Exception)
                    {
                        // do nothing, gridState was not a valid state
                    }
                }
                ViewData["query"] = query;

                return View(_orderRepository.GetAll());
            }
            ...
        }
    ```

    The view should get the query and use it in the **HtmlHelper** contructor: 
    ```razor
        @model IEnumerable<Order>

        @{
            Action<IGridColumnCollection<OrderDetail>> columns = c =>
            {
                c.Add().Encoded(false).Sanitized(false)
                    .RenderValueAs(o => $"<b><a href='/Home/Edit/{o.OrderID}?gridState={c.Grid.GetState()}'>Edit</a></b>");
                c.Add(o => o.OrderID);
                c.Add(o => o.Title);
                c.Add(o => o.Date);
            };
            var query = (IQueryCollection)ViewData["query"];
        }

        @await Html.Grid(Model, query).Named("ordersGrid").Columns(columns).RenderAsync()
     ```

[<- Subgrids](Subgrids.md) | [Grid dimensions ->](Grid_dimensions.md)