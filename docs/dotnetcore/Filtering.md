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
    @await Html.Grid(Model).Columns(columns =>
    {
        columns.Add(foo => foo.Title);
        columns.Add(foo => foo.Description);
    }).Filterable().RenderAsync()
```

You can enable a button to clear all selected filters using the ***ClearFiltersButton*** method of the **GridClient** object:  

```c#
    var client = new GridServer<Order>(_orderRepository.GetAll(), query, false, "ordersGrid", columns, 10, locale)
        .ClearFiltersButton(true);
```

**GridMvc** supports several types of columns (specified in the **Add** method):

* System.String
* System.Guid
* System.Int32
* System.Int64
* System.Boolean
* System.DateTime
* System.DateTimeOffset
* System.DateOnly
* System.TimeOnly
* System.TimeSpan
* System.Decimal
* System.Byte
* System.Double
* System.Single
* enum

It also supports nullable types of any element of the list.

```System.DateOnly``` and ```System.TimeOnly``` need a target framework that has them, so they are
absent from ```netstandard2.1``` and ```net5.0```. ```System.TimeSpan``` is there in every one.
Without its own filter type a duration used to fall through to the text one and was compared as a
string, which reads plausibly and asks the wrong question.

**GridMvc** has different filter widgets for these types:
* **TextFitlerWidget**: it provides a filter interface for text columns (System.String). This means that if your column has text data, **GridMvcCore** will render an specific filter interface:

    ![](../images/Filtering_string.png)

* **NumberFilterWidget**: it provides a filter interface for number columns (System.Int32, System.Decimal etc.)

    ![](../images/Filtering_number.png)

* **BooleanFilterWidget**: it provides a filter interface for boolean columns (System.Boolean):

    ![](../images/Filtering_boolean.png)

* **DateTimeFilterWidget**: it provides a filter interface for datetime columns (System.DateTime):

    ![](../images/Filtering_datetime.png)

## Date format

The date filter is written the way the reader writes dates, and travels to the server in ISO 8601
(```yyyy-MM-dd```) whatever the reader's locale. The two are deliberately separate: the order of
day and month is exactly what changes between locales, and reading ```09/01``` as the ninth of
January when it is the first of September is a mistake nothing in the text reveals.

The pattern the box and its calendar use is decided in this order:

1. the ```format``` entry of the widget data, when the column sets one with
   ```SetFilterWidgetType```, in the datepicker's own spelling (```dd/mm/yyyy```);
2. otherwise the column's own format, when it sets one with ```Format``` that writes a date and
   nothing else - a format carrying a time is skipped here, because the filter asks for a day and
   the calendar it opens can only offer one;
3. otherwise the request's culture, with the day and the month padded to two digits.

The culture is resolved on the server rather than in the browser on purpose: the cells beside the
filter are rendered by the same request, and a filter reading a day differently from the column it
filters is worse than either choice on its own.

## Multiple filters

Pressing the **+** and **-** buttons you can add multiple options to filter. You can also select the condition you want to use, either **And** or **Or**:

![](../images/Filtering_multiple.png)

You can also create your own filter widgets.

[<- Searching](Searching.md) | [Using a list filter ->](Using_list_filter.md)