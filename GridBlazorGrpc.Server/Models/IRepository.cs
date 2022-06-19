using System.Linq;
using System.Threading.Tasks;

namespace GridBlazorGrpc.Server.Models
{
    public interface IRepository<T>
    {
        IQueryable<T> GetAll();
        Task<T> GetById(object id);
    }
}