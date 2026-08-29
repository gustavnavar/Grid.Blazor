namespace GridShared.Utility
{
    public class ScreenPosition
    {
        /// <summary>
        ///     The element's resolved writing direction, "ltr" or "rtl". Measured rather than
        ///     configured: the grid already puts dir on its wrapper, so asking the element what
        ///     it inherited beats threading a flag through every caller.
        /// </summary>
        public string Direction { get; set; }

        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int ScreenWidth { get; set; }
        public int ScreenHeight { get; set; }
        public int InnerWidth { get; set; }
        public int InnerHeight { get; set; }

        public ScreenPosition()
        { }
    }
}
