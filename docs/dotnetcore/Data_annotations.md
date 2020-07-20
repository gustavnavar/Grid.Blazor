## GridMvc for ASP.NET Core MVC

# Data annotations

[Index](Documentation.md)

You can customize grid and column settings using data annotations. In other words, you can mark properties of your model class as grid columns, specify column options, call **AutoGenerateColumns** method, and **GridMvc** will automatically create columns as you describe in your annotations.

There are some attributes for this:

* **GridTableAttribute**: applies to model classes and specify options for the grid (paging options...)
* **GridColumnAttribute**: applies to model public properties and configure a property as a column with a set of properties
* **GridHiddenColumn**: applies to model public properties and configures a property as a hidden column
* **NotMappedColumnAttribute**: applies to model public properties and configures a property as NOT a column. If a property has this attribute, **GridMvc** will not add that column to the column collection

For example a model class with the following data annotations:
 
```c#
    [GridTable(PagingEnabled = true, PageSize = 20)]
    public class Foo
    {
        [GridColumn(Position = 0, Title = "Name title", SortEnabled = true, FilterEnabled = true)]
        public string Name { get; set; }

        [GridColumn(Position = 2, Title = "Active Foo?")]
        public bool Enabled { get; set; }

        [GridColumn(Position = 1, Title = "Date", Format = "{0:dd/MM/yyyy}")]
        public DateTime FooDate { get; set; }

        [NotMappedColumn]
        [GridColumn(Position = 3)]
        public byte[]() Data { get; set; }
    }
```
describes that the grid table must contain 3 columns (**Name**, **Enabled** and **FooDate**) with custom options. It also enables paging for this grid table and page size as 20 rows.

Finnaly you can render this table in the view:

```c#
    @await Html.Grid(Model).AutoGenerateColumns().RenderAsync()
```

**GridMvc** will generate columns based on your data annotations when the **AutoGenerateColumns** method is invoked. 

You can add custom columns after or before this method is called, for example:

```c#
    @await Html.Grid(Model).AutoGenerateColumns().Columns(columns=>columns.Add(foo=>foo.Child.Price)).RenderAsync()
```

You can also overwrite grid options. For example using the **WithPaging** method:

```c#
    @await Html.Grid(Model).AutoGenerateColumns().WithPaging(10).RenderAsync()
```

**Note:** If you use the ```Position``` custom option to order the columns, you must use it on all the columns of the table including the ones using the ```NotMappedColumn``` attribute. If you don't do it the ```AutoGenerateColumns``` will throw an exception.

[<- Multipile grids on the same page](Multipile_grids_on_the_same_page.md) | [Render button, checkbox, etc. in a grid cell ->](Render_button_checkbox_etc_in_a_grid_cell.md)