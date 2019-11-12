using GridShared.Columns;
using Microsoft.AspNetCore.Components;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace GridBlazor
{
    public class GridUpdateComponentBase<T> : ComponentBase
    {
        [CascadingParameter(Name = "GridComponent")]
        protected GridComponent<T> GridComponent { get; set; }

        [Parameter]
        public T Item { get; set; }

        private void ChangeValue(object value, IGridColumn column)
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

        protected void ChangeString(ChangeEventArgs e, IGridColumn column)
        {
            ChangeValue(e.Value.ToString(), column);
        }

        protected void ChangeDateTime(ChangeEventArgs e, IGridColumn column)
        {
            DateTime value;
            DateTime.TryParse(e.Value.ToString(), out value);
            if (value != null && value != default(DateTime))
            {
                ChangeValue(value, column);
            }
        }

        protected void ChangeDateTimeOffset(ChangeEventArgs e, IGridColumn column)
        {
            DateTimeOffset value;
            DateTimeOffset.TryParse(e.Value.ToString(), out value);
            if (value != null && value != default(DateTimeOffset))
            {
                ChangeValue(value, column);
            }
        }

        protected void ChangeTimeSpan(ChangeEventArgs e, IGridColumn column)
        {
            TimeSpan value;
            TimeSpan.TryParse(e.Value.ToString(), out value);
            if (value != null && value != default(TimeSpan))
            {
                ChangeValue(value, column);
            }
        }

        protected void ChangeInt32(ChangeEventArgs e, IGridColumn column)
        {
            Int32 value;
            Int32.TryParse(e.Value.ToString(), out value);
            if (value != default(Int32))
            {
                ChangeValue(value, column);
            }
        }

        protected void ChangeDouble(ChangeEventArgs e, IGridColumn column)
        {
            Double value;
            Double.TryParse(e.Value.ToString(), out value);
            if (value != default(Double))
            {
                ChangeValue(value, column);
            }
        }

        protected void ChangeDecimal(ChangeEventArgs e, IGridColumn column)
        {
            Decimal value;
            Decimal.TryParse(e.Value.ToString(), out value);
            if (value != default(Decimal))
            {
                ChangeValue(value, column);
            }
        }

        protected void ChangeByte(ChangeEventArgs e, IGridColumn column)
        {
            Byte value;
            Byte.TryParse(e.Value.ToString(), out value);
            if (value != default(Byte))
            {
                ChangeValue(value, column);
            }
        }

        protected void ChangeSingle(ChangeEventArgs e, IGridColumn column)
        {
            Single value;
            Single.TryParse(e.Value.ToString(), out value);
            if (value != default(Single))
            {
                ChangeValue(value, column);
            }
        }

        protected void ChangeInt64(ChangeEventArgs e, IGridColumn column)
        {
            Int64 value;
            Int64.TryParse(e.Value.ToString(), out value);
            if (value != default(Int64))
            {
                ChangeValue(value, column);
            }
        }

        protected void ChangeInt16(ChangeEventArgs e, IGridColumn column)
        {
            Int16 value;
            Int16.TryParse(e.Value.ToString(), out value);
            if (value != default(Int16))
            {
                ChangeValue(value, column);
            }
        }

        protected void ChangeUInt64(ChangeEventArgs e, IGridColumn column)
        {
            UInt64 value;
            UInt64.TryParse(e.Value.ToString(), out value);
            if (value != default(UInt64))
            {
                ChangeValue(value, column);
            }
        }

        protected void ChangeUInt32(ChangeEventArgs e, IGridColumn column)
        {
            UInt32 value;
            UInt32.TryParse(e.Value.ToString(), out value);
            if (value != default(UInt32))
            {
                ChangeValue(value, column);
            }
        }

        protected void ChangeUInt16(ChangeEventArgs e, IGridColumn column)
        {
            UInt16 value;
            UInt16.TryParse(e.Value.ToString(), out value);
            if (value != default(UInt16))
            {
                ChangeValue(value, column);
            }
        }

        protected void ChangeBool(ChangeEventArgs e, IGridColumn column)
        {
            bool value;
            bool.TryParse(e.Value.ToString(), out value);
            if (value != default(bool))
            {
                ChangeValue(value, column);
            }
        }

        protected async Task UpdateItem()
        {
            await GridComponent.UpdateItem();
        }

        protected void BackButtonClicked()
        {
            GridComponent.BackButton();
        }

    }
}