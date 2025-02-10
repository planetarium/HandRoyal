namespace HandRoyal.Exceptions;

public class CreateSessionException : Exception
{
    public CreateSessionException()
    {
    }

    public CreateSessionException(string message)
        : base(message)
    {
    }

    public CreateSessionException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
