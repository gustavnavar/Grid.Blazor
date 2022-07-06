## Blazor WASM with OData back-end

# Totals

[Index](Documentation.md)

You can enable the totals option for each column of your grid.

![](../images/Totals.png)

You can enable total's calculation for each column of a grid using the **Sum**, **Average**, **Max** and/or **Min** methods for the **Column** object:

```c#
    Columns.Add(o => o.Freight).Titled("Freight").Sum(true).Average(true);
```

* **Sum** method works only for number columns
* **Average** method works only for number columns
* **Max** method works for number, date-time and text columns
* **Min** method works for number, date-time and text columns

## Methods

Method | Parameter | Description | Example
------ | --------- | ----------- | -------
Sum | enable | bool to enable sum calculation on column | Sum(true)
Average | enable | bool to enable average calculation on column | Average(true)
Max | enable | bool to enable maximum calculation on column | Max(true)
Min | enable | bool to enable minimum calculation on column | Min(true)


## Calculated totals

It is possible to configure calculated totals based on other totals. They can be added to existing columns or to new ones. In both cases, columns must be named.

You can enable calculated totals for each column of a grid using the **Calculate** method for the **Column** object:

```c#
    Columns.Add(o => o.Freight).Titled("Freight").Sum(true).Average(true)
        .Calculate("Average 2", x => x.Get("Freight").SumValue.Number / x.Grid.ItemsCount)
        .Calculate("Average 3", x => x.Get("Freight").SumValue.Number / x.Get("OrderID").SumValue.Number);;
```

The parameters of the **Calculate** method are:
Parameter | Type | Description
--------- | ---- | -----------
name | string | label of the total
calculation | Func<IGridColumnCollection<T>, object> | function to calculate the total

The function to calculted the total has only one parameter, the grid column collection. 

This parameter includes a reference to the Grid, so you can use any Grid attribute as the number of records.

You can also get any column using the **Get** method. So you can include any column total using the following properties of the columns:
- SumValue
- AverageValue
- MaxValue
- MinValue

These properties contain the total values in one of the following attributes, depending on the type of column:
- Number
- DateTime
- String 

You can also use any other variable of the page in calculation function.

[<- Custom columns](Custom_columns.md) | [Sorting ->](Sorting.md)