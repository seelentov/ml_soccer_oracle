using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WebApplication2.Data;
using WebApplication2.Models;
using WebApplication2.Models.ML;
using WebApplication2.Services.Interfaces;

namespace WebApplication2.Services
{
    public class OptionsService : IEntityGetter<Option>, IEntitySetter<Option>
    {
        private readonly DataContext _dbContext;
        public OptionsService(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Option> Get(Expression<Func<Option, bool>> predicate)
        {
            var data = _dbContext.Options.FirstOrDefault(predicate);
            return data;
        }

        public async Task<Option> GetById(int id)
        {
            var data = _dbContext.Options.FirstOrDefault(g => g.Id == id);
            return data;
        }

        public async Task<IEnumerable<Option>> GetRange(Expression<Func<Option, bool>> predicate)
        {
            var data = _dbContext.Options.Where(predicate);
            return data;
        }

        public async Task<IEnumerable<Option>> GetAll()
        {
            var data = _dbContext.Options;
            return data;
        }

        public async Task Remove(Expression<Func<Option, bool>> predicate)
        {
            var data = _dbContext.Options.FirstOrDefault(predicate);

            if (data != null)
            {
                _dbContext.Remove(data);
                _dbContext.SaveChanges();
            }
        }

        public async Task RemoveRange(Expression<Func<Option, bool>> predicate)
        {
            var data = _dbContext.Options.Where(predicate).ToList();

            if (data != null)
            {
                _dbContext.RemoveRange(data);
                _dbContext.SaveChanges();
            }
        }

        public async Task Add(Option game)
        {
            await _dbContext.AddAsync(game);
            _dbContext.SaveChanges();
        }

        public async Task Update(Option option)
        {
            var data = _dbContext.Options.FirstOrDefault(g => g.Key == option.Key);

            if (data != null)
            {
                _dbContext.Options.Attach(data);
                data = option;
                _dbContext.SaveChanges();
            }
        }

        public async Task UpdateOrAdd(Option option)
        {
            var data = _dbContext.Options.FirstOrDefault(g => g.Key == option.Key);

            if (data != null)
            {
                _dbContext.Options.Attach(data);
                data = option;
            }
            else
            {
                await _dbContext.AddAsync(option);
            }

            _dbContext.SaveChanges();
        }

    }
}
