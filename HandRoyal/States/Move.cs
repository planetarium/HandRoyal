using Bencodex;
using Bencodex.Types;

namespace HandRoyal.States;

public sealed record class Move : IBencodable
{
    public Move()
    {
    }

    public Move(IValue value)
    {
        if (value is not List list)
        {
            throw new ArgumentException($"Given value {value} is not a list.");
        }

        PlayerIndex = (Integer)list[0];
        Type = (MoveType)(int)(Integer)list[1];
    }

    public int PlayerIndex { get; set; }

    public MoveType Type { get; set; }

    public IValue Bencoded => new List(
        (Integer)PlayerIndex,
        (Integer)(int)Type);
}
