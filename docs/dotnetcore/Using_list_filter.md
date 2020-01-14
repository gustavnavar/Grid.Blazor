## GridMvc for ASP.NET Core MVC

# Using a list filter

[Index](Documentation.md)

There can be columns where their values can only be selected from a short list. 
You can use an special filter based on a list for those columns. 
It looks like:

![](../images/List_filter.png)

In order to use a list filter you have to create a list of ```SelectItem``` objects in first place:

```c#
    var shipperList = _shippersRepository.GetAll()
        .Select(s => new SelectItem(s.ShipperID.ToString(), r.ShipperID.ToString() + " - " + r.CompanyName))
        .ToList();
``` 

Then you have to add the column using the ```SetListFilter``` method of the ```GridColumn``` object:
```c#
    c.Add(o => o.ShipVia)
        .RenderValueAs(o => o.Shipper.CompanyName)
        .SetListFilter(shipperList);
``` 

[<- Filtering](Filtering.md) | [Creating custom filter widget ->](Creating_custom_filter_widget.md)