## Blazor WASM with GridCore back-end (gRPC)

# Installation

[Index](Documentation.md)

We will follow the "Code-first gRPC" approach, as explained [here](https://docs.microsoft.com/en-us/aspnet/core/grpc/code-first?view=aspnetcore-6.0).

The **GridBlazor** component installation for gRPC is straightforward. Just follow these steps:

1. Create a new Blazor client-side solution using the **Blazor WebAssembly App** template

2. Install the following nuget packages on the client project:
    - [**GridBlazor**](http://nuget.org/packages/GridBlazor/)
    - [**protobuf-net.Grpc**](http://nuget.org/packages/protobuf-net.Grpc/)
    - [**Grpc.Net.Client**](http://nuget.org/packages/Grpc.Net.Client/)
    - [**Grpc.Net.Client.Web**](http://nuget.org/packages/Grpc.Net.Client.Web/)

3. Install the following nuget packages on the server project:
    - [**GridCore**](http://nuget.org/packages/GridCore/)
    - [**protobuf-net.Grpc.AspNetCore**](http://nuget.org/packages/protobuf-net.Grpc.AspNetCore/)
    - [**Grpc.AspNetCore.Web**](http://nuget.org/packages/Grpc.AspNetCore.Web/)

4. Install the following nuget packages on the shated project:
    - [**System.ComponentModel.Annotations**](http://nuget.org/packages/System.ComponentModel.Annotations/)
    - [**System.ServiceModel.Primitives**](http://nuget.org/packages/System.ServiceModel.Primitives/)

5. Add the following lines to the **wwwroot/index.html** file:
    ```html
        <link href="_content/GridBlazor/css/gridblazor.min.css" rel="stylesheet" />
        <script src="_content/GridBlazor/js/gridblazor.js"></script>
    ```
    These files will be loaded from the **GridBlazor** nuget package, so it is not necessary to copy it to you project.


6. If you are using Boostrap 3.x you will also need this line in the **wwwroot/index.html** file:
    ```html
        <link href="~/_content/GridBlazor/css/gridblazor-bootstrap3.min.css" rel="stylesheet" />
     ```
 
[Quick start ->](Quick_start.md)