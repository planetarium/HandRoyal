namespace HandRoyal.Exceptions;

public class MintAssetException : Exception
{
    public MintAssetException()
    {
    }

    public MintAssetException(string message)
        : base(message)
    {
    }

    public MintAssetException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
