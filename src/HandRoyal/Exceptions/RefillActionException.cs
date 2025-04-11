namespace HandRoyal.Exceptions;

public class RefillActionException : Exception
{
    public RefillActionException()
    {
    }

    public RefillActionException(string message)
        : base(message)
    {
    }

    public RefillActionException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
