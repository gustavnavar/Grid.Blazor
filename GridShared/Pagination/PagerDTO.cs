using System.Runtime.Serialization;

namespace GridShared.Pagination
{
    /// <summary>
    ///     Pager DTO
    /// </summary>
    [DataContract]
    public class PagerDTO
    {
        [DataMember(Order = 1)]
        public bool EnablePaging { get; set; }
        [DataMember(Order = 2)]
        public int PageSize { get; set; }
        [DataMember(Order = 3)]
        public int CurrentPage { get; set; }
        [DataMember(Order = 4)]
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