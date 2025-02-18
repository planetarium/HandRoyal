using System.Diagnostics.CodeAnalysis;
using Bencodex;
using Bencodex.Types;
using Libplanet.Crypto;

namespace HandRoyal;

public interface IAccountContext
{
    IValue this[Address address] { get; set; }

    bool TryGetObject<T>(Address address, [MaybeNullWhen(false)] out T value)
        where T : IBencodable;

    bool TryGetState<T>(Address address, [MaybeNullWhen(false)] out T value)
        where T : IValue;

    T GetState<T>(Address address, T fallback)
        where T : IValue;

    bool Contains(Address address);

    bool Remove(Address address);
}
