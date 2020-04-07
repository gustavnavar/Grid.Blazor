using GridShared.Columns;
using System;

namespace GridShared.Events
{
    public class CheckboxEventArgs : EventArgs
    {
        public string ColumnName { get; set; }
        public CheckboxValue Value { get; set; }
    }
}
