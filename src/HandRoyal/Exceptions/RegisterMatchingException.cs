namespace HandRoyal.Exceptions;

public class RegisterMatchingException : Exception
{
    public RegisterMatchingException()
    {
    }

    public RegisterMatchingException(string message)
        : base(message)
    {
    }

    public RegisterMatchingException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
