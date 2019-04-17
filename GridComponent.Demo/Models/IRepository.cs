using System.Linq;

namespace GridComponent.Demo.Models
{
    public interface IRepository<out T>
    {
        IQueryable<T> GetAll();
        T GetById(object id);
    }
}