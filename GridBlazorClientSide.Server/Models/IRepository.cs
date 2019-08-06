using System.Linq;

namespace GridBlazorClientSide.Server.Models
{
    public interface IRepository<out T>
    {
        IQueryable<T> GetAll();
        T GetById(object id);
    }
}