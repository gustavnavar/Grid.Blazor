## Blazor server-side

# Installation

[Index](Documentation.md)

The **GridBlazor** component installation is straightforward. Just follow these steps:

1. Create a new Blazor server-side solution using the **Blazor (server-side)** template

2. Install [**GridBlazor**](http://nuget.org/packages/GridBlazor/) and [**GridMvcCore**](http://nuget.org/packages/GridMvcCore/) nuget packages on the project.

3. Add the following code to the **Startup** class:
    ```c#
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGridMvc();
        }
    ```
    It will add references to the required stylesheet from the nuget package.

4. Add the following lines to the **_Host.cshtml** view or directly to the page:
    ```html
        <link rel="stylesheet "href="~/css/gridmvc.css" />
    ```
    The **gridmvc.css** file will be loaded from the **GridMvCore** nuget package, so it is not necessary to copy it to you project.
 
[Quick start ->](Quick_start.md)
