## GridBlazor for ASP.NET Core MVC

# Custom columns

[Index](Documentation.md)

All customization for columns must be done in the razor page:

* You can create a custom column by calling the **Columns.Add** method in the **IGridColumnCollection** interface. For example:

    ```c#
        Columns.Add(o => o.Customers.CompanyName);
    ```

* The **Titled** method of the **Column** object defines the column header text. If you don't call this method, **GridBlazor** will use the name of the property by default (in this case **CompanyName**).

    ```c#
        Columns.Add(o => o.Customers.CompanyName)
                .Titled("Company Name")
    ```

* If you want to add a column in a specified grid position, you can use the **Insert** method :

    ```c#
        Columns.Insert(0, o => o.Customers.CompanyName)
                .Titled("Company Name")
    ```

* You can construct a custom display value of the column using the **RenderValueAs** method:

    ```c#
        Columns.Add(o => o.Employees.LastName)
                .RenderValueAs(o => o.Employees.FirstName + " " + o.Employees.LastName)
    ```

* You can build **html** content in the column using the **RenderValueAs**, **Encoded** and **Sanitized** methods:

    ```c#
         c.Add(o => o.OrderID).Encoded(false).Sanitized(false)
        	.RenderValueAs(o => $"<b><a class='modal_link' href='/Home/Edit/{o.OrderID}'>Edit</a></b>");
    ```

* You can apply css classes to all cells of a column:

    ```c#
        Columns.Add(o => o.Employees.LastName).Css("hidden-xs")
    ```

* You can apply css classes to selected cells of a column:

    ```c#
        Columns.Add(o => o.OrderDate)
            .SetCellCssClassesContraint(o => o.OrderDate.HasValue && o.OrderDate.Value >= DateTime.Parse("1997-01-01") ? "red" : "")
    ```

* As a general rule you can concatenate method calls of the **Column** object to configure each column. For example:

    ```c#
        Columns.Add(o => o.Customers.CompanyName)
                .Titled("Company Name")
                .Sortable(true)
                .SetWidth(220);
    ```

## Collection columns

When there are tables with one-to-many relationship, each table can reference the other one on the model definition. In this case you can define a grid column that shows the count of the referenced collection. You should use the contructor of the **Add** method to create the column including the collection expression as parameter, as a normal column definition:

```c#
    Columns.Add(o => o.OrderDatails);
```

This column can be filtered, sorted and searched as any other one.

## Component columns

Sometimes you need to add a column that renders a component. In this case you must use an empty contructor of the **Add** method to create a column and its **RenderComponentAs** method to define the content that will be rendered in it:

```c#
    Columns.Add().RenderComponentAs(typeof(ButtonCell));
```

The parameter of the **RenderComponentAs** method must be the **Type** of the Blazor component used for this column.
You must also create a Blazor component that implements the **ICustomGridComponent** interface.
The component can include any html elements as well as event handling features.

This is an example of this type of component:

```razor
    @implements ICustomGridComponent<Order>
    
    <button class='btn btn-sm btn-primary' @onclick="@MyClickHandler">Save</button>

    @code {
        [Parameter]
        protected Order Item { get; set; }

        private void MyClickHandler(UIMouseEventArgs e)
        {
            Console.WriteLine("Button clicked: Item " + Item.OrderID);
        }
    }
```

Remember that sorting and filtering will not work on component columns.

## Not mapped columns

If you need to add a column that is not mapped to a database and the its property is configured with the **NotMapped** attribute, you must use the **SetNotDbMapped** method to configure it properly:

```c#
    Columns.Add(o => o.CalculatedTotal)
	    .Titled("Total")
        .SetNotDbMapped(true);
```
Remember that sorting, filtering and searching will not work on not mapped columns.

## Not connected columns

Sometimes you need to add a column that renders some content, but there is no base model property. In this case you must use an empty contructor of the **Add** method to create a column and its **RenderValueAs** method to define the content that will be rendered in it:

```c#
    Columns.Add().RenderValueAs(model => "Item " + model.Id);
```
Remember that sorting and filtering will not work on not connected columns.

## Hidden columns

All columns added to the grid are visible by default. If you want that a column will not appear on the screen you have to set it up in the **Add** method call:

```c#
    Columns.Add(o => o.Id, true);
```
The second parameter of the **Add** method defines if the column will be hidden or not. 
In this example you will not see the **Id** column, but you can get values at the client side using javascript. 
Important: you can't sort hidden columns.

## Sorting

If you want to enable sorting just for one column, you just call the **Sortable(true)** method of that column:

```c#
    Columns.Add(o => o.Employees.LastName)
                .Titled("Employee")
                .Sortable(true);
```

Sorting will be implemented for the field that you specify the **Add** method, in this example for the **o.Employees.LastName** field.

If you pass an ordered collection of items to the Grid constructor and you want to display this by default, you have to specify the initial sorting options:

```c#
    Columns.Add(o => o.OrderDate)
                .Titled("Date")
                .Sortable(true)
                .SortInitialDirection(GridSortDirection.Descending);
```

Remember that you can also enable [Sorting](Sorting.md) for all columns of a grid. Sorting at grid level has precendence over sorting defined at column level.

## Sorting with a custom comparer

You can enable sorting using a custom comparer. This feature only works for grids where all items are in memory. 
Grids where items are in a database and EF Core is used to get them don't support custom comparers and will throw an exception if the are configured in this way.

If you want to use a custom comparer for a column yo can do it by calling the **Columns.Add** method in the **IGridColumnCollection** interface using an ```IComparer<TKey>``` as a parameter:
```c#
    var comparer = new ....;

    .... 

    c.Add(o => o.Customer.CompanyName, comparer)
```

## Header tooltip

You can enable a tooltip for a column header. The tooltip will appear when the pointer hover on the column header of the grid. 

If the grid is configured for CRUD operations, the tooltip will also appear hovering over the field label of CRUD forms. 

If you want to use a tooltip for a column yo can do it by calling the **SetTooltip** method in the column definition:
```c#
    c.Add(o => o.OrderID).SetTooltip("Order ID is ... ");
```

## Column settings

Method name | Description | Example
------------- | ----------- | -------
Titled | Setup the column header text | Columns.Add(x=>x.Name).Titled("Name of product");
Encoded | Enable or disable encoding column values | Columns.Add(x=>x.Name).Encoded(false);
Sanitized | If encoding is disabled sanitize column value from XSS attacks | Columns.Add(x=>x.Name).Encoded(false).Sanitize(false);
SetWidth | Setup width of current column | Columns.Add(x=>x.Name).SetWidth("30%");
SetNotDbMapped | Configure the current column as not DB mapped | Columns.Add(x=>x.CalculatedTotal).SetNotDbMapped(true);
RenderComponentAs | Setup delegate to render a component column | Columns.Add().RenderComponentAs(typeof(ButtonCell));
RenderValueAs | Setup delegate to render column values | Columns.Add(x=>x.Name).RenderValueAs(o => o.Employees.FirstName + " " + o.Employees.LastName);
Sortable | Enable or disable sorting for current column | Columns.Add(x=>x.Name).Sortable(true);
SortInitialDirection | Setup the initial sort deirection of the column (need to enable sorting) | Columns.Add(x=>x.Name).Sortable(true).SortInitialDirection(GridSortDirection.Descending);
SetInitialFilter | Setup the initial filter of the column | Columns.Add(x=>x.Name).Filterable(true).SetInitialFilter(GridFilterType.Equals, "some name");
ThenSortBy | Setup ThenBy sorting of current column | Columns.Add(x=>x.Name).Sortable(true).ThenSortBy(x=>x.Date);
ThenSortByDescending | Setup ThenByDescending sorting of current column | Columns.Add(x=>x.Name).Sortable(true).ThenSortBy(x=>x.Date).ThenSortByDescending(x=>x.Description);
Filterable | Enable or disable filtering feauture on the column | Columns.Add(x=>x.Name).Filterable(true);
SetFilterWidgetType | Setup filter widget type for rendering custom filtering user interface | Columns.Add(x=>x.Name).Filterable(true).SetFilterWidgetType("MyFilter");
Format | Format column value | Columns.Add(o => o.OrderDate).Format("{0:dd/MM/yyyy}");
Css | Apply css classes to the column | Columns.Add(x => x.Number).Css("hidden-xs");
SetCellCssClassesContraint | Apply css classes to selected cells | Columns.Add(x => x.Number).SetCellCssClassesContraint(x => x.Number < 0 ? "red" : "black");
SetTooltip | Enable column header tooltip | Columns.Add(x => x.Number).SetTooltip("Number column is ... ");

[<- Paging](Paging.md) | [Totals ->](Totals.md)