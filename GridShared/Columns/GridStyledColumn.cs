using System.Collections.Generic;

namespace GridShared.Columns
{
    public abstract class GridStyledColumn
    {
        private readonly List<string> _classes = new List<string>();
        private readonly List<string> _styles = new List<string>();

        public string GetCssClassesString()
        {
            return string.Join(" ", _classes);
        }

        public string GetCssStylesString()
        {
            return string.Join(" ", _styles);
        }

        public void AddCssClass(string className)
        {
            if (!_classes.Contains(className))
                _classes.Add(className);
        }

        public void AddCssStyle(string styleString)
        {
            if (!_styles.Contains(styleString))
                _styles.Add(styleString);
        }

        public void RemoveCssClass(string className)
        {
            if (_classes.Contains(className))
                _classes.Remove(className);
        }

        public void RemoveCssStyle(string styleString)
        {
            if (_styles.Contains(styleString))
                _styles.Remove(styleString);
        }
    }
}