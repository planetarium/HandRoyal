using Bencodex;
using Bencodex.Types;
using static HandRoyal.BencodexUtility;

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

        PlayerIndex = ToInt32(list, 0);
        Type = ToEnum<MoveType>(list, 1);
    }

    public int PlayerIndex { get; set; }

    public MoveType Type { get; set; }

    public IValue Bencoded => new List(
        ToValue(PlayerIndex),
        ToValue(Type));
}
