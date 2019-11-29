using System.Threading.Tasks;

namespace GridShared
{
    public interface ICrudDataService<T>
    {
        Task<T> Get(params object[] keys);
        Task Insert(T item);
        Task Update(T item);
        Task Delete(params object[] keys);
    }
}
