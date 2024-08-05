using System.Linq.Expressions;
using WebApplication1.Models;

namespace WebApplication2.Services.Interfaces
{
    public interface IEntitySetter<T>
    {
        public Task Remove(Expression<Func<T, bool>> predicate);
        public Task RemoveRange(Expression<Func<T, bool>> predicate);
        public Task Add(T newEntity);
        public Task Update(T newEntity);
        public Task UpdateOrAdd(T newEntity);
    }
}
