## Blazor WASM with OData back-end

# Passing grid state as parameter

[Index](Documentation.md)

You can get the current grid state any time.
Grid state contains the page number and all filter, searching and sorting information.

So you can pass it as a parameter to another page, for example it you want to edit a row of the grid.
Then you can pass it back to the page containing the grid, so the grid can be created in the same state as it was left.

Some of these examples are showed in the [GridBlazorOData project](https://github.com/gustavnavar/Grid.Blazor/tree/master/GridBlazorOData.Client)

## Get grid state and pass it to another page

* The easiest way to send the grid state to another page is creating a custom column with a button, as seen in [Render button, checkbox, etc. in a grid cell](Render_button_checkbox_etc_in_a_grid_cell.md):
    ```razor
        @page "/gridsample"
        @using GridBlazor
        @using GridShared
        @using GridShared.Utility
        @using Microsoft.Extensions.Primitives
        @inject NavigationManager NavigationManager
        @inject HttpClient HttpClient

        @if (_task.IsCompleted)
        {
            <GridComponent T="Order" Grid="@_grid"></GridComponent>
        }
        else
        {
            <p><em>Loading...</em></p>
        }

        @code
        {
            private CGrid<Order> _grid;
            private Task _task;

            protected override async Task OnParametersSetAsync()
            {
                Action<IGridColumnCollection<Order>> columns = c =>
                {
                    c.Add().Encoded(false).Sanitized(false).RenderComponentAs<ButtonCell>();
                    c.Add(o => o.OrderID);
                    c.Add(o => o.OrderDate, "OrderCustomDate").Format("{0:yyyy-MM-dd}");
                    c.Add(o => o.Customer.CompanyName);
                    c.Add(o => o.Customer.IsVip);
                };

                string url = NavigationManager.BaseUri() + "odata/OrderColumns";
                var client = new GridODataClient<Order>(HttpClient, url, query, false, "ordersGrid", columns, 10)
                _grid = client.Grid;

                // Set new items to grid
                _task = client.UpdateGrid();
                await _task;
            }
        }
    ```

    Then the **ButtonCell** has to implement the **ICustomGridComponent** interface.
The **GetState** method of the optional **Grid** parameter must be called to get the grid state:
    ```razor
        @using GridBlazor
        @using GridBlazorOData.Shared.Models
        @using GridShared.Columns
        @implements ICustomGridComponent<Order>
        @inject NavigationManager NavigationManager

        <button class='btn btn-sm btn-primary' @onclick="MyClickHandler">Edit</button>

        @code {
            [Parameter]
            public Order Item { get; protected set; }

            [Parameter]
            public CGrid<Order> Grid { get; protected set; }

            private void MyClickHandler(UIMouseEventArgs e)
            {
                string gridState = Grid.GetState();
                NavigationManager.NavigateTo($"/editorder/{Item.OrderID.ToString()}/gridsample/{gridState}");          
            }
        }
    ```

* In the case of a page containing more than one grid, the **ButtonCell** component must call a parent component function to pass more than one state.
The call of the **RenderComponentAs** method must contain a list of Actions including just one element, the method used to call the new page:
    ```razor
        ...

        @code
        {
            ...

            protected override async Task OnParametersSetAsync()
            {
                ...

                Action<IGridColumnCollection<Order>> oColumns = c =>
                {
                    c.Add().Encoded(false).Sanitized(false).RenderComponentAs<ButtonCell>(new List<Action<object>>() { MyAction });
                    c.Add(o => o.OrderID);
                    c.Add(o => o.OrderDate, "OrderCustomDate").Format("{0:yyyy-MM-dd}");
                    c.Add(o => o.Customer.CompanyName);
                    c.Add(o => o.Customer.IsVip);
                };

                var oClient = new GridODataClient<Order>(HttpClient, oUrl, query, false, "ordersGrid", oColumns, 10)
                _grid = oClient.Grid;

                // Set new items to grid
                _task2 = oClient.UpdateGrid();
                await _task2;
            }

            private void MyAction(object item)
            {
                string ordersGridState = _ordersGrid.GetState();
                string customersGridState = _customersGrid.GetState();
                NavigationManager.NavigateTo($"/editorder/{((Order)item).OrderID.ToString()}/multiplegrids/{ordersGridState}/{customersGridState}");
            }
        }
    ```

    Then the **ButtonCell** has to implement the **ICustomGridComponent** interface. And the **Actions** parameter must be defined:
    ```razor
        @using GridBlazor
        @using GridBlazorServerSide.Models
        @using GridShared.Columns
        @implements ICustomGridComponent<Order>

        <button class='btn btn-sm btn-primary' @onclick="MyClickHandler">Edit</button>

        @code {
            [Parameter]
            public Order Item { get; protected set; }

            [Parameter]
            public IList<Action<object>> Actions { get; protected set; }

            private void MyClickHandler(UIMouseEventArgs e)
            {
                Actions[0]?.Invoke(Item);      
            }
        }
    ```

## Initializing grid with a state passed from another page

The page containing the grid must get the grid state parameter from the url.
* If the grid state parameter is not null or empty you have to create a **QueryCollection** object from the parameter.
* If the grid state parameter is null or empty you must use a new **QueryDictionary<StringValues>**.

The static method **StringExtensions.GetQuery** will convert the string to a **Dictionary<string, StringValues>** object that will be used in the **GridClient** contructor:
```razor
    @page "/gridsample"
    @page "/gridsample/{GridState}"

    ...

    [Parameter]
    protected string GridState { get; set; }

    @code
    {
        ...

        protected override async Task OnParametersSetAsync()
        {
            ...
            var query = new QueryDictionary<StringValues>();
            if (!string.IsNullOrWhiteSpace(GridState))
            {
                try
                {
                    query = StringExtensions.GetQuery(GridState);
                }
                catch (Exception)
                {
                    // do nothing, GridState was not a valid state
                }
            }

            string url = NavigationManager.BaseUri() + "odata/Orders";
            var client = new GridODataClient<Order>(HttpClient, url, query, false, "ordersGrid", columns, 10, locale)
            ...
        }
    }
```

If the page contains more than one grid you have to use 2 parameters, one for each grid.

[<- Subgrids](Subgrids.md) | [Front-end back-end API ->](API.md)