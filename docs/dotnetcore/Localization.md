## GridMvc for ASP.NET Core MVC

# Localization

[Index](Documentation.md)

For each additional language add the following lines to the **_Layout.cshtml** view or directly to the page to call the required style sheet and script files (**gridmvc-lang-xx.js** file is loaded from the **GridMvcCore** nuget package):

```html
    <script src="~/js/gridmvc-lang-de.js" type="text/javascript"></script>
    <script src="~/lib/bootstrap-datepicker/locales/bootstrap-datepicker.de.min.js" type="text/javascript"></script>
```
Then you have to call the **SetLanguage** function with the required language value when the grid is created in the view:

```razor
    @using GridMvc
    @model IEnumerable<Foo>

    @await Html.Grid(Model).Columns(columns =>
    {
        columns.Add(foo => foo.Title);
        columns.Add(foo => foo.Description);
    }).SetLanguage("de").RenderAsync()
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
* Sweden
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
Those languages that require right to left direction are also supported. You must configure the grid to user RTL direction using the ```SetDirection``` method of the ```GridServer``` object:
    
```c#
    var server = new GridServer<Order>(items, Request.Query, false, "ordersGrid", columns)
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
* current (actual)
* Delete
* Delete item
* Drop columns here for column extended sorting
* Drop columns here for column grouping
* Duplicated
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
* Not duplicated
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

[<- Setup initial column filtering](Setup_initial_column_filtering.md) | [Client side object model ->](Client_side_object_model.md)