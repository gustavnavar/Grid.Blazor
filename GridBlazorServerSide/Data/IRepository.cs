using System.Linq;

namespace GridBlazorServerSide.Data
{
    public interface IRepository<out T>
    {
        IQueryable<T> GetAll();
        T GetById(object id);
    }
}