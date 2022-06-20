## Blazor WASM with GridCore back-end (gRPC)

# Data annotations

[Index](Documentation.md)

You can customize grid and column settings using data annotations. In other words, you can mark properties of your model class as grid columns, specify column options, call **AutoGenerateColumns** method, and **GridBlazor** will automatically create columns as you describe in your annotations.

There are some attributes for this:

* **GridTableAttribute**: applies to model classes and specify options for the grid (paging options...)
* **GridColumnAttribute**: applies to model public properties and configure a property as a column with a set of properties
* **GridHiddenColumn**: applies to model public properties and configures a property as a hidden column
* **NotMappedColumnAttribute**: applies to model public properties and configures a property as NOT a column. If a property has this attribute, **GridBlazor** will not add that column to the column collection

For example a model class with the following data annotations:
 
```c#
    [GridTable(PagingEnabled = true, PageSize = 20)]
    [DataContract]
    public class Foo
    {
        [GridColumn(Position = 0, Title = "Name title", SortEnabled = true, FilterEnabled = true)]
        [DataMember(Order = 1)]
        public string Name { get; set; }

        [GridColumn(Position = 2, Title = "Active Foo?")]
        [DataMember(Order = 2)]
        public bool Enabled { get; set; }

        [GridColumn(Position = 1, Title = "Date", Format = "{0:dd/MM/yyyy}")]
        [DataMember(Order = 3)]
        public DateTime FooDate { get; set; }

        [NotMappedColumn]
        [GridColumn(Position = 3)]
        [DataMember(Order = 4)]
        public byte[] Data { get; set; }
    }
```
describes that the grid table must contain 3 columns (**Name**, **Enabled** and **FooDate**) with custom options. It also enables paging for this grid table and page size as 20 rows.

**Notes**:
* The ```Order``` parameters for the ```DataMember``` attribute can have different values than ``Position``` parameters for the ```GridColumn``` attribute because they have diffente meanings.

The steps to build a grid razor page using data annotations with **GridBlazor** are:

1. Create a razor page on the client project to render the grid. The page file must have a .razor extension. An example of razor page is:

    ```razor
        @page "/"
        @inject IGridClientService gridClientService

        @if (_task.IsCompleted)
        {
            <GridComponent T="Foo" Grid="@_grid"></GridComponent>
        }
        else
        {
            <p><em>Loading...</em></p>
        }

        @code
        {
            private CGrid<Foo> _grid;
            private Task _task;

            protected override async Task OnParametersSetAsync()
            {
                var query = new QueryDictionary<StringValues>();
                query.Add("grid-page", "2");

                var client = new GridClient<Foo>(gridClientService.GetFooGridRows, query, false, "fooGrid", null).AutoGenerateColumns();
                _grid = client.Grid;

                // Set new items to grid
                _task = client.UpdateGrid();
                await _task;
            }
        }
    ```

    **Notes**:
    * The **columns** parameter passed to the **GridClient** constructor must be **null**

    * You must use the **AutoGenerateColumns** method of the **GridClient** object to configure a grid.

2. Create a gRPC method in the server project. An example of this type of method is: 


    ```c#
        public class GridServerService : IGridService
        {
            ...

            public async ValueTask<ItemsDTO<Foo>> GetOrdersGridordersAutoGenerateColumns(QueryDictionary<string> query)
            {
                var repository = new FooRepository(_context);
                IGridServer<Foo> server = new GridCoreServer<Foo>(repository.GetAll(), query,
                    true, "fooGrid", null).AutoGenerateColumns();

                var items = await server.GetItemsToDisplayAsync(async x => await x.ToListAsync());
                return items;
            }
        }
    ```

    **Notes**:
    * The **columns** parameter passed to the **GridCoreServer** constructor must be **null**

    * You must use the **AutoGenerateColumns** method of the **GridCoreServer**

**GridBlazor** will generate columns based on your data annotations when the **AutoGenerateColumns** method is invoked. 

You can add custom columns after or before this method is called, for example:

```c#
    var server = new GridCoreServer<Foo>(...).AutoGenerateColumns().Columns(columns=>columns.Add(foo=>foo.Child.Price))
```

```c#
    var client = new GridClient<Foo>(...).AutoGenerateColumns().Columns(columns=>columns.Add(foo=>foo.Child.Price))
```

You can also overwrite grid options. For example using the **WithPaging** method:

```c#
    var server = new GridCoreServer<Foo>(...).AutoGenerateColumns().WithPaging(10)
```

**Note:** If you use the ```Position``` custom option to order the columns, you must use it on all the columns of the table including the ones using the ```NotMappedColumn``` attribute. If you don't do it the ```AutoGenerateColumns``` will throw an exception.

[<- Localization](Localization.md) | [Render button, checkbox, etc. in a grid cell ->](Render_button_checkbox_etc_in_a_grid_cell.md)