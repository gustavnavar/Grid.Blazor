## Blazor WASM with GridCore back-end (REST API)

# Searching

[Index](Documentation.md)

You can enable the searching option for your grid. Searching allows to search for a text on all columns at the same time.

![](../images/Searching.png)

You can enable searching for all columns of a grid using the **Searchable** method for both **GridClient** and **GridCoreServer** objects:

* Client project
    ```c#
        var client = new GridClient<Order>(httpClient, url, query, false, "ordersGrid", Columns, locale)
            .Searchable(true, false, true);
    ```

* Server project
    ```c#
        var server = new GridCoreServer<Order>(repository.GetAll(), Request.Query, true, "ordersGrid", columns, 10)
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

# Disable diacritics distinction

GridBlazor distinguishes among letters with diacritics by default. If you search the term "bru, it will return all records that contains "bru", but it won't returns any record containing "brú", "brû" or "brü". 

Anyway, it is possible to override the default behavior, so GridBlazor will return any record containing "brú", "brû" or "brü". 

The solution to be implemented will depend on the back-end used to return the grid data. I will describe the following 2 cases:

- for grids using Entity Framework Core it will be necessary to create a stored function on the database, a static method that will call it and configure the ```GridCoreServer``` object:
    1. For SQL Server you should open the ```SQL Server Studio Management``` tool and execute the following SQL query on your database to create the ```RemoveDiacritics``` function: 
        ```SQL
            CREATE FUNCTION [dbo].[RemoveDiacritics] (
                @input varchar(max)
            )   RETURNS varchar(max)

            AS BEGIN
                DECLARE @result VARCHAR(max);

                select @result = @input collate SQL_Latin1_General_CP1253_CI_AI

                return @result
            END
        ``` 
    2. then you must create the following static function with an string parameter and returning an string. This function will only work from a LINQ expression and it will call the stored function defined before:
        ```c#
            public class NorthwindDbContext : GridShared.Data.SharedDbContext<NorthwindDbContext>
            {
 
                ...

                [DbFunction("RemoveDiacritics", "dbo")]
                public static string RemoveDiacritics(string input)
                {
                     throw new NotImplementedException("This method can only be called using LINQ");
                }
            }
        ```
    3. and finally you must call the ```SetRemoveDiacritics``` method of the ```GridCoreServer``` class:
        ```c#
            var server = new GridCoreServer<Order>(repository.GetAll(), Request.Query, true, "ordersGrid", ColumnCollections.OrderColumns)
                .Searchable(true)
                .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");
        ```

- for data stored in memory you must create the static function that will remove diacritics and configure the ```GridCoreServer``` object:
    1. you must create the following static function with an string parameter and returning an string (other functions removing diacritics are also supported):
        ```c#
            public class StringUtils
            {
 
                ...

                public static string RemoveDiacritics(string text)
                {
                    var normalizedString = text.Normalize(NormalizationForm.FormD);
                    var stringBuilder = new StringBuilder();

                    foreach (var c in normalizedString)
                    {
                        var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                        if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                        {
                            stringBuilder.Append(c);
                        }
                    }

                    return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
                }
            }
        ```
    2. and finally you must call the ```SetRemoveDiacritics``` method of the ```GridCoreServer``` class:
        ```c#
            var server = new GridCoreServer<Order>(repository.GetAll(), Request.Query, true, "ordersGrid", ColumnCollections.OrderColumns)
                .Searchable(true)
                .SetRemoveDiacritics<StringUtils>("RemoveDiacritics");
        ```

[<- Selecting row](Selecting_row.md) | [Filtering ->](Filtering.md)