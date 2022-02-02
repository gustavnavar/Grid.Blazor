## Blazor WASM with OData back-end

# Searching

[Index](Documentation.md)

You can enable the searching option for your grid. Searching allows to search for a text on all columns at the same time.

![](../images/Searching.png)

You can enable searching for all columns of a grid using the **Searchable** method for the **GridODataClient** object:

```c#
    var client = new GridODataClient<Order>(httpClient, url, query, false, "ordersGrid", columns, 10, locale)
        .Searchable(true, false, true);
```

## Searching parameters

Parameter | Description | Example
--------- | ----------- | -------
enable (optional) | bool to enable searching on the grid | Searchable(true, ...)
onlyTextColumns (optional) | bool to enable searching on all collumns or just on string ones | Searchable(..., true, ...)
hiddenColumns (optional) | bool to enable searching on hidden columns | Searchable(..., true)
searchOptions (optional) | ```Action<SearchOptions>``` to configure all search options

```enable``` default value is ```true```, ```onlyTextColumns``` default value is ```true```, and ```hiddenColumns``` default value is ```false```.

## SearchOptions attributes

Parameter | Description
--------- | -----------
Enabled (optional) | bool to enable searching on the grid
OnlyTextColumns (optional) | bool to enable searching on all collumns or just on string ones
HiddenColumns (optional) | bool to enable searching on hidden columns
SplittedWords (optional) | bool to enable search of any word contained in the search phrase on any column. The defaul behavior is to search the complete search phrase on any column.

```Enabled``` default value is ```true```, ```OnlyTextColumns``` default value is ```true```, ```HiddenColumns``` default value is ```false```, and ```SplittedWords ``` default value is false.

Searching on boolean columns has been disabled because EF Core 3.0 is not supporting it yet.

[<- Selecting row](Selecting_row.md) | [Filtering ->](Filtering.md)