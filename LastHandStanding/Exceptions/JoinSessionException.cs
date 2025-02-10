namespace LastHandStanding.Exceptions;

public class JoinSessionException : Exception
{
    public JoinSessionException()
    {
    }

    public JoinSessionException(string message)
        : base(message)
    {
    }

    public JoinSessionException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
