## GridMvc for ASP.NET Core MVC

# Render button, checkbox, etc. in a grid cell

[Index](Documentation.md)

You can use the **RenderValueAs** method to render a custom html markup in the grid cell. You must disable the default encoding and satinizing cell values using **Encoded** and **Sanitized** methods.

## Button

```c#
    columns.Add()
        .Encoded(false)
        .Sanitized(false)
        .SetWidth(30)
        .RenderValueAs(o => @<button type="submit">Submit</button>);
```

## Checkbox

```c#
    columns.Add()
        .Encoded(false)
        .Sanitized(false)
        .SetWidth(30)
        .RenderValueAs(o => Html.CheckBox("checked", false));
```

## Custom layout

You can render any custom layout using razor @helper:

```c#
    @helper CustomRenderingOfColumn(Order order)
    {
        if (order.Customer.IsVip)
        {
            <text>Yes</text>
        }
        else
        {
            <text>No</text>
        }
    }

    @await Html.Grid(Model).Columns(columns =>
    {
        columns.Add(o => o.Customer.IsVip)
                .Titled("Vip customer")
                .SetWidth(150)
                .RenderValueAs(o => CustomRenderingOfColumn(o));
    }).RenderAsync()
```

## ViewComponent

You can also use the **RenderComponentAs** method to render a custom view component in the grid cell:

```c#
    columns.Add().RenderComponentAs<ButtonCellViewComponent>(returnUrl);
```

**RenderComponentAs** method has 2 optional parameters:

Parameter | Type | Description
--------- | ---- | -----------
Actions | IList<Action<object>> (optional) | the grid view can pass a list of Actions to be used by the view component (see [Passing grid state as parameter](Passing_grid_state_as_parameter.md))
Object| object (optional) | the grid view can pass an object to be used by the view component (see [Passing grid state as parameter](Passing_grid_state_as_parameter.md))

If you use any of these paramenters, you must use them when creating the view component.

You must also create a viewcomponent. The InvokeAsync method includes a mandatory parameter called **Item**, and 3 optional parameters:

Parameter | Type | Description
--------- | ---- | -----------
Item | object (mandatory) | the row item that will be used by the view component
Grid | IGrid (optional) | Grid can be used to get the grid state (see [Passing grid state as parameter](Passing_grid_state_as_parameter.md))
Actions | IList<Action<object>> (optional) | the  parent component can pass a list of Actions to be used by the view component (see [Passing grid state as parameter](Passing_grid_state_as_parameter.md))
Object| object (optional) | the parent component can pass an object to be used by the view component (see [Passing grid state as parameter](Passing_grid_state_as_parameter.md))

**Actions** and **Object** must be used when calling the **RenderComponentAs** method, but **Grid** can be used without this requirement.
 
The view component can include any html elements as well as any event handling features.

An example of view component is:

```c#
    public class ButtonCellViewComponent : ViewComponent
    {

        public async Task<IViewComponentResult> InvokeAsync(object Item, IGrid Grid, object Object)
        {
            int orderId = ((Order)Item).OrderID;
            ViewData["gridState"] = Grid.GetState();
            ViewData["returnUrl"] = (string)Object;

            var factory = Task<IViewComponentResult>.Factory;
            return await factory.StartNew(() => View(orderId));
        }
```

And the view: 

```razor
    @model int

    @{
        string gridState = (string)ViewData["gridState"];
        string returnUrl = (string)ViewData["returnUrl"];
    }

    <b><a class='modal_link' href='/Home/Edit/@Model?returnUrl=@returnUrl&gridState=@gridState'>Edit</a></b>
```

[<- Data annotations](Data_annotations.md) | [Subgrids ->](Subgrids.md)