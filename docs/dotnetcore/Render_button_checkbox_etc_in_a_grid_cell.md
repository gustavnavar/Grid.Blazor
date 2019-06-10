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

    @Html.Grid(Model).Columns(columns =>
    {
        columns.Add(o => o.Customer.IsVip)
                .Titled("Vip customer")
                .SetWidth(150)
                .RenderValueAs(o => CustomRenderingOfColumn(o));
    })
```

[<- Data annotations](Data_annotations.md)