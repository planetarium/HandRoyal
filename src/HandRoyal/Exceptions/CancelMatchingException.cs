namespace HandRoyal.Exceptions;

public class CancelMatchingException : Exception
{
    public CancelMatchingException()
    {
    }

    public CancelMatchingException(string message)
        : base(message)
    {
    }

    public CancelMatchingException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
