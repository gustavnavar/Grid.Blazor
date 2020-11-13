## GridMvc for ASP.NET Core MVC

# Quick start with GridMvc

[Index](Documentation.md)

Imagine that you have to retrieve a collection of model items in your project. For example if your model class is:
    
```c#
    public class Foo
    {
        public string Title {get; set;}
        public string Description {get;set;}
    }
```

There are 2 methods to configure a grid on ASP.NET Core MVC:
* Create a **GridServer** object on the controller and using a **TagHelper**
* Create a **Grid** on the view using an **HtmlHelper**

## Method 1: Create a **GridServer** object on the controller and using a **TagHelper**

The steps are:

1. Your controller action has to create a **GridServer** object. The required parameters to create a **GridServer** object are a strongly-typed collection of the model items, the controller's request query and the columns definition. The view model must be the server's **Grid** object. An example of this type of controller action is: 

    ```c#
        public class HomeController : Controller
        {
            private readonly FooRepository fooRepository;

            public HomeController(FooRepository fooRepository)
            {
                this.fooRepository = fooRepository;
            }

            public ActionResult Index()
            {
                IQueryable<Foo> items = fooRepository.GetAll();
                Action<IGridColumnCollection<Foo>> columns = c =>
                {
                    columns.Add(foo => foo.Title);
                    columns.Add(foo => foo.Description);
                };
                var server = new GridServer<Order>(items, Request.Query, false, "ordersGrid", columns);

                return View(server.Grid);
            }
        }
    ```

2. And finally the view has to render the **Grid**. You can use a **TagHelper**. A simple view can be as follows:

    ```razor
        @using GridMvc
        @addTagHelper *, GridMvc
        @model ISGrid

        <grid model="@Model" />
    ```

## Method 2: Create a **Grid** on the view using an **HtmlHelper**

The steps to build a Grid page are:

1. Your controller action has to retrieve a strongly-typed collection of the model items and pass it to the view. An example of this type of controller action is: 

    ```c#
        public class HomeController : Controller
        {
            private readonly FooRepository fooRepository;

            public HomeController(FooRepository fooRepository)
            {
                this.fooRepository = fooRepository;
            }

            public ActionResult Index()
            {
                IQueryable<Foo> items = fooRepository.GetAll();
                return View(items);
            }
        }
    ```

2. And finally the view has to render your items collection. You can use an html helper extension.

    For ASP.NET Core MVC 2.x a simple view can be as follows:

    ```razor
        @using GridMvc
        @model IEnumerable<Foo>

        @Html.Grid(Model).Columns(columns =>
        {
            columns.Add(foo => foo.Title);
            columns.Add(foo => foo.Description);
        })
    ```
    
    **Important**: Synchronous operations are disallowed in ASP.NET Core MVC 3.x. So it´s recomended to use at least version 2.9.0 of the **GridMvcCore** nuget package and to add the asynchronuos **RenderAsync** method at the the end of the html helper to avoid **InvalidOperationException** associated with synchronous operations: 

    ```razor
        @using GridMvc
        @model IEnumerable<Foo>

        @await Html.Grid(Model).Columns(columns =>
        {
            columns.Add(foo => foo.Title);
            columns.Add(foo => foo.Description);
        }).RenderAsync()
    ```

## GridServer parameters

Parameter | Description | Example
--------- | ----------- | -------
items | **IEnumerable<T>** object containing all the grid rows | repository.GetAll()
query | **IQueryCollection** containing all grid parameters | Must be the **Request.Query** of the controller
renderOnlyRows | boolean to configure if only rows are renderend by the server or all the grid object | Must be **false** for ASP.NET Core MVC solutions
gridName | string containing the grid client name  | ordersGrid
columns | lambda expression to define the columns included in the grid (**Optional**) | **Columns** lamba expression defined in the razor page of the example
pageSize | integer to define the number of rows returned by the web service (**Optional**) | 10
pagerViewName |string to define the **Pager** (**Optional**) | GridPager.DefaultPagerViewName or GridPager.DefaultAjaxPagerViewName

## GridServer methods

Method name | Description | Example
----------- | ----------- | -------
AutoGenerateColumns | Generates columns for all properties of the model using data annotations | GridServer<Order>(...).AutoGenerateColumns();
Sortable | Enable or disable sorting for all columns of the grid | GridServer<Order>(...).Sortable(true);
Searchable | Enable or disable searching on the grid | GridServer<Order>(...).Searchable(true, true);
Filterable | Enable or disable filtering for all columns of the grid | GridServer<Order>(...).Filterable(true);
WithMultipleFilters | Allow grid to use multiple filters | GridServer<Order>(...).WithMultipleFilters();
ClearFiltersButton | Enable or disable the ClearFilters button | GridClient<Order>(...).ClearFiltersButton(true);
Selectable | Enable or disable the client grid items selectable feature | GridServer<Order>(...).Selectable(true);
WithPaging | Enable paging for grid | GridServer<Order>(...).WithPaging(10);
SetLanguage | Setup the language of the grid | GridServer<Order>(...).SetLanguage('fr');
SetStriped | Configure the grid as striped | GridServer<Order>(...).SetStriped(true);
EmptyText | Setup the text displayed for all empty items in the grid | GridServer<Order>(...).EmptyText(' - ');
WithGridItemsCount | Allows the grid to show items count | GridServer<Order>(...).WithGridItemsCount();
SetRowCssClasses | Setup specific row css classes | GridServer<Order>(...).SetRowCssClasses(item => item.Customer.IsVip ? "success" : string.Empty);
SetDirection | Allows the grid to be show in right to left direction | GridServer<Order>(...).SetDirection(GridDirection.RTL);
SetTableLayout | Configure fixed dimensions for the grid | GridServer<Order>(...).SetTableLayout(TableLayout.Fixed, "1200px", "400px");

## Grid configuration

You can use multiple methods of the **SGrid** object to configure a grid. For example:
```razor
    @Html.Grid(Model).Columns(columns =>
    {
       columns.Add(foo => foo.Title);
       columns.Add(foo => foo.Description);
    }).WithPaging(10).SetLanguage("fr").Sortable().Filterable().WithMultipleFilters().Render()
```
  
## Grid methods

Method name | Description | Example
------------- | ----------- | -------
Named | Setup the grid client name | Html.Grid(Model).Named("Product List");
Columns | Setup the grid client name | Html.Grid(Model).Columns(...);
AutoGenerateColumns | Generates columns for all properties of the model using data annotations | Html.Grid(Model).AutoGenerateColumns();
Sortable | Enable or disable sorting for all columns of the grid | Html.Grid(Model).Sortable(true);
Searchable | Enable or disable searching on the grid | Html.Grid(Model).Searchable(true, true);
Filterable | Enable or disable filtering for all columns of the grid | Html.Grid(Model).Filterable(true);
WithMultipleFilters | Allow grid to use multiple filters | Html.Grid(Model).WithMultipleFilters();
Selectable | Enable or disable the client grid items selectable feature | Html.Grid(Model).Filterable(true);
WithPaging | Enable paging for grid | Html.Grid(Model).WithPaging(10);
SetLanguage | Setup the language of the grid | Html.Grid(Model).SetLanguage('fr');
SetStriped | Configure the grid as striped | Html.Grid(Model).SetStriped(true);
EmptyText | Setup the text displayed for all empty items in the grid | Html.Grid(Model).EmptyText(' - ');
WithGridItemsCount | Allows the grid to show items count | Html.Grid(Model).WithGridItemsCount();
SetRowCssClasses | Setup specific row css classes | Html.Grid(Model).SetRowCssClasses(item => item.Customer.IsVip ? "success" : string.Empty);
SetDirection | Allows the grid to be show in right to left direction | Html.Grid(Model).SetDirection(GridDirection.RTL);
SetTableLayout | Configure fixed dimensions for the grid | Html.Grid(Model).SetTableLayout(TableLayout.Fixed, "1200px", "400px");

For more documentation about column options, please see: [Custom columns](Custom_columns.md).

[<- Installation](Installation.md) | [Paging ->](Paging.md)