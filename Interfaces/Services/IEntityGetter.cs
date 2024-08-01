using System.Linq.Expressions;
using WebApplication2.Models;

namespace WebApplication2.Services.Interfaces
{
    public interface IEntityGetter<T>
    {
        Task<T> Get(Expression<Func<T, bool>> predicate);
        Task<T> GetById(int Id);
        Task<IEnumerable<T>> GetRange(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> GetAll();
    }
}
