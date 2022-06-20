## Blazor WASM with GridCore back-end (REST API)

# Render button, checkbox, etc. in a grid cell

[Index](Documentation.md)

The prefered method is using a Blazor component because it allows event handling with Blazor.
But you can also use the **RenderValueAs** method to render a custom html markup in the grid cell, as it is used on ASP.NET MVC Core projects.
In this case events will be managed using Javascript.

You have to use the **RenderComponentAs** method to render a component in a cell:

```c#
    columns.Add().RenderComponentAs<ButtonCell>();
```

**RenderComponentAs** method has 3 optional parameters:

Parameter | Type | Description
--------- | ---- | -----------
Actions | IList<Action<object>> (optional) | the parent component can pass a list of Actions to be used by the component (see [Passing grid state as parameter](Passing_grid_state_as_parameter.md))
Functions | IList<Func<object,Task>> (optional) | the parent component can pass a list of Functions to be used by the child component
Object | object (optional) | the parent component can pass an object to be used by the component (see [Passing grid state as parameter](Passing_grid_state_as_parameter.md))

If you use any of these paramenters, you must use them when creating the component.

The generic type used has to be the component created to render the cell.

You must also create a Blazor component that implements the **ICustomGridComponent** interface.
This interface includes a mandatory parameter called **Item** of the same type of the grid row element, and 4 optional parameters:

Parameter | Type | Description
--------- | ---- | -----------
Item | row element (mandatory) | the row item that will be used by the component
GridComponent  | GridComponent<T> (CascadingParameter optional) | Parent Grid component
Grid | CGrid<T> (optional) | Grid can be used to get the grid state (see [Passing grid state as parameter](Passing_grid_state_as_parameter.md))
Actions | IList<Action<object>> (optional) | the parent component can pass a list of Actions to be used by the component (see [Passing grid state as parameter](Passing_grid_state_as_parameter.md))
Functions | IList<Func<object,Task>> (optional) | the parent component can pass a list of Functions to be used by the child component
Object | object (optional) | the parent component can pass an object to be used by the component (see [Passing grid state as parameter](Passing_grid_state_as_parameter.md))

**Actions**, **Functions** and **Object** must be used when calling the **RenderComponentAs** method, but **Grid** can be used without this requirement.
 
The component can include any html elements as well as any event handling features.

## RenderCrudComponentAs

**RenderCrudComponentAs** is a variant of **RenderComponentAs** to be used on grids with CRUD forms. The main difference is:
- Columns defined with **RenderCrudComponentAs** are visible on CRUD forms and on grids
- Columns defined with **RenderComponentAs** are visible on grids, but not visible on CRUD forms. 

You must configure columns created with **RenderCrudComponentAs** as Hidden if you want to show them just on CRUD forms:

``` razor
    c.Add(true).RenderCrudComponentAs<Carousel>();
```

And finally all columns included in the grid but not in the CRUD forms should be configured as "CRUD hidden" using the ```SetCrudHidden(true)``` method.

**Notes**: 
- You can have more granularity in the "CRUD hidden" configuration. You can use the ```SetCrudHidden(bool create, bool read, bool update, bool delete)``` method to configure the columns that will be hidden on each type of form.
- You can have more granularity in the components configuration.  You can use the ```RenderCrudComponentAs<TCreateComponent, TReadComponent, TUpdateComponent, TDeleteComponent>``` method to configure the components that will be shown on each type of form. Id you don't want to show any component for a specific type of form you must use ```NullComponent```


## Button

In this sample we name the component **ButtonCell.razor**:

```razor
    @implements ICustomGridComponent<Order>
    @inject NavigationManager NavigationManager

    <button class='btn btn-sm btn-primary' @onclick="MyClickHandler">Edit</button>

    @code {
        [CascadingParameter(Name = "GridComponent")]
        public GridComponent<Order> GridComponent { get; set; }
        
        [Parameter]
        public Order Item { get; protected set; }

        [Parameter]
        public IList<Func<object, Task>> Functions { get; protected set; }

        [Parameter]
        public CGrid<Order> Grid { get; protected set; }

        [Parameter]
        public object Object { get; protected set; }

        private async Task MyClickHandler(UIMouseEventArgs e)
        {
            if (Functions == null)
            {
                string gridState = Grid.GetState();
                if (Object == null)
                {
                    await Task.Run(() => 
                        NavigationManager.NavigateTo($"/editorder/{Item.OrderID.ToString()}/gridsample/{gridState}"));
                }
                else
                {
                    string returnUrl = (string)Object;
                    await Task.Run(() => 
                        NavigationManager.NavigateTo($"/editorder/{Item.OrderID.ToString()}/{returnUrl}/{gridState}"));
                }
            }
            else
            {
                await Functions[0]?.Invoke(Item);
            }
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