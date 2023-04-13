using System;
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
        public PagingType PagingType { get; set; }
        [DataMember(Order = 2)]
        public int PageSize { get; set; }
        [DataMember(Order = 3)]
        public int CurrentPage { get; set; }
        [DataMember(Order = 4)]
        public int ItemsCount { get; set; }
        [DataMember(Order = 5)]
        public int StartIndex { get; set; }
        [DataMember(Order = 6)]
        public int VirtualizedCount { get; set; }
        [Obsolete("This property is obsolete. Use PagingType property", true)]
        [DataMember(Order = 7)]
        public bool EnablePaging { get; set; }

        public PagerDTO()
        { }

        public PagerDTO(PagingType pagingType, int pageSize, int currentPage, int itemsCount, int startIndex, int virtualizedCount)
        {
            PagingType = pagingType;
            PageSize = pageSize;
            CurrentPage = currentPage;
            ItemsCount = itemsCount;
            StartIndex = startIndex;
            VirtualizedCount = virtualizedCount;
        }
    }
}