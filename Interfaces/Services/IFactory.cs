namespace WebApplication2.Services.Interfaces
{
    public interface IFactory<T>
    {
        public T Get();
    }
}
