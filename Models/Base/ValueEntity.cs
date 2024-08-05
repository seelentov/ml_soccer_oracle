namespace WebApplication2.Models.Base
{
    public abstract class ValueEntity<T>: BaseEntity
    {
        public T Value { get; set; }
    }
}
