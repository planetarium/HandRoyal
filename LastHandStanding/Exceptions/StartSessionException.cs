namespace LastHandStanding.Exceptions;

public class StartSessionException : Exception
{
    public StartSessionException()
    {
    }
    
    public StartSessionException(string message)
        : base(message)
    {
    }
    
    public StartSessionException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}