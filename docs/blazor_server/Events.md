## Blazor server-side

# Events

[Index](Documentation.md)

GridBlazor component provides some events to notify other classes or objects when the grid has changed. The supported events are:
- PagerChanged: it's fired when the page number and/or page size are changed 
- SortChanged: it's fired when sorting is changed 
- ExtSortChanged: it's fired when extended sorting or grouping are changed 
- FilterChanged: it's fired when a filter is created, changed or removed
- SearchChanged: it's fired when the a new word is searched or search has been cleared 

If you whant to handle an event you have to create a reference to the ```GridCompoment```. 
Then you have to add the events that you want to handle in the ```OnAfterRender``` method.
And finally you have to write the handlers for each event.

You can see here an example handling all component events that writes all grid changes on the console:

```c#
    <GridComponent @ref="_gridComponent" T="Order" Grid="@_grid"></GridComponent>

    @code
    {
        private GridComponent<Order> _gridComponent;
        ...

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                _gridComponent.PagerChanged += PagerChanged;
                _gridComponent.SortChanged += SortChanged;
                _gridComponent.ExtSortChanged += ExtSortChanged;
                _gridComponent.FilterChanged += FilterChanged;
                _gridComponent.SearchChanged += SearchChanged;
            }
        }

        private async Task PagerChanged(object sender, PagerEventArgs e)
        {
            Console.WriteLine("The pager has changed: EnablePaging: {0}, CurrentPage: {1}, ItemsCount: {2}, PageSize: {3}.",
                e.Pager.EnablePaging, e.Pager.CurrentPage, e.Pager.ItemsCount, e.Pager.PageSize);
            await Task.CompletedTask;
        }

        private async Task SortChanged(object sender, SortEventArgs e)
        {
            Console.WriteLine("Sorting has changed: ColumnName: {0}, Direction: {1}.",
                e.ColumnName, e.Direction);
            await Task.CompletedTask;
        }

        private async Task ExtSortChanged(object sender, ExtSortEventArgs e)
        {
            Console.WriteLine("Extended sorting has changed:");
            foreach (var sortValues in e.SortValues)
            {
                Console.WriteLine(" - ColumnName: {0}, Direction: {1}, Id: {2}.",
                    sortValues.ColumnName, sortValues.Direction, sortValues.Id);
            }
            await Task.CompletedTask;
        }

        private async Task FilterChanged(object sender, FilterEventArgs e)
        {
            Console.WriteLine("Filters have changed:");
            foreach (var filteredColumn in e.FilteredColumns)
            {
                Console.WriteLine(" - ColumnName: {0}, FilterType: {1}, FilterValue: {2}.",
                    filteredColumn.ColumnName, filteredColumn.FilterType, filteredColumn.FilterValue);
            }
            await Task.CompletedTask;
        }

        private async Task SearchChanged(object sender, SearchEventArgs e)
        {
            Console.WriteLine("Search has changed: SearchValue: {0}.", e.SearchValue);
            await Task.CompletedTask;
        }
    }
```

Notice that all handlers must be async.

[<- CRUD](Crud.md)