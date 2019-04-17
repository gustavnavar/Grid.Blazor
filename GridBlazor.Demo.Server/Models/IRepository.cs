using System.Linq;

namespace GridBlazor.Demo.Server.Models
{
    public interface IRepository<out T>
    {
        IQueryable<T> GetAll();
        T GetById(object id);
    }
}