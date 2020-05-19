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
                _value = _expr(Item);
            }
            else if (Object.GetType() == typeof((string, Func<T, bool>, Func<T, bool>)))
            {
                (_columnName, _expr, _readonlyExpr) = ((string, Func<T, bool>, Func<T, bool>))Object;
                _value = _expr(Item);
                _readonly = _readonlyExpr(Item);
            }

            // add an empty list if column is not in the dictionary
            if (GridComponent.Checkboxes.Get(_columnName) == null)
                GridComponent.Checkboxes.Add(_columnName, new Dictionary<int, CheckboxComponent<T>>());
            var checkedRows = GridComponent.Checkboxes.Get(_columnName);
            if (checkedRows.ContainsKey(RowId))
                checkedRows[RowId] = this;
            else
                checkedRows.Add(RowId, this);
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                GridComponent.HeaderCheckboxChanged += HeaderCheckboxChanged;
            }
        }

        private async Task ChangeHandler()
        {
            await SetChecked(!_value);
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
                StateHasChanged();
                await Task.CompletedTask;
            }
        }

        public bool IsChecked()
        {
            return _value;
        }

        public async Task SetChecked(bool value)
        {
            _value = value;
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
            StateHasChanged();
        }
    }
}
