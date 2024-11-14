## Blazor WASM with local data

# Installation

[Index](Documentation.md)

The **GridBlazor** component installation is straightforward. Just follow these steps:

1. Create a new Blazor client-side solution using the **Blazor WebAssembly App** template

2. Install [**GridBlazor**](http://nuget.org/packages/GridBlazor/) and [**GridCore**](http://nuget.org/packages/GridCore/) nuget packages on the project.

3. Add the following lines to the **wwwroot/index.html** file:
    ```html
        <link href="_content/GridBlazor/css/gridblazor.min.css" rel="stylesheet" />
        <script src="_content/GridBlazor/js/gridblazor.js"></script>
    ```
    These files will be loaded from the **GridBlazor** nuget package, so it is not necessary to copy it to you project.


4. Add this line to the **Program.cs** file on the client project:
     ```c#
        builder.Services.AddGridBlazor(x => x.Style = CssFramework.Bootstrap_4);
    ```
    You can select the CSS framework used in your project among the following:
    - CssFramework.Bootstrap_4
    - CssFramework.Bootstrap_5
    - CssFramework.Bootstrap_3
    - CssFramework.Materialize
    - CssFramework.Bulma

5. Add the CCS framework that you use to the **wwwroot/index.html** file

6. If you are using Boostrap 3.x you will also need this line in the **wwwroot/index.html** file:
    ```html
        <link href="~/_content/GridBlazor/css/gridblazor-bootstrap3.min.css" rel="stylesheet" />
     ```

7. If you are using Materialize you will also need this line in the **wwwroot/index.html** file:
    ```html
        <link href="~/_content/GridBlazor/css/gridblazor-materialize.min.css" rel="stylesheet" />
     ```

8. If you are using Bulma you will also need this line in the **wwwroot/index.html** file:
    ```html
        <link href="~/_content/GridBlazor/css/gridblazor-bulma.min.css" rel="stylesheet" />
     ```
 
[Quick start ->](Quick_start.md)