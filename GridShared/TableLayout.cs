namespace GridShared
{
    public enum TableLayout
    {
        Auto = 0,
        Fixed = 1
    }

    public static class Extensions
    {
        public static string ToStr(this TableLayout color)
        {
            switch (color)
            {
                case TableLayout.Fixed:
                    return "fixed";
                case TableLayout.Auto:
                default:
                    return "auto";
            }
        }
    }
}