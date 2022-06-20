## Blazor WASM with GridCore back-end (REST API)

# Front-end back-end API

[Index](Documentation.md)

Normal usage of  **GridBlazor** and **GridCore** packages doesn't require any knowledge of this interface. This documentation is included only for those who want to implement their own back endpoint instead of using **GridCore**. 

The interface between **GridBlazor** and **GridCore** packages on Blazor client-site solutions uses a query string for the request from the front-end and a json file for the response from the back-end.

## Request

The **GridBlazor** package sends a query string with the following parameters:

Parameter | Description | Example
--------- | ----------- | -------
grid-page | integer to define the requested page number | grid-page=2
grid-column | the name of the column that is used for sorting | grid-column=OrderID
grid-dir | the direction used for sorting: 0 means ascending and 1 means descending | grid-dir=0
grid-sorting | multiple strings used for extended sorting and grouping | grid-sorting=Customer.CompanyName__0__1
grid-filter | multiple strings used for filtering  | grid-filter=OrderCustomDate__6__2019-06-11
grid-clearinitfilter | the name of columns that have an initial filter but it is not used anymore | grid-clearinitfilter=Customer.CompanyName
grid-search | word to be searched on all columns | grid-search=aro
grid-pagesize | integer to dynamically change the initial grid page size | grid-pagesize=20

The parameters **grid-page**, **grid-column**, **grid-dir**, **grid-search** and **grid-pagesize** should appear once in a query string. Their use is straightforward.

But the parameters **grid-sorting**, **grid-filter** and **grid-clearinitfilter** may appear multiple times in a query string. Let's see more detail about them.

* **grid-sorting** is a string with 3 parts separated by "__":
    * the first part is the column name that is sorted or grouped
    * the second part is a number defining the type of sorting:
        * **0**: Ascending
        * **1**: Descending
    * the third part is a number defining the column order in which sorting is applied. It starts with 1 and cannot be repeated

* **grid-filter** is a string with 3 parts separated by "__":
    * the first part is the column name that is filtered
    * the second part is a number defining the type of filter:
        * **1**: Equals
        * **2**: Contains
        * **3**: StartsWith
        * **4**: EndsWidth
        * **5**: GreaterThan
        * **6**: LessThan
        * **7**: GreaterThanOrEquals
        * **8**: LessThanOrEquals
        * **9**: Special type to define the type of condition for multiple filtering
        * **10**: NotEquals
    * the third part is  the **filterValue**, a string for the value of the filter. 

    In the special case of multiple filtering the possible values of a **grid-filter** represents the condition used to combine filter for the specified column:
    * **1**: And
    * **2**: Or

* **grid-clearinitfilter** is used only for columns that have defined an initial filter as:
    
    ```c#
        Columns.Add(o => o.Customers.CompanyName)
            .SetInitialFilter(GridFilterType.StartsWith, "a");
    ```
    While the grid is using the initial filter this parameters must not be used. But from the moment that the initial filter is removed, either by clearing it or by defining other filter for that column, this parameter must be included in the query string with the name of the column.
    A query string can contain multiple times this parameter, once per each column that had an initial filter not used anymore.
    
The following query string is an example:

```url
    /Home/GetOrdersGridRows?
        grid-page=2&
        grid-pagesize=20&
        grid-sorting=Customer.CompanyName__0__1&
        grid-column=OrderID&
        grid-dir=0&
        grid-filter=Freight__5__50&
        grid-filter=Freight__9__1&
        grid-filter=Freight__6__100&
        grid-filter=Customer.CompanyName__1__Around+the+Horn&
        grid-clearinitfilter=Customer.CompanyName&
        grid-search=horn
```
In this example the front-end is requesting:
* the page **2**
* of pages with size **20**
* for the grid sorted/grouped by column **Customer.CompanyName**
* **ascending** ordered
* in first position
* then ordered by column **OrderID**
* **ascending** ordered
* filtered by column **Freight** with a value **greater** than **50**
* using the **and** condition for the column **Freight**
* filtered by column **Freight** with a value **less** than **100**
* filtered by column **Customer.CompanyName** with a value **equals** to **Around the Horn**
* without using the **intial filter** for the column **Customer.CompanyName**
* and including the word **horn** in any column of a register

## Response

The **GridCore** package sends back a **json** reponse string with the following format:

```json
    {
        "items":[ array of registers ],
        "totals":
        {
            "sum":{ values of column's addition },
            "average":{ values of column's average },
            "max":{ values of column's max },
            "min":{ values of column's min }
        },
        "pager":
        {
            "enablePaging": true|false,
            "pageSize": number,
            "currentPage": number,
            "itemsCount": number
        }
    }
```

[<- Passing grid state as parameter](Passing_grid_state_as_parameter.md) | [CRUD ->](Crud.md)