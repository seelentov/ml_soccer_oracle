namespace WebApplication2.Models.Base
{
    public abstract class KeyValueEntity<T,V>: ValueEntity<T>
    {
        public V Key { get; set; }
    }
}
