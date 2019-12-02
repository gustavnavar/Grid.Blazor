using System;

namespace GridShared.Columns
{
    public interface IConstrainedGridColumn
    {
        bool HasConstraint { get; }
    }

    public interface IConstrainedGridColumn<T, TDataType> : IConstrainedGridColumn
    {
        Func<T, TDataType> Constraint { get; }
    }
}
