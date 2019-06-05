## Blazor server-side

# Localization

[Index](Documentation.md)

English is the default laguage. But you can use other languages. You have to create a **CultureInfo** on the razor page and pass it as parameter in the contructor of the **GridClient** object:
    
```c#
    var locale = new CultureInfo("de-DE");
    var client = new GridClient<Order>(q => orderService.GetOrdersGridRows(columns, q), query, false, "ordersGrid", Columns, locale);
```

## Supported languages

* English (default)
* German
* French
* Italian
* Russian
* Spanish

## Additional languages

If you need to support other languages, please send me the translation of the following expressions and I will updete the component:
* There are no items to display
* Filter this column
* Type
* Value
* Apply
* Clear filter
* Equals
* Starts with
* Contains
* Ends with
* Greater than
* Less than
* Greater than or equals
* Less than or equals
* Yes
* No
* Items
* Search for ...
* Sum
* Average
* Max
* Min

[<- Setup initial column filtering](Setup_initial_column_filtering.md) | [Data annotations ->](Data_annotations.md)