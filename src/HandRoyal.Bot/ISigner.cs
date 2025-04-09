namespace HandRoyal.Bot;

public interface ISigner
{
    byte[] Sign(byte[] message);

    bool Verify(byte[] message, byte[] signature);
}
