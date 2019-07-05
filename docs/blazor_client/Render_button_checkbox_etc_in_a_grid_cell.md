## Blazor client-side

# Render button, checkbox, etc. in a grid cell

[Index](Documentation.md)

The prefered method is using a Blazor component because it allows event handling with Blazor.
But you can also use the **RenderValueAs** method to render a custom html markup in the grid cell, as it is used on ASP.NET MVC Core projects. In this case events will be managed using Javascript.

You have to use the **RenderComponentAs** method to render a component in a cell:

```c#
    columns.Add().RenderComponentAs<ButtonCell>();
```

The generic type used has to be the component created to render the cell.

You must also create a Blazor component that implements the **ICustomGridComponent** interface.
This interface includes only a parameter called **Item** of the same type of the grid row element. 
The component can include any html elements as well as any event handling features.

## Button

In this sample we name the component **ButtonCell.razor**:

```razor
    @using GridShared.Columns
    @implements ICustomGridComponent<Order>

    <button class='btn btn-sm btn-primary' @onclick="@MyClickHandler">Save</button>

    @code {
        [Parameter]
        public Order Item { get; protected set; }

        private void MyClickHandler(UIMouseEventArgs e)
        {
            Console.WriteLine("Button clicked: Item " + Item.OrderID);
        }
    }
```

## Checkbox

```razor
    @using GridShared.Columns
    @implements ICustomGridComponent<Order>

    <input type='checkbox' @onchange="@CheckChanged" />

    @code {
        [Parameter]
        public Order Item { get; protected set; }

        private void CheckChanged()
        {
            Console.WriteLine("Check changed: Item " + Item.OrderID);
        }
    }
```

## Custom layout

```razor
    @using GridShared.Columns
    @implements ICustomGridComponent<Order>

    <b><a class='modal_link' href='#' @onclick="@MyClickHandler">Edit</a></b>

    @code {
        [Parameter]
        public Order Item { get; protected set; }

        private void MyClickHandler(UIMouseEventArgs e)
        {
            Console.WriteLine("Button clicked: /Home/Edit/" + Item.OrderID);
        }
    }
```

[<- Data annotations](Data_annotations.md) | [Subgrids ->](Subgrids.md)