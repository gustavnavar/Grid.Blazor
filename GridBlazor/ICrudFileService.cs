#if NETSTANDARD2_1 || NET5_0
using Agno.BlazorInputFile;
#else
using Microsoft.AspNetCore.Components.Forms;
#endif
using GridShared.Utility;
using System.Threading.Tasks;

namespace GridBlazor
{
    public interface ICrudFileService<T>
    {
#if NETSTANDARD2_1 || NET5_0
        Task InsertFiles(T item, IQueryDictionary<IFileListEntry[]> files);
        Task<T> UpdateFiles(T item, IQueryDictionary<IFileListEntry[]> files);
#else
        Task InsertFiles(T item, IQueryDictionary<IBrowserFile[]> files);
        Task<T> UpdateFiles(T item, IQueryDictionary<IBrowserFile[]> files);
#endif
        Task DeleteFiles(params object[] keys);
    }
}
