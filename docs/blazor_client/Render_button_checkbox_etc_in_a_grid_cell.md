## Blazor client-side

# Render button, checkbox, etc. in a grid cell

[Index](Documentation.md)

You can use the **RenderValueAs** method to render a custom html markup in the grid cell. You must disable the default encoding and satinizing cell values using **Encoded** and **Sanitized** methods.

## Button

```c#
    columns.Add()
        .Encoded(false)
        .Sanitized(false)
        .SetWidth(30)
        .RenderValueAs(o => $"<button type='submit'>Submit</button>");
```

## Checkbox

```c#
    columns.Add()
        .Encoded(false)
        .Sanitized(false)
        .SetWidth(30)
        .RenderValueAs(o => $"<input type='checkbox' />");
```

## Custom layout

```c#
    columns.Add()
        .Encoded(false)
        .Sanitized(false)
        .SetWidth(30)
        .RenderValueAs(o => $"<b><a class='modal_link' href='/Home/Edit/{o.OrderID}'>Edit</a></b>");
```

[<- Data annotations](Data_annotations.md)