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

2. And finally the view has to render your items collection. You can use an html helper extension. A simple view can be as follows:

    ```razor
        @using GridMvc
        @model IEnumerable<Foo>
        @inject ICompositeViewEngine viewEngine

        @Html.Grid(Model, viewEngine).Columns(columns =>
        {
            columns.Add(foo => foo.Title);
            columns.Add(foo => foo.Description);
        })
    ```

## Grid configuration

You can use multiple methods of the **SGrid** object to configure a grid. For example:
```razor
    @Html.Grid(Model, viewEngine).Columns(columns =>
    {
       columns.Add(foo => foo.Title);
       columns.Add(foo => foo.Description);
    }).WithPaging(10).SetLanguage("fr").Sortable().Filterable().WithMultipleFilters()
```
  
## Grid methods

Method name | Description | Example
------------- | ----------- | -------
Named | Setup the grid client name | Html.Grid(Model, viewEngine).Named("Product List");
Columns | Setup the grid client name | Html.Grid(Model, viewEngine).Columns(...);
AutoGenerateColumns | Generates columns for all properties of the model using data annotations | Html.Grid(Model, viewEngine).AutoGenerateColumns();
Sortable | Enable or disable sorting for all columns of the grid | Html.Grid(Model, viewEngine).Sortable(true);
Filterable | Enable or disable filtering for all columns of the grid | Html.Grid(Model, viewEngine).Filterable(true);
WithMultipleFilters | Allow grid to use multiple filters | Html.Grid(Model, viewEngine).WithMultipleFilters();
Selectable | Enable or disable the client grid items selectable feature | Html.Grid(Model, viewEngine).Filterable(true);
WithPaging | Enable paging for grid | Html.Grid(Model, viewEngine).WithPaging(10);
SetLanguage | Setup the language of the grid | Html.Grid(Model, viewEngine).SetLanguage('fr');
EmptyText | Setup the text displayed for all empty items in the grid | Html.Grid(Model, viewEngine).EmptyText(' - ');
WithGridItemsCount | Allows the grid to show items count | Html.Grid(Model, viewEngine).WithGridItemsCount();
SetRowCssClasses | Setup specific row css classes | Html.Grid(Model, viewEngine).SetRowCssClasses(item => item.Customer.IsVip ? "success" : string.Empty);

For more documentation about column options, please see: [Custom columns](Custom_columns.md).

[<- Installation](Installation.md) | [Paging ->](Paging.md)