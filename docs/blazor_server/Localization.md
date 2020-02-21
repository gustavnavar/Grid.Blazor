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
* Norwegian
* Turkish
* Czech
* Slovenian

## Additional languages

If you need to support other languages, please send me the translation of the following expressions and I will updete the component:
* Add
* And
* Apply
* Average
* Back
* No
* Yes
* Clear all filters
* Clear filter
* Contains
* Create item
* There are no items to display
* Delete
* Delete item
* Edit
* Ends with
* Equals
* Drop columns here for column extended sorting
* Filter this column
* Type
* Value
* Greater than
* Greater than or equals
* Drop columns here for column grouping
* Items
* Less than
* Less than or equals
* Max
* Min
* Not equals
* Or
* Read item
* Save
* Search for ...
* --- Select an item ---
* Show
* Starts with
* Sum
* Update item
* View
* There was an error creating the new item
* There was an error deleting this item
* There was an error updating this item

[<- Setup initial column filtering](Setup_initial_column_filtering.md) | [Data annotations ->](Data_annotations.md)