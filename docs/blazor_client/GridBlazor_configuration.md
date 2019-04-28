## Blazor client-side

# GridBlazor configuration

[Index](Documentation.md) 

You can configure the settings of the grid with the parameters and methods of the **GridClient** and **GridServer** objects. Remember that the **GridClient** object on the client project and the **GridServer** object on the server project must have compatible settings.
 
## GridClient parameters

Parameter | Description | Example
--------- | ----------- | -------
url | string containing the url of the action of the server project that provides the grid rows | http://localhost:43550/api/SampleData/GetOrdersGridForSample
query | dictionary containing grid parameters as the initial page | query.Add("grid-page", "2");
renderOnlyRows | boolean to configure if only rows are renderend by the server or all the grid object | Must be **false** for Blazor solutions
gridName | string containing the grid client name  | ordersGrid
columns | lambda expression to define the columns included in the grid | c => { c.Add(o => o.OrderID); c.Add(o => o.Customer.CompanyName); };
cultureInfo | **CultureInfo** class to define the language of the grid (**Optional**) | new CultureInfo("de-DE");

## GridClient methods

Method name | Description | Example
----------- | ----------- | -------
AutoGenerateColumns | Generates columns for all properties of the model using data annotations | GridClient<Order>(...).AutoGenerateColumns();
Sortable | Enable or disable sorting for all columns of the grid | GridClient<Order>(...).Sortable(true);
Filterable | Enable or disable filtering for all columns of the grid | GridClient<Order>(...).Filterable(true);
WithMultipleFilters | Allow grid to use multiple filters | GridClient<Order>(...).WithMultipleFilters();
Selectable | Enable or disable the client grid items selectable feature | GridClient<Order>(...).Filterable(true);
EmptyText | Setup the text displayed for all empty items in the grid | GridClient<Order>(...).EmptyText(' - ');
WithGridItemsCount | Allows the grid to show items count | GridClient<Order>(...).WithGridItemsCount();
SetRowCssClasses | Setup specific row css classes | GridClient<Order>(...).SetRowCssClasses(item => item.Customer.IsVip ? "success" : string.Empty);

## GridServer parameters

Parameter | Description | Example
--------- | ----------- | -------
items | **IEnumerable<T>** oblect containing all the grid rows | repository.GetAll()
query | **IQueryCollection** containing all grid parameters | Must be the **Request.Query** of the controller
renderOnlyRows | boolean to configure if only rows are renderend by the server or all the grid object | Must be **true** for Blazor solutions
gridName | string containing the grid client name  | ordersGrid
columns | lambda expression to define the columns included in the grid | **Columns** lamba expression defined in the razor page of the example
pageSize | integer to define the number of rows returned by the web service (**Optional**) | 10

## GridServer methods

Method name | Description | Example
----------- | ----------- | -------
AutoGenerateColumns | Generates columns for all properties of the model using data annotations | GridServer<Order>(...).AutoGenerateColumns();
Sortable | Enable or disable sorting for all columns of the grid | GridServer<Order>(...).Sortable(true);
Filterable | Enable or disable filtering for all columns of the grid | GridServer<Order>(...).Filterable(true);
WithMultipleFilters | Allow grid to use multiple filters | GridServer<Order>(...).WithMultipleFilters();
WithGridItemsCount | Allows the grid to show items count | GridServer<Order>(...).WithGridItemsCount();


[<- Quick start](Quick_start.md) | [Paging ->](Paging.md)