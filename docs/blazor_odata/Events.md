## Blazor client-side with OData back-end

# Events, exceptions and CRUD validation

[Index](Documentation.md)

## Events and CRUD validation

GridBlazor component provides some events to notify other classes or objects when the grid has changed. The supported events are:
- ```Func<object, PagerEventArgs, Task> PagerChanged```: it's fired when the page number and/or page size are changed 
- ```Func<object, SortEventArgs, Task> SortChanged```: it's fired when sorting is changed 
- ```Func<object, ExtSortEventArgs, Task> ExtSortChanged```: it's fired when extended sorting or grouping are changed 
- ```Func<object, FilterEventArgs, Task> FilterChanged```: it's fired when a filter is created, changed or removed
- ```Func<object, SearchEventArgs, Task> SearchChanged```: it's fired when the a new word is searched or search has been cleared 

And these events are provided to allow running tasks on changing grid items:
- ```Func<GridCreateComponent<T>, T, Task<bool>> BeforeInsert```: it's fired before an item is inserted
- ```Func<GridCreateComponent<T>, T, Task<bool>> BeforeUpdate```: it's fired before an item is updated
- ```Func<GridCreateComponent<T>, T, Task<bool>> BeforeDelete```: it's fired before an item is deleted
- ```Func<GridCreateComponent<T>, T, Task> AfterInsert```: it's fired after an item is inserted
- ```Func<GridCreateComponent<T>, T, Task> AfterUpdate```: it's fired after an item is updated
- ```Func<GridCreateComponent<T>, T, Task> AfterDelete```: it's fired after an item is deleted

If you want to handle an event you have to create a reference to the ```GridCompoment```. 

The ```GridCompoment``` object has an attribute named ```Error``` that can be set to show an error on the CRUD form.

Then you have to add the events that you want to handle in the ```OnAfterRender``` method.

And finally you have to write the handlers for each event.

You can see here an example handling all component events that writes some grid changes on the console and validates a grid item before being inserted, updated and deleted:

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

                _gridComponent.BeforeInsert += BeforeInsert;
                _gridComponent.BeforeUpdate += BeforeUpdate;
                _gridComponent.BeforeDelete += BeforeDelete;
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

        private async Task<bool> BeforeInsert(GridCreateComponent<Order> component, Order item)
        {
            var orderValidator = new OrderValidator();
            var valid = await orderValidator.ValidateAsync(item);

            if (!valid.IsValid)
            {
                component.Error = valid.ToString();
            }

            return valid.IsValid;
        }

        private async Task<bool> BeforeUpdate(GridUpdateComponent<Order> component, Order item)
        {
            var orderValidator = new OrderValidator();
            var valid = await orderValidator.ValidateAsync(item);

            if (!valid.IsValid)
            {
                component.Error = valid.ToString();
            }

            return valid.IsValid;
        }

        private async Task<bool> BeforeDelete(GridDeleteComponent<Order> component, Order item)
        {
            var orderValidator = new OrderValidator();
            var valid = await orderValidator.ValidateAsync(item);

            if (!valid.IsValid)
            {
                component.Error = valid.ToString();
            }

            return valid.IsValid;
        }
    }
```

Notice that all handlers must be async. 

In this sample ```OrderValidator``` is a class that validates the ```Order``` object to be modified. If it's a valid item the event returns ```true```.  If the item is not valid the event writes an error on the form and returns ```false```. 

## Exceptions and messages on the CRUD forms

```GridException``` is provided to throw exceptions from the ```ICrudDataService<T>``` services used for data persistence. The ```GridException``` message will be shown on the CRUD forms.


You can see here an example catching any exception issued during database insert and throwing a new ```GridException``` with a custom message:
```c#
    public async Task Insert(OrderDetail item)
    {
        using (var context = new NorthwindDbContext(_options))
        {
            try
            {
                var repository = new OrderDetailsRepository(context);
                await repository.Insert(item);
                repository.Save();
            }
            catch (Exception e)
            {
                throw new GridException("There was an error during the order detail record creation");
            }
        }
    }

```

You can also throw a ```GridException``` that will show the most inner exception's message on the CRUD form:
```c#
    public async Task Insert(OrderDetail item)
    {
        using (var context = new NorthwindDbContext(_options))
        {
            try
            {
                var repository = new OrderDetailsRepository(context);
                await repository.Insert(item);
                repository.Save();
            }
            catch (Exception e)
            {
                throw new GridException(e);
            }
        }
    }

```

This is an example of a CRUD form error:

![](../images/Crud_error.png)

[<- Nested CRUD](Nested_crud.md) | [Button components on the grid ->](Button_components.md)