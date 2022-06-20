## Blazor WASM with GridCore back-end (REST API)

# GridBlazor configuration

[Index](Documentation.md) 

You can configure the settings of the grid with the parameters and methods of the **GridComponent**, **GridClient** and **GridCoreServer** objects. Remember that the **GridClient** object on the client project and the **GridCoreServer** object on the server project must have compatible settings.
 
## GridComponent parameters

Parameter | Type | Description | Example
--------- | ---- | ----------- | -------
T | ```Type``` (mandatory) | type of the model items | ```<GridComponent T="Order" Grid="@_grid" />```
Grid | ```ICGrid``` (mandatory) | grid object that has to be created in the ```OnParametersSetAsync``` method of the Blazor page | ```<GridComponent T="Order" Grid="@_grid" />```
OnRowClicked | ```Action<object>``` (optional) |  to be executed when selecting a row on "selectable" grids | ```<GridComponent T="Order" Grid="@_grid" OnRowClicked="@OrderDetails" />```
CustomFilters | ```IQueryDictionary<Type>``` (optional) | Dictionary containing all types of custom filter widgets used on the grid  | ```<GridComponent T="Order" Grid="@_grid" CustomFilters="@_customFilters" />```
GridMvcCssClass | ```string``` (optional) | Html classes used by the parent grid element (it overrides default parameter) | ```<GridComponent T="Order" Grid="@_grid" GridMvcCssClass="grid-mvc-alt" />```
GridWrapCssClass | ```string``` (optional) | Html classes used by the wrap element (it overrides default parameter) | ```<GridComponent T="Order" Grid="@_grid" GridWrapCssClass ="grid-wrap-alt" />```
GridFooterCssClass | ```string``` (optional) | Html classes used by the footer element (it overrides default parameter) | ```<GridComponent T="Order" Grid="@_grid" GridFooterCssClass="grid-footer-alt" />```
GridItemsCountCssClass | ```string``` (optional) | Html classes used by the items count element (it overrides default parameter) | ```<GridComponent T="Order" Grid="@_grid" GridItemsCountCssClass="grid-items-count-alt" />```
TableCssClass | ```string``` (optional) | Html classes used by the table element (it overrides default parameter) | ```<GridComponent T="Order" Grid="@_grid" TableCssClass="grid-table-alt" />```
TableWrapCssClass | ```string``` (optional) | Html classes used by the parent div of the table element (it overrides default parameter) | ```<GridComponent T="Order" Grid="@_grid" TableWrapCssClass="table-wrap-alt" />```
GridHeaderCssClass | ```string``` (optional) | Html classes used by the table header element (it overrides default parameter) | ```<GridComponent T="Order" Grid="@_grid" GridHeaderCssClass="grid-header-alt" />```
GridCellCssClass | ```string``` (optional) | Html classes used by the cell elements (it overrides default parameter) | ```<GridComponent T="Order" Grid="@_grid" GridCellCssClass="grid-cell-alt" />```
GridButtonCellCssClass | ```string``` (optional) | Html classes used by the button elements of CRUD grids (it overrides default parameter) | ```<GridComponent T="Order" Grid="@_grid" GridButtonCellCssClass="grid-button-cell-alt" />```
GridSubGridCssClass | ```string``` (optional) | Html classes used by the subgrid elements (it overrides default parameter) | ```<GridComponent T="Order" Grid="@_grid" GridSubGridCssClass="grid-subgrid-alt" />```
GridEmptyTextCssClass | ```string``` (optional) | Html classes used by the empty cell elements (it overrides default parameter) | ```<GridComponent T="Order" Grid="@_grid" GridEmptyTextCssClass="grid-empty-text-alt" />```
GridErrorCssClass | ```string``` (optional) | Html classes used by the error message element (it overrides default parameter) | ```<GridComponent T="Order" Grid="@_grid" GridErrorCssClass="grid-error-alt" />```

## GridClient parameters

Parameter | Description | Example
--------- | ----------- | -------
httpClient | HttpClient from dependency injection | @inject HttpClient HttpClient
url | string containing the url of the action of the server project that provides the grid rows | http://localhost:43550/api/SampleData/GetOrdersGridForSample
query | dictionary containing grid parameters as the initial page | query.Add("grid-page", "2");
renderOnlyRows | boolean to configure if only rows are renderend by the server or all the grid object | Must be **false** for Blazor solutions
gridName | string containing the grid client name  | ordersGrid
columns | lambda expression to define the columns included in the grid (**Optional**) | c => { c.Add(o => o.OrderID); c.Add(o => o.Customer.CompanyName); };
cultureInfo | **CultureInfo** class to define the language of the grid (**Optional**) | new CultureInfo("de-DE");

## GridClient methods

Method name | Description | Example
----------- | ----------- | -------
AutoGenerateColumns | Generates columns for all properties of the model using data annotations | GridClient<Order>(...).AutoGenerateColumns();
Sortable | Enable or disable sorting for all columns of the grid | GridClient<Order>(...).Sortable(true);
Searchable | Enable or disable searching on the grid | GridClient<Order>(...).Searchable(true, true);
Filterable | Enable or disable filtering for all columns of the grid | GridClient<Order>(...).Filterable(true);
WithMultipleFilters | Allow grid to use multiple filters | GridClient<Order>(...).WithMultipleFilters();
ClearFiltersButton | Enable or disable the ClearFilters button | GridClient<Order>(...).ClearFiltersButton(true);
Selectable | Enable or disable the client grid items selectable feature | GridClient<Order>(...).Selectable(true, true);
SetStriped | Enable or disable the grid as a striped one | GridClient<Order>(...).SetStriped(true);
SetKeyboard | Enable or disable the keyboard navigation | GridClient<Order>(...).SetKeyboard(true);
SetModifierKey | Configure the modifier key for keyboard navigation | GridClient<Order>(...).SetModifierKey(ModifierKey.ShiftKey);
EmptyText | Setup the text displayed for all empty items in the grid | GridClient<Order>(...).EmptyText(' - ');
WithGridItemsCount | Allows the grid to show items count | GridClient<Order>(...).WithGridItemsCount();
SetRowCssClasses | Setup specific row css classes | GridClient<Order>(...).SetRowCssClasses(item => item.Customer.IsVip ? "success" : string.Empty);
AddToOnAfterRender | Add a Func<GridComponent<T>, bool, Task> to be executed at the end of the ```OnAfterRenderAsync``` method of the grid component | GridClient<Order>(...).AddToOnAfterRender(OnAfterDepartmentRender);
SetDirection | Allows the grid to be show in right to left direction | GridClient<Order>(...).SetDirection(GridDirection.RTL);
HandleServerErrors | Allows errors from the server to be handled by grid client | GridClient<Order>(...).HandleServerErrors(true, false);
SetTableLayout | Configure fixed dimensions for the grid | GridClient<Order>(...).SetTableLayout(TableLayout.Fixed, "1200px", "400px");

## GridCoreServer parameters

Parameter | Description | Example
--------- | ----------- | -------
items | **IEnumerable<T>** object containing all the grid rows | repository.GetAll()
query | **IQueryCollection** containing all grid parameters | Must be the **Request.Query** of the controller
renderOnlyRows | boolean to configure if only rows are renderend by the server or all the grid object | Must be **true** for Blazor solutions
gridName | string containing the grid client name  | ordersGrid
columns | lambda expression to define the columns included in the grid (**Optional**) | **Columns** lamba expression defined in the razor page of the example
pageSize | integer to define the number of rows returned by the web service (**Optional**) | 10

## GridCoreServer methods

Method name | Description | Example
----------- | ----------- | -------
AutoGenerateColumns | Generates columns for all properties of the model using data annotations | GridCoreServer<Order>(...).AutoGenerateColumns();
Sortable | Enable or disable sorting for all columns of the grid | GridCoreServer<Order>(...).Sortable(true);
Searchable | Enable or disable searching on the grid | GridCoreServer<Order>(...).Searchable(true, true);
Filterable | Enable or disable filtering for all columns of the grid | GridCoreServer<Order>(...).Filterable(true);
WithMultipleFilters | Allow grid to use multiple filters | GridCoreServer<Order>(...).WithMultipleFilters();
WithGridItemsCount | Allows the grid to show items count | GridCoreServer<Order>(...).WithGridItemsCount();


[<- Quick start](Quick_start.md) | [Keyboard navigation ->](Keyboard_navigation.md)