namespace GridShared
{
    public enum GridDirection
    {
        LTR = 1,
        RTL = 2,
        Auto = 3
    }

    public static class GridDirectionExtensions
    {
        public static string ToDirection(this GridDirection me)
        {
            switch (me)
            {
                case GridDirection.LTR:
                    return "ltr";
                case GridDirection.RTL:
                    return "rtl";
                case GridDirection.Auto:
                    return "auto";
                default:
                    return "ltr";
            }
        }
    }
}