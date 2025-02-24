namespace HandRoyal.Serialization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class ModelAttribute(int version) : Attribute
{
    public int Version { get; } = version;
}
