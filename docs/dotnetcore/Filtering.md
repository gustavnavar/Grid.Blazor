## GridMvc for ASP.NET Core MVC

# Filtering

[Index](Documentation.md)

You can enable the filtering option for your columns. To enable this functionality use the **Filterable** method of the **Column** object:

```c#
    Columns.Add(o => o.Customers.CompanyName)
        .Titled("Company Name")
        .Filterable(true)
        .Width(220)
```
After that you can filter this column. 

Remember that you can also enable filtering for all columns of a grid using the **Filterable** method of the **SGrid** object:
```razor
    @Html.Grid(Model, viewEngine).Columns(columns =>
    {
        columns.Add(foo => foo.Title);
        columns.Add(foo => foo.Description);
    }).Filterable()
```

GridMvc supports several types of columns (specified in the **Add** method):

* System.String
* System.Int32
* System.Int64
* System.Boolean
* System.DateTime
* System.Decimal
* System.Byte
* System.Double
* System.Single

It also supports nullable types of any element of the list.

**GridMvc** has different filter widgets for these types:
* **TextFitlerWidget**: it provides a filter interface for text columns (System.String). This means that if your column has text data, **Grid.Mvc** will render an specific filter interface:

    ![](../images/Filtering_string.png)

* **NumberFilterWidget**: it provides a filter interface for number columns (System.Int32, System.Decimal etc.)

    ![](../images/Filtering_number.png)

* **BooleanFilterWidget**: it provides a filter interface for boolean columns (System.Boolean):

    ![](../images/Filtering_boolean.png)

* **DateTimeFilterWidget**: it provides a filter interface for datetime columns (System.DateTime):

    ![](../images/Filtering_datetime.png)

You can also create your own filter widgets.

[<- Custom columns](Custom_columns.md) | [Creating custom filter widget ->](Creating_custom_filter_widget.md)