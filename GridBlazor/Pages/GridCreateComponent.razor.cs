#if NETSTANDARD2_1 || NET5_0
using Agno.BlazorInputFile;
#endif
using GridBlazor.Columns;
using GridBlazor.Resources;
using GridShared;
using GridShared.Columns;
using GridShared.Utility;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.CompilerServices;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace GridBlazor.Pages
{
    public partial class GridCreateComponent<T> : ICustomGridComponent<T>
    {
        private bool _shouldRender = false;
        private QueryDictionary<RenderFragment> _renderFragments;
        private IEnumerable<string> _tabGroups;
        private string _code = StringExtensions.RandomString(8);
        private string _confirmationCode = "";

        public string Error { get; set; } = "";
        public QueryDictionary<string> ColumnErrors { get; set; } = new QueryDictionary<string>();
        public QueryDictionary<VariableReference> Children { get; private set; } = new QueryDictionary<VariableReference>();

        public QueryDictionary<VariableReference> InputFiles { get; private set; } = new QueryDictionary<VariableReference>();

#if NETSTANDARD2_1 || NET5_0
        public QueryDictionary<IFileListEntry[]> Files { get; private set; } = new QueryDictionary<IFileListEntry[]>();
#else
        public QueryDictionary<IBrowserFile[]> Files { get; private set; } = new QueryDictionary<IBrowserFile[]>();
#endif

        public QueryDictionary<IEnumerable<SelectItem>> SelectItems { get; private set; } = new QueryDictionary<IEnumerable<SelectItem>>();

        public EditForm Form { get; private set; }

        [Inject]
        private IJSRuntime jSRuntime { get; set; }

        [CascadingParameter(Name = "GridComponent")]
        protected internal GridComponent<T> GridComponent { get; set; }

        [Parameter]
        public T Item { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            _renderFragments = new QueryDictionary<RenderFragment>();
            Children = new QueryDictionary<VariableReference>();
            SelectItems = new QueryDictionary<IEnumerable<SelectItem>>();

            foreach (var column in GridComponent.Grid.Columns)
            {
                // Name must have a non empty value
                if (string.IsNullOrWhiteSpace(column.Name))
                    column.Name = Guid.NewGuid().ToString();

                if (((ICGridColumn)column).ShowCreateSubGrids && ((ICGridColumn)column).SubGrids != null)
                {
                    var values = ((ICGridColumn)column).GetSubGridKeyValues(Item);
                    var grid = await ((ICGridColumn)column).SubGrids(values.Values.ToArray(), true, true, true, true) as ICGrid;
                    grid.Direction = GridComponent.Grid.Direction;
                    grid.FixedValues = values;
                    VariableReference reference = new VariableReference();
                    Children.AddParameter(column.Name, reference);
                    _renderFragments.AddParameter(column.Name, CreateSubGridComponent(grid, reference));
                }
                else if (column.CreateComponentType != null)
                {
                    VariableReference reference = new VariableReference();
                    Children.AddParameter(column.Name, reference);
                    _renderFragments.AddParameter(column.Name, GridCellComponent<T>.CreateComponent(GridComponent,
                        column.CreateComponentType, column, Item, null, true, reference));
                }
                else if (!((IGridColumn<T>)column).IsSelectField.IsSelectKey && !((IGridColumn<T>)column).IsSelectColumn.IsSelectKey
                    && !((IGridColumn<T>)column).ReadOnlyOnCreate(Item) && ((IGridColumn<T>)column).InputType == InputType.File)
                {
                    VariableReference reference;
                    if (InputFiles.ContainsKey(column.FieldName))
                    {
                        reference = InputFiles[column.FieldName];
                    }
                    else
                    {
                        reference = new VariableReference();
                        InputFiles.Add(column.FieldName, reference);
                    }
                    _renderFragments.AddParameter(column.Name, CreateInputFileComponent((IGridColumn<T>)column, reference));
                }
            }
            _tabGroups = GridComponent.Grid.Columns
                .Where(r => !string.IsNullOrWhiteSpace(r.TabGroup) && _renderFragments.Keys.Any(s => s.Equals(r.Name)))
                .Select(r => r.TabGroup).Distinct();

            foreach (var column in GridComponent.Grid.Columns)
            {
                if (((IGridColumn<T>)column).IsSelectColumn.IsSelectKey)
                {
                    var selectItem = await ((IGridColumn<T>)column).SelectItemExpr(Item);
                    SelectItems.AddParameter(column.Name, selectItem);
                }
            }

            _shouldRender = true;
        }

        private RenderFragment CreateSubGridComponent(ICGrid grid, VariableReference reference) => builder =>
        {
            Type gridComponentType = typeof(GridComponent<>).MakeGenericType(grid.Type);
            builder.OpenComponent(0, gridComponentType);
            builder.AddAttribute(1, "Grid", grid);
            builder.AddAttribute(2, "UseMemoryCrudDataService", true);
            builder.AddComponentReferenceCapture(3, r => reference.Variable = r);
            builder.CloseComponent();
        };

        private RenderFragment CreateInputFileComponent(IGridColumn<T> column, VariableReference reference) => builder =>
        {
            Type gridComponentType;
#if NETSTANDARD2_1 || NET5_0
            gridComponentType = typeof(AgnoInputFile);
#else
            gridComponentType = typeof(InputFile);
#endif

            builder.OpenComponent(0, gridComponentType);
            builder.AddAttribute(1, "name", column.FieldName + "-file");
            builder.AddAttribute(2, "style", "display:none;");
            if (column.MultipleInput)
                builder.AddAttribute(3, "multiple", "");
#if NETSTANDARD2_1 || NET5_0
            builder.AddAttribute(4, "OnFileChange", RuntimeHelpers.TypeCheck(EventCallback.Factory.Create<IFileListEntry[]>(this, (files) => OnFileChange(column, files))));
#else
            builder.AddAttribute(4, "OnChange", RuntimeHelpers.TypeCheck(EventCallback.Factory.Create<InputFileChangeEventArgs>(this, (e) => OnChange(column, e))));
#endif
            builder.AddComponentReferenceCapture(5, r => reference.Variable = r);
            builder.CloseComponent();
        };

        protected override bool ShouldRender()
        {
            return _shouldRender;
        }

        protected override void OnAfterRender(bool firstRender)
        {
            _shouldRender = false;
        }

        protected new void StateHasChanged()
        {
            _shouldRender = true;
            base.StateHasChanged();
        }

        private void SetValue(object value, IGridColumn column)
        {
            var names = column.FieldName.Split('.');
            PropertyInfo pi;
            object obj = Item;
            for (int i = 0; i < names.Length - 1; i++)
            {
                pi = obj.GetType().GetProperty(names[i]);
                obj = pi.GetValue(obj, null);
            }
            pi = obj.GetType().GetProperty(names[names.Length - 1]);
            pi.SetValue(obj, value, null);
        }

        private async Task ChangeValue(ChangeEventArgs e, IGridColumn column, string typeAttr = null)
        {
            if (string.IsNullOrWhiteSpace(e.Value.ToString()))
            {
                SetValue(null, column);
            }
            else
            {
                if (typeAttr == "week")
                {
                    var value = DateTimeUtils.FromIso8601WeekDate(e.Value.ToString());
                    SetValue(value, column);
                }
                else if (typeAttr == "month")
                {
                    var value = DateTimeUtils.FromMonthDate(e.Value.ToString());
                    SetValue(value, column);
                }
                else
                {
                    var (type, _) = ((IGridColumn<T>)column).GetTypeAndValue(Item);
                    var typeConverter = TypeDescriptor.GetConverter(type);
                    if (typeConverter != null)
                    {
                        try
                        {
                            object value = null;
                            // if is number type
                            if (type == typeof(decimal) || type == typeof(float) || type == typeof(double) || type == typeof(byte) || type == typeof(short) || type == typeof(int) || type == typeof(long) ||
                                type == typeof(decimal?) || type == typeof(float?) || type == typeof(double?) || type == typeof(byte?) || type == typeof(short?) || type == typeof(int?) || type == typeof(long?))
                            {
                                string thousandSeparator = ",";
                                if (thousandSeparator == CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)
                                {
                                    thousandSeparator = "."; // separators are inverted compared to EN, like is in DE and some other european languages
                                }
                                string valueText = e.Value.ToString();
                                if (valueText.Contains(thousandSeparator))
                                {
                                    valueText = valueText.Replace(thousandSeparator, ""); // removes thousands separator if exist so that parsing can be correctly done
                                }
                                value = typeConverter.ConvertFrom(valueText);
                            }
                            else
                            {
                                value = typeConverter.ConvertFrom(e.Value.ToString());
                            }
                            SetValue(value, column);
                        }
                        catch (Exception)
                        {
                            SetValue(null, column);
                        }
                    }
                }
            }

            if (((IGridColumn<T>)column).AfterChangeValue != null)
                await((IGridColumn<T>)column).AfterChangeValue(Item, GridMode.Create);
        }

#if NETSTANDARD2_1 || NET5_0
        private async Task OnFileChange(IGridColumn column, IFileListEntry[] files)
        {
            try
            {
                if (!column.MultipleInput && files.Length > 1)
                    files = new IFileListEntry[] { files[0] };

                Files.AddParameter(column.FieldName, files);

                if (((IGridColumn<T>)column).AfterChangeValue != null)
                    await((IGridColumn<T>)column).AfterChangeValue(Item, GridMode.Create);
            }
            catch (GridException ex)
            {
                Console.WriteLine(ex.Message);
                Error = string.IsNullOrWhiteSpace(ex.Code) ? ex.Message : ex.Code + " - " + ex.Message;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Error = Strings.CreateError;
            }

            _shouldRender = true;
            StateHasChanged();
        }
#else
        private async Task OnChange(IGridColumn column, InputFileChangeEventArgs e)
        {
            try
            {
                IBrowserFile[] files;
                if (!column.MultipleInput && e.FileCount >= 1)
                    files = new IBrowserFile[] { e.File };
                else
                    files = e.GetMultipleFiles(e.FileCount).ToArray();

                Files.AddParameter(column.FieldName, files);

                if (((IGridColumn<T>)column).AfterChangeValue != null)
                    await ((IGridColumn<T>)column).AfterChangeValue(Item, GridMode.Create);
            }
            catch (GridException ex)
            {
                Console.WriteLine(ex.Message);
                Error = string.IsNullOrWhiteSpace(ex.Code) ? ex.Message : ex.Code + " - " + ex.Message;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Error = Strings.CreateError;
            }

            _shouldRender = true;
            StateHasChanged();
        }
#endif

        protected async Task CreateItem()
        {
            if (GridComponent.Grid.CreateConfirmation && _code != _confirmationCode)
            {
                _shouldRender = true;
                Error = Strings.ConfirmCodeError;
                return;
            }

            try
            {
                bool isValid = await GridComponent.CreateItem(this);
                if (isValid)
                {
                    Error = "";
                    ColumnErrors = new QueryDictionary<string>();
                    _tabGroups = null;
                }
            }
            catch (GridException e)
            {
                Console.WriteLine(e.Message);
                _shouldRender = true;
                Error = string.IsNullOrWhiteSpace(e.Code) ? e.Message : e.Code + " - " + e.Message;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                _shouldRender = true;
                Error = Strings.CreateError;
            } 
        }

        protected async Task ButtonFileClicked(string fieldName)
        {
            var inputFile = InputFiles.Get(fieldName);
            var type = inputFile.Variable.GetType();
#if NETSTANDARD2_1 || NET5_0
            if (type == typeof(AgnoInputFile)
                && ((AgnoInputFile)inputFile.Variable).InputFileElement.Id != null)
            {
                await jSRuntime.InvokeVoidAsync("gridJsFunctions.click", (ElementReference)((AgnoInputFile)inputFile.Variable).InputFileElement);
            }
#else
            if (type == typeof(InputFile)
                && ((InputFile)inputFile.Variable).Element.HasValue)
            {
                await jSRuntime.InvokeVoidAsync("gridJsFunctions.click", ((InputFile)inputFile.Variable).Element.Value);
            }
#endif
        }

        public async Task BackButtonClicked()
        {
            await GridComponent.Back();
        }

    }
}