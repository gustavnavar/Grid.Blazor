using System;

namespace GridShared
{
    [Flags]
    public enum GridMode
    {
        Grid = 0,
        Create = 1,
        Read = 2,
        Update = 4,
        Delete = 8,
        Form = 16
    }
}