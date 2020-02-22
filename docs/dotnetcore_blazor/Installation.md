## GridBlazor for ASP.NET Core MVC

# Installation

[Index](Documentation.md)

The **GridBlazor** component installation is straightforward. Just follow these steps:

1. Create a new ASP.NET Core MVC solution using .Net Core 3.x template.

2. Install [**GridBlazor**](http://nuget.org/packages/GridBlazor/) and [**GridMvcCore**](http://nuget.org/packages/GridMvcCore/) nuget packages on the project.

3. Add the following lines to the **_Host.cshtml** view or directly to the page:
    ```html
        <link href="_content/GridBlazor/css/gridblazor.min.css" rel="stylesheet" />
        <script src="_content/GridBlazor/js/gridblazor.js"></script>
    ```
    These files will be loaded from the **GridBlazor** nuget package, so it is not necessary to copy it to you project.

4. If you are using Boostrap 3.x you will also need this line in the **_Host.cshtml** view or directly to the page:
    ```html
        <link href="~/_content/GridBlazor/css/gridblazor-bootstrap3.min.css" rel="stylesheet" />
     ```
 
The [Grid.Demo](https://github.com/gustavnavar/Grid.Blazor/tree/master/GridMvc.Demo) contains an example using a GridBlazor component. 
The view ```BlazorComponentView``` uses a GridBlazor component.

[Quick start ->](Quick_start.md)