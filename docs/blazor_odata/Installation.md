## Blazor client-side with OData back-end

# Installation

[Index](Documentation.md)

The **GridBlazor** component installation is straightforward. Just follow these steps:

1. Create a new Blazor client-side solution using the **Blazor WebAssembly App** template

2. Install the [**GridBlazor**](http://nuget.org/packages/GridBlazor/) nuget package v1.5.0 or lateron the client project.

3. Install the [**Microsoft.AspNetCore.OData**](http://www.nuget.org/packages/Microsoft.AspNetCore.OData/) nuget package v7.4.0 or later on the server project.

4. Add the following lines to the **wwwroot/index.html** file:
    ```html
        <link href="_content/GridBlazor/css/gridblazor.min.css" rel="stylesheet" />
        <script src="_content/GridBlazor/js/gridblazor.js"></script>
    ```
    These files will be loaded from the **GridBlazor** nuget package, so it is not necessary to copy it to you project.


5. If you are using Boostrap 3.x you will also need this line in the **wwwroot/index.html** file:
    ```html
        <link href="~/_content/GridBlazor/css/gridblazor-bootstrap3.min.css" rel="stylesheet" />
     ```

6. Add the OData endpoints in **Startup.cs** file of the server project:

    ```c#
        app.UseEndpoints(endpoints =>
        {
            endpoints.Select().Expand().Filter().OrderBy().MaxTop(100).Count();
            endpoints.MapODataRoute("odata", "odata", EdmModel.GetEdmModel());

            endpoints.MapControllers();
            endpoints.MapFallbackToFile("index.html");
        });
    ```

    It's important to enable ```Select```, ```Expand```, ```Filter```, and ```OrderBy``` operations at application level or at controller level.
    More information in the Microsoft [OData dcoumentation](https://docs.microsoft.com/en-us/dotnet/api/overview/odata-dotnet/).

    If you use another OData server implementation for the back-end you should read the providers documentation to configure the back-end project.
 
[Quick start ->](Quick_start.md)