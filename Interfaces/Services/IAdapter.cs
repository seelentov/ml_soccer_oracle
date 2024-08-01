namespace WebApplication2.Services.Interfaces
{
    public interface IAdapter<T, V>
    {
        public V Adapt(T oldData);
        public IEnumerable<V> AdaptRange(IEnumerable<T> oldData);
    }
}
