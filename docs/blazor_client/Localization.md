## Blazor WASM with GridCore back-end (REST API)

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
* Brazilian Portuguese
* Bulgarian
* Ukrainian
* Arabic
* Danish

## Right to left direction
Those languages that require right to left direction are also supported. You must configure the grid to user RTL direction using the ```SetDirection``` method of the ```GridClient``` object:
    
```c#
    var client = new GridClient<Order>(HttpClient, url, query, false, "ordersGrid", ColumnCollections.OrderColumns, locale)
        .SetDirection(GridDirection.RTL);
```

## Additional languages

If you need to support other languages, please send me the translation of the following expressions and I will updete the component:
* --- Select an item ---
* Add
* All
* And
* Apply
* Average
* Back
* Clear all filters
* Clear filter
* Code
* Code and confirmation code must be equal to delete this item
* Code and confirmation code must be equal to save this item
* Confirm Code
* Contains
* Create item
* Delete
* Delete item
* Drop columns here for column extended sorting
* Drop columns here for column grouping
* Edit
* Ends with
* Equals
* Files
* Filter this column
* Go to
* Greater than
* Greater than or equals
* Is not null
* Is null
* Items
* Less than
* Less than or equals
* Max
* Min
* No
* None
* Not equals
* Or
* Read item
* Save
* Search for ...
* Search phrase
* Select
* Show
* Starts with
* Sum
* There are no items to display
* There was an error creating the new item
* There was an error deleting this item
* There was an error updating this item
* Type
* Update item
* Value
* View
* Yes
* You must select the row you want to delete
* You must select the row you want to edit
* You must select the row you want to view
	

[<- Setup initial column filtering](Setup_initial_column_filtering.md) | [Data annotations ->](Data_annotations.md)