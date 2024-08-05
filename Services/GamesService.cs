using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WebApplication2.Data;
using WebApplication2.Models;
using WebApplication2.Models.ML;
using WebApplication2.Services.Interfaces;

namespace WebApplication2.Services
{
    public class GamesService : IEntityGetter<Game>, IEntitySetter<Game>
    {
        private readonly DataContext _dbContext;
        private readonly MLGameAdapter _MLGameAdapter;
        public GamesService(DataContext dbContext, MLGameAdapter mLGameAdapter)
        {
            _dbContext = dbContext;
            _MLGameAdapter = mLGameAdapter;
        }

        public async Task<Game> Get(Expression<Func<Game, bool>> predicate)
        {
            var data = _dbContext.Games.FirstOrDefault(predicate);
            return data;
        }

        public async Task<Game> GetById(int id)
        {
            var data = _dbContext.Games.FirstOrDefault(g => g.Id == id);
            return data;
        }

        public async Task<IEnumerable<Game>> GetRange(Expression<Func<Game, bool>> predicate)
        {
            var data = _dbContext.Games.Where(predicate);
            return data;
        }

        public async Task<IEnumerable<Game>> GetAll()
        {
            var data = _dbContext.Games;
            return data;
        }

        public async Task<MLGame> GetML(Expression<Func<Game, bool>> predicate)
        {
            var data = _dbContext.Games.FirstOrDefault(predicate);
            var mlgame = _MLGameAdapter.Adapt(data);
            return mlgame;
        }

        public async Task<MLGame> GetByIdML(int id)
        {
            var data = _dbContext.Games.FirstOrDefault(g => g.Id == id);
            var mlgame = _MLGameAdapter.Adapt(data);
            return mlgame;
        }

        public async Task<IEnumerable<MLGame>> GetRangeML(Expression<Func<Game, bool>> predicate)
        {
            var data = _dbContext.Games.Where(predicate).Where(g=>g.Result != null)
                .Include(g=>g.League)
                .Select(g=>_MLGameAdapter.Adapt(g));
            return data;
        }

        public async Task<IEnumerable<MLGame>> GetAllML()
        {
            var data = _dbContext.Games
                .Where(g => g.Result != null)
                .Include(g => g.League)
                .Include(g => g.Team1.HeadToHeadBase)
                .Include(g => g.Team1.HeadToHeadInGame)
                .Include(g => g.Team1.HeadToHeadInGameOpponent)
                .Include(g => g.Team2.HeadToHeadBase)
                .Include(g => g.Team2.HeadToHeadInGame)
                .Include(g => g.Team2.HeadToHeadInGameOpponent)
                .Select(g => _MLGameAdapter.Adapt(g));
            return data;
        }


        public async Task Remove(Expression<Func<Game, bool>> predicate)
        {
            var data = _dbContext.Games.FirstOrDefault(predicate);

            if (data != null)
            {
                _dbContext.Remove(data);
                _dbContext.SaveChanges();
            }
        }

        public async Task RemoveRange(Expression<Func<Game, bool>> predicate)
        {
            var data = _dbContext.Games.Where(predicate).ToList();

            if (data != null)
            {
                _dbContext.RemoveRange(data);
                _dbContext.SaveChanges();
            }
        }

        public async Task Add(Game game)
        {
            await _dbContext.AddAsync(game);
            _dbContext.SaveChanges();
        }

        public async Task Update(Game game)
        {
            var data = _dbContext.Games.FirstOrDefault(g => g.Url == game.Url);

            if (data != null)
            {
                _dbContext.Games.Attach(data);
                data = game;
                _dbContext.SaveChanges();
            }
        }

        public async Task UpdateOrAdd(Game game)
        {
            var data = _dbContext.Games.FirstOrDefault(g => g.Url == game.Url);

            if (data != null)
            {
                _dbContext.Games.Attach(data);
                data = game;
            }
            else
            {
                await _dbContext.AddAsync(game);
            }

            _dbContext.SaveChanges();
        }

        public async Task UpdateResult(string url, float result)
        {
            var data = _dbContext.Games.FirstOrDefault(g => g.Url == url);
            data.Result = result;
            _dbContext.SaveChanges();
        }

    }
}
