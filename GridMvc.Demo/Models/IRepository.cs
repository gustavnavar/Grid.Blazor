using System.Linq;

namespace GridMvc.Demo.Models
{
    public interface IRepository<out T>
    {
        IQueryable<T> GetAll();
        T GetById(object id);
    }
}