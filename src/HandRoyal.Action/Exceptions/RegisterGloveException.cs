namespace HandRoyal.Exceptions;

public class RegisterGloveException : Exception
{
    public RegisterGloveException()
    {
    }

    public RegisterGloveException(string message)
        : base(message)
    {
    }

    public RegisterGloveException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
