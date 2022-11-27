using GridBlazor.Pages;
using GridShared;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GridBlazor
{
    //// <summary>
    ///     Grid options for html helper
    /// </summary>
    public interface IGridClient<T>
    {
        IGridClient<T> Columns(Action<IGridColumnCollection<T>> columnBuilder);

        /// <summary>
        ///     Enable change page size for grid
        /// </summary>
        /// <param name="enable">Enable dynamic setup the page size of the grid</param>
        IGridClient<T> ChangePageSize(bool enable);

        /// <summary>
        ///     Enable paging for grid
        /// </summary>
        /// <param name="pageSize">Setup the page size of the grid</param>
        IGridClient<T> WithPaging(int pageSize);

        /// <summary>
        ///     Enable paging for grid
        /// </summary>
        /// <param name="pageSize">Setup the page size of the grid</param>
        /// <param name="maxDisplayedItems">Setup max count of displaying pager links</param>
        IGridClient<T> WithPaging(int pageSize, int maxDisplayedItems);

        /// <summary>
        ///     Enable paging for grid
        /// </summary>
        /// <param name="pageSize">Setup the page size of the grid</param>
        /// <param name="maxDisplayedItems">Setup max count of displaying pager links</param>
        /// <param name="queryStringParameterName">Query string parameter name</param>
        IGridClient<T> WithPaging(int pageSize, int maxDisplayedItems, string queryStringParameterName);

        /// <summary>
        ///     Enable sorting for all columns
        /// </summary>
        IGridClient<T> Sortable();

        /// <summary>
        ///     Enable or disable sorting for all columns
        /// </summary>
        IGridClient<T> Sortable(bool enable);

        /// <summary>
        ///     Enable filtering for all columns
        /// </summary>
        IGridClient<T> Filterable();

        /// <summary>
        ///     Enable or disable filtering for all columns
        /// </summary>
        IGridClient<T> Filterable(bool enable);

        /// <summary>
        ///     Enable searching for text columns
        /// </summary>
        IGridClient<T> Searchable();

        /// <summary>
        ///     Enable or disable searching for text columns
        /// </summary>
        IGridClient<T> Searchable(bool enable);

        /// <summary>
        ///     Enable or disable searching for all columns
        /// </summary>
        IGridClient<T> Searchable(bool enable, bool onlyTextColumns);

        /// <summary>
        ///     Enable or disable searching for all columns
        /// </summary>
        IGridClient<T> Searchable(bool enable, bool onlyTextColumns, bool hiddenColumns);

        /// <summary>
        ///     Enable or disable searching for all columns
        /// </summary>
        IGridClient<T> Searchable(Action<SearchOptions> searchOptions);

        /// <summary>
        ///     Enable extended sorting
        /// </summary>
        IGridClient<T> ExtSortable();

        /// <summary>
        ///     Enable or disable extended sorting
        /// </summary>
        IGridClient<T> ExtSortable(bool enable);

        /// <summary>
        ///     Hide extended sorting header
        /// </summary>
        IGridClient<T> ExtSortable(bool enable, bool hidden);

        /// <summary>
        ///     Enable grouping
        /// </summary>
        IGridClient<T> Groupable();

        /// <summary>
        ///     Enable or disable grouping
        /// </summary>
        IGridClient<T> Groupable(bool enable);

        /// <summary>
        ///     Hide grouping header
        /// </summary>
        IGridClient<T> Groupable(bool enable, bool hidden);

        /// <summary>
        ///     Enable column rearrange
        /// </summary>
        IGridClient<T> RearrangeableColumns();

        /// <summary>
        ///     Enable or disable column rearrange
        /// </summary>
        IGridClient<T> RearrangeableColumns(bool enable);

        /// <summary>
        ///     Enable or disable visibility of ClearFiltersButton
        /// </summary>
        IGridClient<T> ClearFiltersButton(bool enable);

        /// <summary>
        ///     Enable or disable client grid items selectable feature
        /// </summary>
        IGridClient<T> Selectable(bool enable);

        /// <summary>
        ///     Enable or disable client grid items selectable feature
        /// </summary>
        IGridClient<T> Selectable(bool enable, bool initSelection);

        /// <summary>
        ///     Enable or disable client grid items selectable/mutli selectable feature
        /// </summary>
        IGridClient<T> Selectable(bool enable, bool initSelection, bool multiSelectable);

        /// <summary>
        /// Enable or disable multi-select functionality
        /// </summary>
        /// <param name="multiSelectable">if set to <c>true</c> Allows user to multi select rows.</param>
        /// <returns></returns>
        /// <autogeneratedoc />
        /// TODO Edit XML Comment Template for MultiSelectable
        IGridClient<T> MultiSelectable(bool multiSelectable);

        /// <summary>
        ///     Enable or disable client grid CRUD
        /// </summary>
        IGridClient<T> Crud(bool enabled, ICrudDataService<T> crudDataService, ICrudFileService<T> crudFileService = null);

        /// <summary>
        ///     Enable or disable client grid CRUD
        /// </summary>
        IGridClient<T> Crud(bool createEnabled, Func<T, bool> enabled, ICrudDataService<T> crudDataService, 
            ICrudFileService<T> crudFileService = null);

        /// <summary>
        ///     Enable or disable client grid CRUD
        /// </summary>
        IGridClient<T> Crud(bool createEnabled, bool readEnabled, bool updateEnabled, bool deleteEnabled, 
            ICrudDataService<T> crudDataService, ICrudFileService<T> crudFileService = null);

        /// <summary>
        ///     Enable or disable client grid CRUD
        /// </summary>
        IGridClient<T> Crud(bool createEnabled, Func<T, bool> readEnabled, Func<T, bool> updateEnabled,
            Func<T, bool> deleteEnabled, ICrudDataService<T> crudDataService, ICrudFileService<T> crudFileService = null);

        /// <summary>
        ///     Enable or disable client grid CRUD with and OData back-end
        /// </summary>
        IGridClient<T> ODataCrud(bool enabled, ICrudFileService<T> crudFileService = null);

        /// <summary>
        ///     Enable or disable client grid CRUD with and OData back-end
        /// </summary>
        IGridClient<T> ODataCrud(bool createEnabled, Func<T, bool> enabled, ICrudFileService<T> crudFileService = null);

        /// <summary>
        ///     Enable or disable client grid CRUD with and OData back-end
        /// </summary>
        IGridClient<T> ODataCrud(bool createEnabled, bool readEnabled, bool updateEnabled, bool deleteEnabled, 
            ICrudFileService<T> crudFileService = null);

        /// <summary>
        ///     Enable or disable client grid CRUD with and OData back-end
        /// </summary>
        IGridClient<T> ODataCrud(bool createEnabled, Func<T, bool> readEnabled, 
            Func<T, bool> updateEnabled, Func<T, bool> deleteEnabled, ICrudFileService<T> crudFileService = null);

        /// <summary>
        ///     Set function to init values for columns in the Create form
        /// </summary>
        IGridClient<T> SetInitCreateValues(Func<T, Task> initCreateValues);

        /// <summary>
        ///     Enable or disable annotations validation for CRUD views
        /// </summary>
        IGridClient<T> SetDataAnnotationsValidation(bool enabled = true);

        /// <summary>
        ///     Configure CRUD button labels
        /// </summary>
        IGridClient<T> SetCrudButtonLabels(string createLabel, string readLabel, string updateLabel, string deleteLabel);

        /// <summary>
        ///     Configure CRUD button tootips
        /// </summary>
        IGridClient<T> SetCrudButtonTooltips(string createTooltip, string readTooltip, string updateTooltip, string deleteTooltip);

        /// <summary>
        ///     Configure CRUD form labels
        /// </summary>
        IGridClient<T> SetCrudFormLabels(string createLabel, string readLabel, string updateLabel, string deleteLabel);

        /// <summary>
        ///     Configure CRUD form button labels
        /// </summary>
        IGridClient<T> SetCrudFormButtonLabels(string createLabel, string updateLabel, string deleteLabel);

        /// <summary>
        ///     Configure delete confirmation fields
        /// </summary>
        IGridClient<T> SetCreateConfirmation(bool enabled, int? width = null, int? labelWidth = null);

        /// <summary>
        ///     Configure delete confirmation fields
        /// </summary>
        IGridClient<T> SetUpdateConfirmation(bool enabled, int? width = null, int? labelWidth = null);

        /// <summary>
        ///     Configure delete confirmation fields
        /// </summary>
        IGridClient<T> SetDeleteConfirmation(bool enabled, int? width = null, int? labelWidth = null);

        /// <summary>
        ///     Configure CRUD buttons on the header
        /// </summary>
        IGridClient<T> SetHeaderCrudButtons(bool enabled);

        /// <summary>
        ///     Setup the Create Component
        /// </summary>
        IGridClient<T> SetCreateComponent<TComponent>();

        /// <summary>
        ///     Setup the Create Component
        /// </summary>
        IGridClient<T> SetCreateComponent<TComponent>(IList<Action<object>> actions);

        /// <summary>
        ///     Setup the Create Component
        /// </summary>
        IGridClient<T> SetCreateComponent<TComponent>(object obj);

        /// <summary>
        ///     Setup the Create Component
        /// </summary>
        IGridClient<T> SetCreateComponent<TComponent>(IList<Action<object>> actions, object obj);

        /// <summary>
        ///     Setup the Create Component
        /// </summary>
        IGridClient<T> SetCreateComponent<TComponent>(IList<Func<object,Task>> functions);

        /// <summary>
        ///     Setup the Create Component
        /// </summary>
        IGridClient<T> SetCreateComponent<TComponent>(IList<Action<object>> actions, IList<Func<object, Task>> functions);

        /// <summary>
        ///     Setup the Create Component
        /// </summary>
        IGridClient<T> SetCreateComponent<TComponent>(IList<Func<object, Task>> functions, object obj);

        /// <summary>
        ///     Setup the Create Component
        /// </summary>
        IGridClient<T> SetCreateComponent<TComponent>(IList<Action<object>> actions, IList<Func<object, Task>> functions, 
            object obj);

        /// <summary>
        ///     Setup the Read Component
        /// </summary>
        IGridClient<T> SetReadComponent<TComponent>();

        /// <summary>
        ///     Setup the Read Component
        /// </summary>
        IGridClient<T> SetReadComponent<TComponent>(IList<Action<object>> actions);

        /// <summary>
        ///     Setup the Read Component
        /// </summary>
        IGridClient<T> SetReadComponent<TComponent>(object obj);

        /// <summary>
        ///     Setup the Read Component
        /// </summary>
        IGridClient<T> SetReadComponent<TComponent>(IList<Action<object>> actions, object obj);

        /// <summary>
        ///     Setup the Read Component
        /// </summary>
        IGridClient<T> SetReadComponent<TComponent>(IList<Func<object, Task>> functions);

        /// <summary>
        ///     Setup the Read Component
        /// </summary>
        IGridClient<T> SetReadComponent<TComponent>(IList<Action<object>> actions, IList<Func<object, Task>> functions);

        /// <summary>
        ///     Setup the Read Component
        /// </summary>
        IGridClient<T> SetReadComponent<TComponent>(IList<Func<object, Task>> functions, object obj);

        /// <summary>
        ///     Setup the Read Component
        /// </summary>
        IGridClient<T> SetReadComponent<TComponent>(IList<Action<object>> actions, IList<Func<object, Task>> functions,
            object obj);

        /// <summary>
        ///     Setup the Update Component
        /// </summary>
        IGridClient<T> SetUpdateComponent<TComponent>();

        /// <summary>
        ///     Setup the Update Component
        /// </summary>
        IGridClient<T> SetUpdateComponent<TComponent>(IList<Action<object>> actions);

        /// <summary>
        ///     Setup the Update Component
        /// </summary>
        IGridClient<T> SetUpdateComponent<TComponent>(object obj);

        /// <summary>
        ///     Setup the Update Component
        /// </summary>
        IGridClient<T> SetUpdateComponent<TComponent>(IList<Action<object>> actions, object obj);

        /// <summary>
        ///     Setup the Update Component
        /// </summary>
        IGridClient<T> SetUpdateComponent<TComponent>(IList<Func<object, Task>> functions);

        /// <summary>
        ///     Setup the Update Component
        /// </summary>
        IGridClient<T> SetUpdateComponent<TComponent>(IList<Action<object>> actions, IList<Func<object, Task>> functions);

        /// <summary>
        ///     Setup the Update Component
        /// </summary>
        IGridClient<T> SetUpdateComponent<TComponent>(IList<Func<object, Task>> functions, object obj);

        /// <summary>
        ///     Setup the Update Component
        /// </summary>
        IGridClient<T> SetUpdateComponent<TComponent>(IList<Action<object>> actions, IList<Func<object, Task>> functions,
            object obj);

        /// <summary>
        ///     Setup the Delete Component
        /// </summary>
        IGridClient<T> SetDeleteComponent<TComponent>();

        /// <summary>
        ///     Setup the Delete Component
        /// </summary>
        IGridClient<T> SetDeleteComponent<TComponent>(IList<Action<object>> actions);

        /// <summary>
        ///     Setup the Delete Component
        /// </summary>
        IGridClient<T> SetDeleteComponent<TComponent>(object obj);

        /// <summary>
        ///     Setup the Delete Component
        /// </summary>
        IGridClient<T> SetDeleteComponent<TComponent>(IList<Action<object>> actions, object obj);

        /// <summary>
        ///     Setup the Delete Component
        /// </summary>
        IGridClient<T> SetDeleteComponent<TComponent>(IList<Func<object, Task>> functions);

        /// <summary>
        ///     Setup the Delete Component
        /// </summary>
        IGridClient<T> SetDeleteComponent<TComponent>(IList<Action<object>> actions, IList<Func<object, Task>> functions);

        /// <summary>
        ///     Setup the Delete Component
        /// </summary>
        IGridClient<T> SetDeleteComponent<TComponent>(IList<Func<object, Task>> functions, object obj);

        /// <summary>
        ///     Setup the Delete Component
        /// </summary>
        IGridClient<T> SetDeleteComponent<TComponent>(IList<Action<object>> actions, IList<Func<object, Task>> functions,
            object obj);

        /// <summary>
        ///     Go to Edit form after insert row
        /// </summary>
        IGridClient<T> SetEditAfterInsert(bool enable);

        /// <summary>
        ///     Setup the Other Component
        /// </summary>
        IGridClient<T> AddButtonComponent<TComponent>(string name, string label);

        /// <summary>
        ///     Setup the Other Component
        /// </summary>
        IGridClient<T> AddButtonComponent<TComponent>(string name, string label, Nullable<MarkupString> content);

        /// <summary>
        ///     Setup the Other Component
        /// </summary>
        IGridClient<T> AddButtonComponent<TComponent>(string name, string label, IList<Action<object>> actions);

        /// <summary>
        ///     Setup the Other Component
        /// </summary>
        IGridClient<T> AddButtonComponent<TComponent>(string name, string label, Nullable<MarkupString> content,
            IList<Action<object>> actions);

        /// <summary>
        ///     Setup the Other Component
        /// </summary>
        IGridClient<T> AddButtonComponent<TComponent>(string name, string label, object obj);

        /// <summary>
        ///     Setup the Other Component
        /// </summary>
        IGridClient<T> AddButtonComponent<TComponent>(string name, string label, Nullable<MarkupString> content, object obj);

        /// <summary>
        ///     Setup the Other Component
        /// </summary>
        IGridClient<T> AddButtonComponent<TComponent>(string name, string label, IList<Action<object>> actions, object obj);

        /// <summary>
        ///     Setup the Other Component
        /// </summary>
        IGridClient<T> AddButtonComponent<TComponent>(string name, string label, Nullable<MarkupString> content, IList<Action<object>> actions, object obj);

        /// <summary>
        ///     Setup the Other Component
        /// </summary>
        IGridClient<T> AddButtonComponent<TComponent>(string name, string label, IList<Func<object, Task>> functions);

        /// <summary>
        ///     Setup the Other Component
        /// </summary>
        IGridClient<T> AddButtonComponent<TComponent>(string name, string label, Nullable<MarkupString> content, IList<Func<object, Task>> functions);

        /// <summary>
        ///     Setup the Other Component
        /// </summary>
        IGridClient<T> AddButtonComponent<TComponent>(string name, string label, IList<Action<object>> actions, IList<Func<object, Task>> functions);

        /// <summary>
        ///     Setup the Other Component
        /// </summary>
        IGridClient<T> AddButtonComponent<TComponent>(string name, string label, Nullable<MarkupString> content, IList<Action<object>> actions, IList<Func<object, Task>> functions);

        /// <summary>
        ///     Setup the Other Component
        /// </summary>
        IGridClient<T> AddButtonComponent<TComponent>(string name, string label, IList<Func<object, Task>> functions, object obj);

        /// <summary>
        ///     Setup the Other Component
        /// </summary>
        IGridClient<T> AddButtonComponent<TComponent>(string name, string label, Nullable<MarkupString> content, IList<Func<object, Task>> functions, object obj);

        /// <summary>
        ///     Setup the Other Component
        /// </summary>
        IGridClient<T> AddButtonComponent<TComponent>(string name, string label, IList<Action<object>> actions, IList<Func<object, Task>> functions,
            object obj);

        /// <summary>
        ///     Setup the Other Component
        /// </summary>
        IGridClient<T> AddButtonComponent<TComponent>(string name, string label, Nullable<MarkupString> content, IList<Action<object>> actions, IList<Func<object, Task>> functions,
            object obj);

        /// <summary>
        ///     Setup the Other Component
        /// </summary>
        IGridClient<T> AddButtonCrudComponent<TComponent>(string name, string label, bool createEnabled, bool readEnabled, bool updateEnabled, bool deleteEnabled);

        /// <summary>
        ///     Setup the Other Component
        /// </summary>
        IGridClient<T> AddButtonCrudComponent<TComponent>(string name, string label, bool createEnabled, bool readEnabled, bool updateEnabled, bool deleteEnabled, Nullable<MarkupString> content);

        /// <summary>
        ///     Setup the Other Component
        /// </summary>
        IGridClient<T> AddButtonCrudComponent<TComponent>(string name, string label, bool createEnabled, bool readEnabled, bool updateEnabled, bool deleteEnabled, IList<Action<object>> actions);

        /// <summary>
        ///     Setup the Other Component
        /// </summary>
        IGridClient<T> AddButtonCrudComponent<TComponent>(string name, string label, bool createEnabled, bool readEnabled, bool updateEnabled, bool deleteEnabled, Nullable<MarkupString> content,
            IList<Action<object>> actions);

        /// <summary>
        ///     Setup the Other Component
        /// </summary>
        IGridClient<T> AddButtonCrudComponent<TComponent>(string name, string label, bool createEnabled, bool readEnabled, bool updateEnabled, bool deleteEnabled, object obj);

        /// <summary>
        ///     Setup the Other Component
        /// </summary>
        IGridClient<T> AddButtonCrudComponent<TComponent>(string name, string label, bool createEnabled, bool readEnabled, bool updateEnabled, bool deleteEnabled, Nullable<MarkupString> content, object obj);

        /// <summary>
        ///     Setup the Other Component
        /// </summary>
        IGridClient<T> AddButtonCrudComponent<TComponent>(string name, string label, bool createEnabled, bool readEnabled, bool updateEnabled, bool deleteEnabled, IList<Action<object>> actions, object obj);

        /// <summary>
        ///     Setup the Other Component
        /// </summary>
        IGridClient<T> AddButtonCrudComponent<TComponent>(string name, string label, bool createEnabled, bool readEnabled, bool updateEnabled, bool deleteEnabled, Nullable<MarkupString> content, IList<Action<object>> actions, object obj);

        /// <summary>
        ///     Setup the Other Component
        /// </summary>
        IGridClient<T> AddButtonCrudComponent<TComponent>(string name, string label, bool createEnabled, bool readEnabled, bool updateEnabled, bool deleteEnabled, IList<Func<object, Task>> functions);

        /// <summary>
        ///     Setup the Other Component
        /// </summary>
        IGridClient<T> AddButtonCrudComponent<TComponent>(string name, string label, bool createEnabled, bool readEnabled, bool updateEnabled, bool deleteEnabled, Nullable<MarkupString> content, IList<Func<object, Task>> functions);

        /// <summary>
        ///     Setup the Other Component
        /// </summary>
        IGridClient<T> AddButtonCrudComponent<TComponent>(string name, string label, bool createEnabled, bool readEnabled, bool updateEnabled, bool deleteEnabled, IList<Action<object>> actions, IList<Func<object, Task>> functions);

        /// <summary>
        ///     Setup the Other Component
        /// </summary>
        IGridClient<T> AddButtonCrudComponent<TComponent>(string name, string label, bool createEnabled, bool readEnabled, bool updateEnabled, bool deleteEnabled, Nullable<MarkupString> content, IList<Action<object>> actions, IList<Func<object, Task>> functions);

        /// <summary>
        ///     Setup the Other Component
        /// </summary>
        IGridClient<T> AddButtonCrudComponent<TComponent>(string name, string label, bool createEnabled, bool readEnabled, bool updateEnabled, bool deleteEnabled, IList<Func<object, Task>> functions, object obj);

        /// <summary>
        ///     Setup the Other Component
        /// </summary>
        IGridClient<T> AddButtonCrudComponent<TComponent>(string name, string label, bool createEnabled, bool readEnabled, bool updateEnabled, bool deleteEnabled, Nullable<MarkupString> content, IList<Func<object, Task>> functions, object obj);

        /// <summary>
        ///     Setup the Other Component
        /// </summary>
        IGridClient<T> AddButtonCrudComponent<TComponent>(string name, string label, bool createEnabled, bool readEnabled, bool updateEnabled, bool deleteEnabled, IList<Action<object>> actions, IList<Func<object, Task>> functions,
            object obj);

        /// <summary>
        ///     Setup the Other Component
        /// </summary>
        IGridClient<T> AddButtonCrudComponent<TComponent>(string name, string label, bool createEnabled, bool readEnabled, bool updateEnabled, bool deleteEnabled, Nullable<MarkupString> content, IList<Action<object>> actions, IList<Func<object, Task>> functions,
            object obj);

        /// <summary>
        ///     Setup the Other Component
        /// </summary>
        IGridClient<T> AddButtonCrudComponent<TComponent>(string name, string label, bool createEnabled, Func<T, bool> readEnabled, Func<T, bool> updateEnabled, Func<T, bool> deleteEnabled);

        /// <summary>
        ///     Setup the Other Component
        /// </summary>
        IGridClient<T> AddButtonCrudComponent<TComponent>(string name, string label, bool createEnabled, Func<T, bool> readEnabled, Func<T, bool> updateEnabled, Func<T, bool> deleteEnabled, Nullable<MarkupString> content);

        /// <summary>
        ///     Setup the Other Component
        /// </summary>
        IGridClient<T> AddButtonCrudComponent<TComponent>(string name, string label, bool createEnabled, Func<T, bool> readEnabled, Func<T, bool> updateEnabled, Func<T, bool> deleteEnabled, IList<Action<object>> actions);

        /// <summary>
        ///     Setup the Other Component
        /// </summary>
        IGridClient<T> AddButtonCrudComponent<TComponent>(string name, string label, bool createEnabled, Func<T, bool> readEnabled, Func<T, bool> updateEnabled, Func<T, bool> deleteEnabled, Nullable<MarkupString> content,
            IList<Action<object>> actions);

        /// <summary>
        ///     Setup the Other Component
        /// </summary>
        IGridClient<T> AddButtonCrudComponent<TComponent>(string name, string label, bool createEnabled, Func<T, bool> readEnabled, Func<T, bool> updateEnabled, Func<T, bool> deleteEnabled, object obj);

        /// <summary>
        ///     Setup the Other Component
        /// </summary>
        IGridClient<T> AddButtonCrudComponent<TComponent>(string name, string label, bool createEnabled, Func<T, bool> readEnabled, Func<T, bool> updateEnabled, Func<T, bool> deleteEnabled, Nullable<MarkupString> content, object obj);

        /// <summary>
        ///     Setup the Other Component
        /// </summary>
        IGridClient<T> AddButtonCrudComponent<TComponent>(string name, string label, bool createEnabled, Func<T, bool> readEnabled, Func<T, bool> updateEnabled, Func<T, bool> deleteEnabled, IList<Action<object>> actions, object obj);

        /// <summary>
        ///     Setup the Other Component
        /// </summary>
        IGridClient<T> AddButtonCrudComponent<TComponent>(string name, string label, bool createEnabled, Func<T, bool> readEnabled, Func<T, bool> updateEnabled, Func<T, bool> deleteEnabled, Nullable<MarkupString> content, IList<Action<object>> actions, object obj);

        /// <summary>
        ///     Setup the Other Component
        /// </summary>
        IGridClient<T> AddButtonCrudComponent<TComponent>(string name, string label, bool createEnabled, Func<T, bool> readEnabled, Func<T, bool> updateEnabled, Func<T, bool> deleteEnabled, IList<Func<object, Task>> functions);

        /// <summary>
        ///     Setup the Other Component
        /// </summary>
        IGridClient<T> AddButtonCrudComponent<TComponent>(string name, string label, bool createEnabled, Func<T, bool> readEnabled, Func<T, bool> updateEnabled, Func<T, bool> deleteEnabled, Nullable<MarkupString> content, IList<Func<object, Task>> functions);

        /// <summary>
        ///     Setup the Other Component
        /// </summary>
        IGridClient<T> AddButtonCrudComponent<TComponent>(string name, string label, bool createEnabled, Func<T, bool> readEnabled, Func<T, bool> updateEnabled, Func<T, bool> deleteEnabled, IList<Action<object>> actions, IList<Func<object, Task>> functions);

        /// <summary>
        ///     Setup the Other Component
        /// </summary>
        IGridClient<T> AddButtonCrudComponent<TComponent>(string name, string label, bool createEnabled, Func<T, bool> readEnabled, Func<T, bool> updateEnabled, Func<T, bool> deleteEnabled, Nullable<MarkupString> content, IList<Action<object>> actions, IList<Func<object, Task>> functions);

        /// <summary>
        ///     Setup the Other Component
        /// </summary>
        IGridClient<T> AddButtonCrudComponent<TComponent>(string name, string label, bool createEnabled, Func<T, bool> readEnabled, Func<T, bool> updateEnabled, Func<T, bool> deleteEnabled, IList<Func<object, Task>> functions, object obj);

        /// <summary>
        ///     Setup the Other Component
        /// </summary>
        IGridClient<T> AddButtonCrudComponent<TComponent>(string name, string label, bool createEnabled, Func<T, bool> readEnabled, Func<T, bool> updateEnabled, Func<T, bool> deleteEnabled, Nullable<MarkupString> content, IList<Func<object, Task>> functions, object obj);

        /// <summary>
        ///     Setup the Other Component
        /// </summary>
        IGridClient<T> AddButtonCrudComponent<TComponent>(string name, string label, bool createEnabled, Func<T, bool> readEnabled, Func<T, bool> updateEnabled, Func<T, bool> deleteEnabled, IList<Action<object>> actions, IList<Func<object, Task>> functions,
            object obj);

        /// <summary>
        ///     Setup the Other Component
        /// </summary>
        IGridClient<T> AddButtonCrudComponent<TComponent>(string name, string label, bool createEnabled, Func<T, bool> readEnabled, Func<T, bool> updateEnabled, Func<T, bool> deleteEnabled, Nullable<MarkupString> content, IList<Action<object>> actions, IList<Func<object, Task>> functions,
            object obj);

        /// <summary>
        ///     Setup the Other Component
        /// </summary>
        IGridClient<T> AddButtonCrudComponent<TComponent>(string name, string label, bool createEnabled, Func<T, Task<bool>> readEnabled, Func<T, Task<bool>> updateEnabled, Func<T, Task<bool>> deleteEnabled);

        /// <summary>
        ///     Setup the Other Component
        /// </summary>
        IGridClient<T> AddButtonCrudComponent<TComponent>(string name, string label, bool createEnabled, Func<T, Task<bool>> readEnabled, Func<T, Task<bool>> updateEnabled, Func<T, Task<bool>> deleteEnabled, Nullable<MarkupString> content);

        /// <summary>
        ///     Setup the Other Component
        /// </summary>
        IGridClient<T> AddButtonCrudComponent<TComponent>(string name, string label, bool createEnabled, Func<T, Task<bool>> readEnabled, Func<T, Task<bool>> updateEnabled, Func<T, Task<bool>> deleteEnabled, IList<Action<object>> actions);

        /// <summary>
        ///     Setup the Other Component
        /// </summary>
        IGridClient<T> AddButtonCrudComponent<TComponent>(string name, string label, bool createEnabled, Func<T, Task<bool>> readEnabled, Func<T, Task<bool>> updateEnabled, Func<T, Task<bool>> deleteEnabled, Nullable<MarkupString> content,
            IList<Action<object>> actions);

        /// <summary>
        ///     Setup the Other Component
        /// </summary>
        IGridClient<T> AddButtonCrudComponent<TComponent>(string name, string label, bool createEnabled, Func<T, Task<bool>> readEnabled, Func<T, Task<bool>> updateEnabled, Func<T, Task<bool>> deleteEnabled, object obj);

        /// <summary>
        ///     Setup the Other Component
        /// </summary>
        IGridClient<T> AddButtonCrudComponent<TComponent>(string name, string label, bool createEnabled, Func<T, Task<bool>> readEnabled, Func<T, Task<bool>> updateEnabled, Func<T, Task<bool>> deleteEnabled, Nullable<MarkupString> content, object obj);

        /// <summary>
        ///     Setup the Other Component
        /// </summary>
        IGridClient<T> AddButtonCrudComponent<TComponent>(string name, string label, bool createEnabled, Func<T, Task<bool>> readEnabled, Func<T, Task<bool>> updateEnabled, Func<T, Task<bool>> deleteEnabled, IList<Action<object>> actions, object obj);

        /// <summary>
        ///     Setup the Other Component
        /// </summary>
        IGridClient<T> AddButtonCrudComponent<TComponent>(string name, string label, bool createEnabled, Func<T, Task<bool>> readEnabled, Func<T, Task<bool>> updateEnabled, Func<T, Task<bool>> deleteEnabled, Nullable<MarkupString> content, IList<Action<object>> actions, object obj);

        /// <summary>
        ///     Setup the Other Component
        /// </summary>
        IGridClient<T> AddButtonCrudComponent<TComponent>(string name, string label, bool createEnabled, Func<T, Task<bool>> readEnabled, Func<T, Task<bool>> updateEnabled, Func<T, Task<bool>> deleteEnabled, IList<Func<object, Task>> functions);

        /// <summary>
        ///     Setup the Other Component
        /// </summary>
        IGridClient<T> AddButtonCrudComponent<TComponent>(string name, string label, bool createEnabled, Func<T, Task<bool>> readEnabled, Func<T, Task<bool>> updateEnabled, Func<T, Task<bool>> deleteEnabled, Nullable<MarkupString> content, IList<Func<object, Task>> functions);

        /// <summary>
        ///     Setup the Other Component
        /// </summary>
        IGridClient<T> AddButtonCrudComponent<TComponent>(string name, string label, bool createEnabled, Func<T, Task<bool>> readEnabled, Func<T, Task<bool>> updateEnabled, Func<T, Task<bool>> deleteEnabled, IList<Action<object>> actions, IList<Func<object, Task>> functions);

        /// <summary>
        ///     Setup the Other Component
        /// </summary>
        IGridClient<T> AddButtonCrudComponent<TComponent>(string name, string label, bool createEnabled, Func<T, Task<bool>> readEnabled, Func<T, Task<bool>> updateEnabled, Func<T, Task<bool>> deleteEnabled, Nullable<MarkupString> content, IList<Action<object>> actions, IList<Func<object, Task>> functions);

        /// <summary>
        ///     Setup the Other Component
        /// </summary>
        IGridClient<T> AddButtonCrudComponent<TComponent>(string name, string label, bool createEnabled, Func<T, Task<bool>> readEnabled, Func<T, Task<bool>> updateEnabled, Func<T, Task<bool>> deleteEnabled, IList<Func<object, Task>> functions, object obj);

        /// <summary>
        ///     Setup the Other Component
        /// </summary>
        IGridClient<T> AddButtonCrudComponent<TComponent>(string name, string label, bool createEnabled, Func<T, Task<bool>> readEnabled, Func<T, Task<bool>> updateEnabled, Func<T, Task<bool>> deleteEnabled, Nullable<MarkupString> content, IList<Func<object, Task>> functions, object obj);

        /// <summary>
        ///     Setup the Other Component
        /// </summary>
        IGridClient<T> AddButtonCrudComponent<TComponent>(string name, string label, bool createEnabled, Func<T, Task<bool>> readEnabled, Func<T, Task<bool>> updateEnabled, Func<T, Task<bool>> deleteEnabled, IList<Action<object>> actions, IList<Func<object, Task>> functions,
            object obj);

        /// <summary>
        ///     Setup the Other Component
        /// </summary>
        IGridClient<T> AddButtonCrudComponent<TComponent>(string name, string label, bool createEnabled, Func<T, Task<bool>> readEnabled, Func<T, Task<bool>> updateEnabled, Func<T, Task<bool>> deleteEnabled, Nullable<MarkupString> content, IList<Action<object>> actions, IList<Func<object, Task>> functions,
            object obj);
        /// <summary>
        ///     Setup the text, which will displayed with empty items collection in the grid
        /// </summary>
        /// <param name="text">Grid empty text</param>
        IGridClient<T> EmptyText(string text);

        /// <summary>
        ///     Setup the language of Grid.Mvc
        /// </summary>
        /// <param name="lang">SetLanguage string (example: "en", "ru", "fr" etc.)</param>
        IGridClient<T> SetLanguage(string lang);

        /// <summary>
        ///     Setup specific row css classes
        /// </summary>
        IGridClient<T> SetRowCssClasses(Func<T, string> contraint);

        /// <summary>
        ///     Specify Grid client name
        /// </summary>
        IGridClient<T> Named(string gridName);

        /// <summary>
        ///     Generates columns for all properties of the model.
        ///     Use data annotations to customize columns
        /// </summary>
        IGridClient<T> AutoGenerateColumns();

        /// <summary>
        ///     Allow grid to use multiple filters
        /// </summary>
        IGridClient<T> WithMultipleFilters();

        /// <summary>
        ///    Allow grid to show Grid items count
        /// </summary>
        IGridClient<T> WithGridItemsCount(string gridItemsName);

        /// <summary>
        ///    Allow grid to show Grid items count
        /// </summary>
        IGridClient<T> WithGridItemsCount();

        /// <summary>
        ///     Enable or disable striped grid
        /// </summary>
        IGridClient<T> SetStriped(bool enable);

        /// <summary>
        ///    Allow grid to show a SubGrid
        /// </summary>
        [Obsolete("This method is obsolete. Use one including an '(string,string)[]' keys parameter.", true)]
        IGridClient<T> SubGrid(Func<object[], Task<ICGrid>> subGrids, params string[] keys);

        /// <summary>
        ///    Allow grid to show a SubGrid
        /// </summary>
        IGridClient<T> SubGrid(Func<object[], Task<ICGrid>> subGrids, params (string,string)[] keys);

        /// <summary>
        ///    Allow grid to show a SubGrid
        /// </summary>
        IGridClient<T> SubGrid(Func<object[], Task<ICGrid>> subGrids, bool allOpened, params (string, string)[] keys);

        /// <summary>
        ///     Configure keyboard utilization
        /// </summary>
        IGridClient<T> SetKeyboard(bool enable);

        /// <summary>
        ///     Configure the modifier key
        /// </summary>
        IGridClient<T> SetModifierKey(ModifierKey modifierKey, ModifierKey selectionKey = ModifierKey.ShiftKey);

        /// <summary>
        ///     Allow grid to export to an Excel file
        /// </summary>
        IGridClient<T> SetExcelExport(bool enable, string fileName = null);

        /// <summary>
        ///     Allow grid to export to an Excel file
        /// </summary>
        IGridClient<T> SetExcelExport(bool enable, bool allRows, string fileName = null);

        /// <summary>
        ///     Configure the Server API
        /// </summary>
        IGridClient<T> UseServerAPI(ServerAPI serverAPI);

        /// <summary>
        ///     Use OData extend for columns
        /// </summary>
        IGridClient<T> UseODataExpand(IEnumerable<string> oDataExpandList);

        /// <summary>
        ///     Use OData extend for columns
        /// </summary>
        IGridClient<T> OverrideODataExpand(IEnumerable<string> oDataExpandList);

        /// <summary>
        ///    Add code to the end of OnAfterRenderAsync method of the component
        /// </summary>
        IGridClient<T> AddToOnAfterRender(Func<GridComponent<T>, bool, Task> OnAfterRender);

        /// <summary>
        ///    Setup the direction of grid
        /// </summary>
        IGridClient<T> SetDirection(GridDirection dir);

        /// <summary>
        ///    Setup the table layout and dimensions
        /// </summary>
        IGridClient<T> SetTableLayout(TableLayout tableLayout, string width = null, string height = null);

        /// <summary>
        ///    Handle UpdateGrid errors from the server
        /// </summary>
        IGridClient<T> HandleServerErrors(bool showOnGrid, bool throwExceptions);

        /// <summary>
        ///    Get grid object
        /// </summary>
        CGrid<T> Grid { get; }

        //void OnPreRender(); //TODO backward Compatibility

        /// <summary>
        ///    Set items from the server api
        /// </summary>
        Task UpdateGrid();
    }
}