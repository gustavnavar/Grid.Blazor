## Blazor WASM with GridCore back-end

# Using a list filter

[Index](Documentation.md)

There can be columns where their values can only be selected from a short list. 
You can use an special filter based on a list for those columns. 
It looks like:

![](../images/List_filter.png)

In order to use a list filter you have to create a list of ```SelectItem``` objects in the ```OnParametersSetAsync``` method of the razor page:

```c#
    ...
    _task1 = HttpClient.GetJsonAsync<SelectItem[]>(NavigationManager.BaseUri + $"api/SampleData/GetAllShippers");
    var shippers = await _task1;
    _shippers = shippers.ToList();
    ...
``` 

There must be a web service returning an array of ```SelectItem``` objects in the server project. Its name is ```api/SampleData/GetAllShippers``` in this sample.

Then you have to add the column using the ```SetListFilter``` method of the ```GridColumn``` object:
```c#
    c.Add(o => o.ShipVia)
        .RenderValueAs(o => o.Shipper.CompanyName)
        .SetListFilter(_shippers, true, true);
``` 

[<- Filtering](Filtering.md) | [Using a date time filter ->](Using_datetime_filter.md)