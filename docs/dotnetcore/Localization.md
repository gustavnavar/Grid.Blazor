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

[<- Setup initial column filtering](Setup_initial_column_filtering.md) | [Client side object model ->](Client_side_object_model.md)