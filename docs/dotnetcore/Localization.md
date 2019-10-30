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

[<- Setup initial column filtering](Setup_initial_column_filtering.md) | [Client side object model ->](Client_side_object_model.md)