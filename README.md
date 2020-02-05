# Grid.Blazor

A fork from: https://gridmvc.codeplex.com/

It supports .NET Core 3.1 and Blazor WebAssembly 3.2.0 Preview 1

## Notes

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

## Folder description
* [GridBlazor](./GridBlazor): Library to build de GridBlazor package
* [GridMvc](./GridMvc): Library to build de GridMvcCore package
* [GridShared](./GridShared): Library to build de GridShared package
* [GridBlazorClientSide.Client](./GridBlazorClientSide.Client): Front-end project for the Blazor WebAssembly demo
* [GridBlazorClientSide.Server](./GridBlazorClientSide.Server): Back-end project for the Blazor WebAssembly demo
* [GridBlazorClientSide.Shared](./GridBlazorClientSide.Shared): Shared project for the Blazor WebAssembly demo
* [GridBlazorServerSide](./GridBlazorServerSide): Blazor Server App demo
* [GridMvc.Demo](./GridMvc.Demo): ASP.NET Core MVC demo
* [GridBlazor.Tests](./GridBlazor.Tests): Unit tests for the GridBlazor library
* [GridMvc.Tests](./GridMvc.Tests): Unit tests for the GridMvcCore library
* [docs](./docs): Documentation

The SQL Server database for all demos can be downloaded from [here](./GridMvc.Demo/App_Data)

## Documentation
There are native C# Grid components for Blazor client-side and server-side, and for ASP.NET Core MVC.

You can find the specific documentation for each environment clicking the following links:
* [Documentation for Blazor client-side](./docs/blazor_client/Documentation.md)
* [Documentation for Blazor server-side](./docs/blazor_server/Documentation.md)
* [Documentation for ASP.NET Core MVC](./docs/dotnetcore/Documentation.md)

This is an example of a table of items using this component:

![Image of GridBlazor](./docs/images/GridBlazor.png)


