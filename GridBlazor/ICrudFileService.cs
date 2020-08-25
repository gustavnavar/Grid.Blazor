using Agno.BlazorInputFile;
using GridShared.Utility;
using System.Threading.Tasks;

namespace GridBlazor
{
    public interface ICrudFileService<T>
    {
        Task InsertFiles(T item, IQueryDictionary<IFileListEntry[]> files);
        Task<T> UpdateFiles(T item, IQueryDictionary<IFileListEntry[]> files);
        Task DeleteFiles(params object[] keys);
    }
}
