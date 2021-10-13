## Blazor WASM with local data

# Totals

[Index](Documentation.md)

You can enable the totals option for each column of your grid.

![](../images/Totals.png)

You can enable searching for each columns of a grid using the **Sum**, **Average**, **Max** and/or **Min** methods for the **Column** object:

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
Sum |enable | bool to enable sum calculation on column | Sum(true)
Average |enable | bool to enable average calculation on column | Average(true)
Max |enable | bool to enable maximum calculation on column | Max(true)
Min |enable | bool to enable minimum calculation on column | Min(true)


[<- Custom columns](Custom_columns.md) | [Sorting ->](Sorting.md)