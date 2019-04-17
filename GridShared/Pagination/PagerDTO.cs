namespace GridShared.Pagination
{
    /// <summary>
    ///     Pager DTO
    /// </summary>
    public class PagerDTO
    {
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int ItemsCount { get; set; }

        public PagerDTO()
        { }

        public PagerDTO(int pageSize, int currentPage, int itemsCount)
        {
            PageSize = pageSize;
            CurrentPage = currentPage;
            ItemsCount = itemsCount;
        }
    }
}