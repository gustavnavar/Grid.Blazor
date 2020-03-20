using System;

namespace GridShared.Columns
{
    [Flags]
    public enum CrudHidden
    {
        NONE = 0,
        CREATE = 1,
        READ = 2,
        UPDATE = 4,
        DELETE = 8
    }
}
