using System.ComponentModel;

namespace GridShared.Style
{
    [DefaultValue(Bootstrap_4)]
    public enum CssFramework
    {
        Bootstrap_3,
        Bootstrap_4,
        Bootstrap_5,
        Materialize,
        Bulma
    }

    public static class CssFrameworkExtensions
    {
        public static string ToText(this CssFramework framework)
        {
            switch (framework)
            {
                case CssFramework.Bootstrap_3:
                    return "Bootstrap 3";
                case CssFramework.Bootstrap_4:
                    return "Bootstrap 4";
                case CssFramework.Bootstrap_5:
                    return "Bootstrap 5";
                case CssFramework.Materialize:
                    return "Materialize";
                case CssFramework.Bulma:
                    return "Bulma";
                default:
                    return "";
            }
        }
    }
}
