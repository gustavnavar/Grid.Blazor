## GridMvc for ASP.NET Core MVC

# Multipile grids on the same page

[Index](Documentation.md)

## Overview the problem

**GridMvc** uses query strings to pass filtering, sorting and paging settings to the server side. 
If you place multiple grids on the same page, those query string parameters will be applied to all grids on the page. 
But there are ways to resolve these conflicts.

## Columns

If your page grids have the same column names, those filtering and sorting settings will be applied to both grids. 
In this case you must use unique internal names for all grid columns. 
You can do that using overloaded **Columns.Add** method to define a unique name for each of those columns:

```c#
    сolumns.Add(o => o.Number, "CustomNumberName");
```
This configuration will be applied just to that column.

The guiding principle is to have unique names for all colummns in all the grids on the page.

## Paging

To resolve paging configuration conflicts you must use "unique" custom pager name for each grid (**grid1-page** in the following example):

```c#
    @await Html.Grid(Model).Columns(columns =>
    {
        ...
    }).WithPaging(15, 6,"grid1-page").RenderAsync()
```

## Client side

If you want to use the [client side object model API](Client_side_object_model.md), then you should give each grid a unique client side ID (**myGrid** in the following example):


```c#
    @await Html.Grid(Model).Named("myGrid").Columns(columns =>
    {
        ...
    }).RenderAsync()
```

[<- Client side object model](Client_side_object_model.md) | [Data annotations ->](Data_annotations.md)