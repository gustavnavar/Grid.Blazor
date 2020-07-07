# Grid.Blazor

A fork from: https://gridmvc.codeplex.com/

It supports .NET Core 3.1 and Blazor WebAssembly 3.2.0

## Notes

- GridMvcCore 3.0.0 does not support .Net Core 2.x. It requires .NET Core 3.1

- GridBlazor 1.6.7 requires a change on the column defintion when selecting rows with checkboxes using the ```SetCheckboxColumn``` method. It's mandatory to identify the columns that are primary keys for the grid. You must do it using the SetPrimaryKey(true) method for the primary key columns' definitions:

    ```c#
        c.Add("CheckboxColumn").SetCheckboxColumn(true, o => o.Customer.IsVip);
        c.Add(o => o.OrderID).SetPrimaryKey(true);
    ```

- GridBlazor 1.6.2 doesn't support the ```CheckedRows``` property anymore. ```CheckedRows``` only allowed to retrieve the checked values, but not to change them. Use the ```Checkboxes``` property instead of it.

- GridBlazor 1.5.0 supports OData server back-ends for Blazor WA applications. More info [here](./docs/blazor_odata/Documentation.md)

- Versions before GridBlazor 1.3.9 had the keyboard navigation enabled by default. This feature requires to focus on the grid element, but it can create problems when used on pages with 2 or more grids. As a consequence, starting with version 1.3.9 it has to be explicitly configured for each grid that requires keyboard navigation. Users can enable keyboard navigation between pages using the ```SetKeyboard``` method of the ```GridClient``` object:

    ```c#
        var client = new GridClient<Order>( ... ).SetKeyboard(true);
    ```

- Grid components have been moved to ```GridBlazor.Pages``` folder in GridBlazor 1.3.2. You must add a reference to this namespace in the ```_Imports.razor```: 

    ```razor
        @using GridBlazor.Pages
    ```

- Blazor Server App require these changes on to the **_Host.cshtml** file for .Net Core 3.1:
    
    ```html
        <link href="_content/GridBlazor/css/gridblazor.min.css" rel="stylesheet" />
        <script src="_content/GridBlazor/js/gridblazor.js"></script>
    ```

- Blazor WebAssembly projects require these changes on to the **wwwroot/index.html** file for version 3.2.0 Preview 1:
    
    ```html
        <link href="_content/GridBlazor/css/gridblazor.min.css" rel="stylesheet" />
        <script src="_content/GridBlazor/js/gridblazor.js"></script>
    ```

- Blazor WebAssembly projects require to use a new constructor of the **GridClient** object including an HttpClient object from Dependency Injection for .Net Core 3.1:
    
    ```razor
        @page "/..."
        @inject HttpClient httpClient

        ...
    
        protected override async Task OnParametersSetAsync()
        {
            ...
            var client = new GridClient<Order>(httpClient, url, query, false, "ordersGrid", Columns);
            ...
        }
    
    ```

- The button to clear all filters is disabled by default starting from ```GridBlazor``` version 1.3.6. You can enable it using the ***ClearFiltersButton*** method of the **GridClient** object:  

    ```razor
        var client = new GridClient<Order>(httpClient, url, query, false, "ordersGrid", Columns).ClearFiltersButton(true);
    ```

## Demo 
http://gridblazor.azurewebsites.net

## Change Log
https://github.com/gustavnavar/Grid.Blazor/releases

## Folder description
* [GridBlazor](./GridBlazor): Library to build de GridBlazor package
* [GridMvc](./GridMvc): Library to build de GridMvcCore package
* [GridShared](./GridShared): Library to build de GridShared package
* [GridBlazorClientSide.Client](./GridBlazorClientSide.Client): Front-end project for the Blazor WebAssembly demo
* [GridBlazorClientSide.Server](./GridBlazorClientSide.Server): Back-end project for the Blazor WebAssembly demo
* [GridBlazorClientSide.Shared](./GridBlazorClientSide.Shared): Shared project for the Blazor WebAssembly demo
* [GridBlazorOData.Client](./GridBlazorOData.Client): Front-end project for the Blazor WebAssembly with OData server demo
* [GridBlazorOData.Server](./GridBlazorOData.Server): Back-end project implementing an OData server for the Blazor WebAssembly demo
* [GridBlazorOData.Shared](./GridBlazorOData.Shared): Shared project for the Blazor WebAssembly with OData server demo
* [GridBlazorServerSide](./GridBlazorServerSide): Blazor Server App demo
* [GridMvc.Demo](./GridMvc.Demo): ASP.NET Core MVC demo
* [GridBlazor.Tests](./GridBlazor.Tests): Unit tests for the GridBlazor library
* [GridMvc.Tests](./GridMvc.Tests): Unit tests for the GridMvcCore library
* [docs](./docs): Documentation

The SQL Server database for all demos can be downloaded from [here](./GridMvc.Demo/App_Data)

Alternatively, if you prefer to install a fresh version of the database you can perform the following steps:
- run this script from Microsoft web to create a new database: https://github.com/microsoft/sql-server-samples/blob/master/samples/databases/northwind-pubs/instnwnd.sql
- add a column to the Customers table with the name IsVip of type bit (NOT NULL) and default value 0:
    ```sql
        USE Northwind;
        ALTER TABLE dbo.Customers ADD IsVip bit NOT NULL DEFAULT 0 WITH VALUES;
        GO
    ```
- change manually some values of the new IsVip column to True
- review the datetime columns. Any missmatch between EF Core model and database definitions will produce an exception and filtering will not work as expected. If database columns are defined as ```datetime``` you must modify the ```NorthwindDbContext``` class including:
    ```c#
        modelBuilder.Entity<Order>().Property(r => r.OrderDate).HasColumnType("datetime");
    ```
    for each datetime column. Or you can change all database columns' type to ```datetime2(7)```.

## Documentation
There are native C# Grid components for Blazor client-side and server-side, and for ASP.NET Core MVC.

You can find the specific documentation for each environment clicking the following links:
* [Documentation for Blazor client-side](./docs/blazor_client/Documentation.md)
* [Documentation for Blazor client-side with OData back-end](./docs/blazor_odata/Documentation.md)
* [Documentation for Blazor server-side](./docs/blazor_server/Documentation.md)
* [Documentation for ASP.NET Core MVC](./docs/dotnetcore/Documentation.md)
* [Using GridBlazor component for ASP.NET Core MVC](./docs/dotnetcore_blazor/Documentation.md)

This is an example of a table of items using this component:

![Image of GridBlazor](./docs/images/GridBlazor.png)


