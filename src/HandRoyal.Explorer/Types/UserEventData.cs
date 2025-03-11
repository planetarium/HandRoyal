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

        [GraphSkip]
        public Address[] OwnedGloves => user.OwnedGloves.ToArray();
    }
}
