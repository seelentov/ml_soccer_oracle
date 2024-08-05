
namespace WebApplication2.Services.Interfaces
{
    public interface ICachingChecker<T>
    {
        public Task<bool> CheckIsCached(string value);
        public Task AddCached(string value);
    }
}
