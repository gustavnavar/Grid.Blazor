## Blazor WASM with GridCore back-end (gRPC)

# Using a list filter

[Index](Documentation.md)

There can be columns where their values can only be selected from a short list. 
You can use an special filter based on a list for those columns. 
It looks like:

![](../images/List_filter.png)

First you have to add a definition to the interface for the gRPC service. This new method will provide the list in the **Shared** project:
```c#
    [ServiceContract]
    public interface IGridService
    {
        ValueTask<ItemsDTO<Order>> GetOrdersGrid(QueryDictionary<string> query);
        ValueTask<IEnumerable<SelectItem>> GetAllShippers();
    }
```

Then you have to implement the new method in the **Server** project as follows:
 ```c#
    public class GridServerService : IGridService
    {
        ...

        public async ValueTask<IEnumerable<SelectItem>> GetAllShippers()
        {
            var repository = new ShipperRepository(_context);
            return await repository.GetAll()
                    .Select(r => new SelectItem(r.ShipperID.ToString(), r.ShipperID.ToString() + " - " + r.CompanyName))
                    .ToListAsync();
        }
    }
```

It's also recomended to add anew method in the gRPC service of the **Client** project calling the server's gRPC service:
```c#
    public class GridClientService : IGridClientService
    {
        ...
        
        public async Task<IEnumerable<SelectItem>> GetAllShippers()
        {
            var handler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler());
            using (var channel = GrpcChannel.ForAddress(_baseUri, new GrpcChannelOptions() { HttpClient = new HttpClient(handler) }))
            {
                var service = channel.CreateGrpcService<IGridService>();
                return await service.GetAllShippers();
            }
        }
    }

    public interface IGridClientService
    {
        ...

        Task<IEnumerable<SelectItem>> GetAllShippers();
    }
```

In order to use a list filter you have to create a list of ```SelectItem``` objects in the ```OnParametersSetAsync``` method of the razor page calling the client gRPC service:

```c#
    ...
    _task1 = gridClientService.GetAllShippers();
    _shippers = await _task1;
    ...
``` 

Then you have to add the column using the ```SetListFilter``` method of the ```GridColumn``` object:
```c#
    c.Add(o => o.ShipVia)
        .RenderValueAs(o => o.Shipper.CompanyName)
        .SetListFilter(_shippers, true, true);
``` 

## SetListFilter parameters

Parameter | Description 
--------- | -----------
selectItems | list of ```SelectItem``` objects to be shown on the list
includeIsNull (optional) | bool to show a list item to select null items
includeIsNotNull (optional) | bool to show a list item to select no-null items
filterOptions (optional) | ```Action<ListFilterOptions>``` to configure all list filter options

```includeIsNull``` default value is ```false```, and ```includeIsNotNull``` default value is ```false```.

## ListFilterOptions attributes

Parameter | Description
--------- | -----------
IncludeIsNull (optional) | bool to show a list item to select null items
IncludeIsNotNull (optional) | bool to show a list item to select no-null items
ShowSelectAllButtons (optional) | bool to show buttons to select all / none item
ShowSearchInput (optional) | bool to show a text box to filter the items shown on the list

Default value for all attributes is ```false```.

## List filter with search input

You can add searching features to the list filter. You have to add the column using the ```SetListFilter``` method of the ```GridColumn``` object as follows:
```c#
    c.Add(o => o.ShipVia)
        .RenderValueAs(o => o.Shipper.CompanyName)
        .SetListFilter(_shippers, o => {
            o.ShowSelectAllButtons = true;
            o.ShowSearchInput = true;
        });
```

The result looks like:

![](../images/List_filter_with_search.png)

[<- Filtering](Filtering.md) | [Using a date time filter ->](Using_datetime_filter.md)