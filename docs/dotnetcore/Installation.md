## GridMvc for ASP.NET Core MVC

# Installation

[Index](Documentation.md)

These are the steps for the installation of the **GridMvc** component:

1. Install the [**GridMvcCore**](http://nuget.org/packages/GridMvcCore/) nuget package on your project.

2. Add the following code to the **Startup** class:
    ```c#
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddGridMvc();
    }
    ```
    It will add references to the required stylesheets, script files and resources from the nuget package.

3. Install the following client libraries on the project:
    * bootstrap
    * bootstrap-datepicker
    * jquery

4. Add the following lines to the **_Layout.cshtml** view or directly to the page:
    ```html
    <link rel="stylesheet" href="~/css/bootstrap/css/bootstrap.css" />
    <link rel="stylesheet" href="~/lib/bootstrap-datepicker/css/bootstrap-datepicker.css" />
    <link rel="stylesheet "href="~/css/gridmvc.css" />
    <script src="~/lib/jquery/jquery.js" type="text/javascript"></script>
    <script src="~/lib/bootstrap-datepicker/js/bootstrap-datepicker.js" type="text/javascript"></script>
    <script src="~/js/gridmvc.js" type="text/javascript"></script>
    ```
    It will call the required style sheet and script files. **gridmvc.css** and **gridmvc.js** files will be loaded from the **GridMvCore** nuget package, so it is not necessary to copy them to you project.

## Additional languagues (optional)

5. For each additional language add the following lines to the **_Layout.cshtml** view or directly to the page:
    ```html
    <script src="~/js/gridmvc-lang-de.js" type="text/javascript"></script>
    <script src="~/lib/bootstrap-datepicker/locales/bootstrap-datepicker.de.min.js" type="text/javascript"></script>
    ```
    It will call the required style sheet and script files. The **gridmvc-lang-xx.js** file will be loaded from the **GridMvCore** nuget package, so it is not necessary to copy it to your project.

## Client side object model support (optional)

If you want to use the client side object model instead of using the default grid server management, you have to follow the following steps:

6. Install the following client libraries on the project:
    * URI.js

7. Add the following lines to the page that will use the client side object model:
    ```html
    <script src="~/js/gridmvc-ajax.js" type="text/javascript"></script>
    <script src="~/lib/URI.js/URI.js" type="text/javascript"></script>
    ```
    The **gridmvc-ajax.js** file will be loaded from the nuget package, so it is not necessary to copy it to your project.

[Quick start ->](Quick_start.md)
