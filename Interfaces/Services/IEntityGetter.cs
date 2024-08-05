using System.Linq.Expressions;
using WebApplication2.Models;

namespace WebApplication2.Services.Interfaces
{
    public interface IEntityGetter<T>
    {
        public Task<T> Get(Expression<Func<T, bool>> predicate);
        public Task<T> GetById(int Id);
        public Task<IEnumerable<T>> GetRange(Expression<Func<T, bool>> predicate);
        public Task<IEnumerable<T>> GetAll();
    }
}
