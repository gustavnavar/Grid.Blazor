namespace GridShared.Pagination
{
    /// <summary>
    ///     Pager DTO
    /// </summary>
    public class PagerDTO
    {
        public bool EnablePaging { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int ItemsCount { get; set; }

        public PagerDTO()
        { }

        public PagerDTO(bool enablePaging, int pageSize, int currentPage, int itemsCount)
        {
            EnablePaging = enablePaging;
            PageSize = pageSize;
            CurrentPage = currentPage;
            ItemsCount = itemsCount;
        }
    }
}