## Blazor client-side with OData back-end

# Using a list filter

[Index](Documentation.md)

There can be columns where their values can only be selected from a short list. 
You can use an special filter based on a list for those columns. 
It looks like:

![](../images/List_filter.png)

In order to use a list filter you have to create a list of ```SelectItem``` objects in the ```OnParametersSetAsync``` method of the razor page:

```c#
    ...
    _task1 = HttpClient.GetFromJsonAsync<ODataDTO<Shipper>>(NavigationManager.BaseUri + $"odata/Shippers?$select=ShipperID,CompanyName");
    var shippers = await _task1;
    if (shippers == null || shippers.Value == null)
    {
        _shippers = new List<SelectItem>();
    }
    else
    {
        _shippers = shippers.Value
            .Select(r => new SelectItem(r.ShipperID.ToString(), r.ShipperID.ToString() + " - " + r.CompanyName))
            .ToList();
    }
    ...
``` 

There must be an OData service returning an array of ```ODataDTO<T>``` objects in the server project. Its name is ```odata/Shippers``` in this sample.

Then you have to add the column using the ```SetListFilter``` method of the ```GridColumn``` object:
```c#
    c.Add(o => o.ShipVia)
        .RenderValueAs(o => o.Shipper.CompanyName)
        .SetListFilter(_shippers);
``` 

[<- Filtering](Filtering.md) | [Using a date time filter ->](Using_datetime_filter.md)