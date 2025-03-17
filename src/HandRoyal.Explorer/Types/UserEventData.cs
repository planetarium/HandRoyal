using GraphQL.AspNet.Attributes;
using HandRoyal.States;
using Libplanet.Crypto;

namespace HandRoyal.Explorer.Types
{
    internal sealed class UserEventData(User user)
    {
        [GraphSkip]
        public User User => user;

        public Address Id => user.Id;

        public Address[] RegisteredGloves => user.RegisteredGloves.ToArray();

        public Dictionary<Address, int> OwnedGloves
            => User.OwnedGloves.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        public Address EquippedGlove => user.EquippedGlove;
    }
}
