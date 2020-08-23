using System.Threading.Tasks;

namespace GridShared.OData
{
    public interface ICrudODataService<T> : ICrudDataService<T>
    {
        Task<T> Add(T item);
    }
}
