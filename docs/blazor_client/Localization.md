## Blazor client-side

# Localization

[Index](Documentation.md)

GridBlazor 1.3.31 and newer versions support localization.

English is the default laguage. But you can use other languages. You have to create a **CultureInfo** on the razor page and pass it as parameter in the contructor of the **GridClient** object:
    
```c#
    var locale = new CultureInfo("de-DE");
    var client = new GridClient<Order>(HttpClient, url, query, false, "ordersGrid", ColumnCollections.OrderColumns, locale);
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
* Swedish
* Serbian
* Croatian
* Farsi
* Basque
* Catalan
* Galician

## Right to left direction
Those languages that require right to left direction are also supported. You must configure the grid to user RTL direction using the ```SetDirection``` method of the ```GridClient``` object:
    
```c#
    var client = new GridClient<Order>(HttpClient, url, query, false, "ordersGrid", ColumnCollections.OrderColumns, locale)
        .SetDirection(GridDirection.RTL);
```

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
* Code
* Code and confirmation code must be equal to delete this item
* Code and confirmation code must be equal to save this item
* Confirm Code
* Contains
* Create item
* There are no items to display
* Delete
* Delete item
* Edit
* Ends with
* Equals
* Drop columns here for column extended sorting
* Files
* Filter this column
* Type
* Value
* Greater than
* Greater than or equals
* Drop columns here for column grouping
* Is null
* Is not null
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
* You must select the row you want to delete	
* You must select the row you want to view	
* You must select the row you want to edit	

[<- Setup initial column filtering](Setup_initial_column_filtering.md) | [Data annotations ->](Data_annotations.md)