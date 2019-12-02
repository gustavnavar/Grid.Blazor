using System;
using System.Collections.Generic;
using System.Text;

namespace GridBlazor.Columns
{
    interface IConstrainedGridColumn
    {
        bool HasConstraint { get; }
    }

    interface IConstrainedGridColumn<T, TDataType> : IConstrainedGridColumn
    {
        Func<T, TDataType> Constraint { get; }
    }
}
