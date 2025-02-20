using System.Diagnostics.CodeAnalysis;
using Bencodex;
using Bencodex.Types;
using Libplanet.Crypto;

namespace HandRoyal;

public interface IAccountContext
{
    bool IsReadOnly { get; }

    object this[Address address] { get; set; }

    bool TryGetObject<T>(Address address, [MaybeNullWhen(false)] out T value)
        where T : IBencodable;

    bool TryGetState<T>(Address address, [MaybeNullWhen(false)] out T value)
        where T : IValue;

    T GetState<T>(Address address, T fallback)
        where T : IValue;

    bool ContainsState(Address address);

    bool RemoveState(Address address);

    void SetObject(Address address, IBencodable obj)
    {
        if (IsReadOnly)
        {
            throw new InvalidOperationException("This context is read-only.");
        }

        this[address] = obj.Bencoded;
    }
}
