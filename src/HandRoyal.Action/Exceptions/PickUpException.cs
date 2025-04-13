namespace HandRoyal.Exceptions;

public class PickUpException : Exception
{
    public PickUpException()
    {
    }

    public PickUpException(string message)
        : base(message)
    {
    }

    public PickUpException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
