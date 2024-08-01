namespace WebApplication2.Exceptions
{
    public class AdapterValidatorException : Exception
    {
        public AdapterValidatorException()
        {
        }

        public AdapterValidatorException(string message)
            : base(message)
        {
        }

        public AdapterValidatorException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
