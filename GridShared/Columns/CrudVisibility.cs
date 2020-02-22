using System;

namespace GridShared.Columns
{
    [Flags]
    public enum CrudHidden
    {
        NONE = 0,
        DETAIL = 1,
        INSERT = 2,
        UPDATE = 4,
        DELETE = 8
    }
}
