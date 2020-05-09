using GridShared.Columns;
using GridShared.Events;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GridBlazor.Pages
{
    public partial class CheckboxComponent<T> : ICustomGridComponent<T>
    {
        private Func<T, bool> _expr;
        private Func<T, bool> _readonlyExpr;
        private bool _value = false;
        private bool _readonly = false;
        private string _columnName;

        [CascadingParameter(Name = "GridComponent")]
        protected GridComponent<T> GridComponent { get; set; }

        [Parameter]
        public T Item { get; set; }

        [Parameter]
        public int RowId { get; set; }

        [Parameter]
        public object Object { get; set; }

        protected override void OnParametersSet()
        {
            if (Object.GetType() == typeof((string, Func<T, bool>)))
            {
                (_columnName, _expr) = ((string, Func<T, bool>)) Object;

                // add an empty list if column is not in the dictionary
                if (GridComponent.CheckedRows.Get(_columnName) == null)
                    GridComponent.CheckedRows.Add(_columnName, new List<int>());

                _value = _expr(Item);
                UpdadeList();
            }
            else if (Object.GetType() == typeof((string, Func<T, bool>, Func<T, bool>)))
            {
                (_columnName, _expr, _readonlyExpr) = ((string, Func<T, bool>, Func<T, bool>))Object;

                // add an empty list if column is not in the dictionary
                if (GridComponent.CheckedRows.Get(_columnName) == null)
                    GridComponent.CheckedRows.Add(_columnName, new List<int>());

                _value = _expr(Item);
                _readonly = _readonlyExpr(Item);
                UpdadeList();
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                GridComponent.HeaderCheckboxChanged += HeaderCheckboxChanged;

                if (RowId + 1 == GridComponent.Grid.DisplayingItemsCount)
                {
                    CheckboxEventArgs<T> args = new CheckboxEventArgs<T>
                    {
                        ColumnName = _columnName,
                        Value = CheckboxValue.Checked,
                        Item = Item,
                        RowId = RowId
                    };
                    await GridComponent.OnRowCheckboxChanged(args);
                }
            }
        }

        private async Task ChangeHandler()
        {
            _value = !_value;
            UpdadeList();

            CheckboxEventArgs<T> args = new CheckboxEventArgs<T>
            {
                ColumnName = _columnName,
                Item = Item,
                RowId = RowId
            };
            if (_value)
            {
                args.Value = CheckboxValue.Checked;
            }
            else
            {
                args.Value = CheckboxValue.Unchecked;
            }
            await GridComponent.OnRowCheckboxChanged(args);
        }

        private async Task HeaderCheckboxChanged(CheckboxEventArgs<T> e)
        {
            if (e.ColumnName == _columnName && !_readonly)
            {
                if (e.Value == CheckboxValue.Checked)
                {
                    _value = true;
                }
                else
                {
                    _value = false;
                }
                UpdadeList();
                StateHasChanged();
                await Task.CompletedTask;
            }
        }

        private void UpdadeList()
        {
            var list = GridComponent.CheckedRows.Get(_columnName);
            if (_value)
            {
                if (!list.Contains(RowId))
                    list.Add(RowId);
            }
            else
            {
                list.RemoveAll(r => r == RowId);
            }
        }
    }
}
