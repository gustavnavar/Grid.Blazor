using GridShared.Style;
using System;

namespace GridShared
{
    public class GridBlazorService : IGridBlazorService
    {
        public CssFramework Style { get; set; }

        public GridBlazorService(Action<IGridBlazorOptions> optionsAction) 
        {
            var options = new GridBlazorOptions();
            if (optionsAction != null)
            {
                optionsAction.Invoke(options);
                Style = options.Style;
            }
            else
                Style = CssFramework.Bootstrap_4;
        }   
    }

    public interface IGridBlazorService
    {
        CssFramework Style { get; set; }
    }
}
