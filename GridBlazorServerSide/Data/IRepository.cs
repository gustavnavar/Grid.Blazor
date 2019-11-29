using System.Linq;
using System.Threading.Tasks;

namespace GridBlazorServerSide.Data
{
    public interface IRepository<T>
    {
        IQueryable<T> GetAll();
        Task<T> GetById(object id);
    }
}