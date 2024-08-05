using System.Linq.Expressions;
using WebApplication2.Data;
using WebApplication2.Models;
using WebApplication2.Services.Interfaces;

namespace WebApplication2.Services
{
    public class LeaguesService : IEntityGetter<League>, IEntitySetter<League>
    {
        private readonly DataContext _dbContext;
        public LeaguesService(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<League> Get(Expression<Func<League, bool>> predicate)
        {
            var data = _dbContext.Leagues.FirstOrDefault(predicate);
            return data;
        }

        public async Task<League> GetById(int id)
        {
            var data = _dbContext.Leagues.FirstOrDefault(g => g.Id == id);
            return data;
        }

        public async Task<IEnumerable<League>> GetRange(Expression<Func<League, bool>> predicate)
        {
            var data = _dbContext.Leagues.Where(predicate);
            return data;
        }

        public async Task<IEnumerable<League>> GetAll()
        {
            var data = _dbContext.Leagues;

            return data;
        }



        public async Task Remove(Expression<Func<League, bool>> predicate)
        {
            var data = _dbContext.Leagues.FirstOrDefault(predicate);

            if (data != null)
            {
                _dbContext.Remove(data);
                _dbContext.SaveChanges();
            }
        }

        public async Task RemoveRange(Expression<Func<League, bool>> predicate)
        {
            var data = _dbContext.Leagues.Where(predicate).ToList();

            if (data != null)
            {
                _dbContext.RemoveRange(data);
                _dbContext.SaveChanges();
            }
        }

        public async Task Add(League game)
        {
            await _dbContext.AddAsync(game);
            _dbContext.SaveChanges();
        }

        public async Task Update(League game)
        {
            var data = _dbContext.Leagues.FirstOrDefault(g => g.Url == game.Url);

            if (data != null)
            {
                _dbContext.Leagues.Attach(data);
                data = game;
                _dbContext.SaveChanges();
            }
        }

        public async Task UpdateOrAdd(League game)
        {
            if (game.Url.Contains("http"))
            {
                var data = _dbContext.Leagues.FirstOrDefault(g => g.Url == game.Url);

                if (data != null)
                {
                    var year = data.Year;

                    _dbContext.Leagues.Attach(data);

                    data = game;
                    data.Year = year;
                }
                else
                {
                    await _dbContext.AddAsync(game);
                }

                _dbContext.SaveChanges();
            }
        }

        public void CheckParsed(Expression<Func<League, bool>> predicate)
        {
            var data = _dbContext.Leagues.FirstOrDefault(predicate);

            if (data != null)
            {
                data.Parsed = true;
                _dbContext.SaveChanges();
            }
        }

        public void UpdateYear(Expression<Func<League, bool>> predicate, int? year)
        {
            var data = _dbContext.Leagues.FirstOrDefault(predicate);

            if (data != null)
            {
                data.Year = year;
                _dbContext.SaveChanges();
            }
        }
    }
}
