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

        public GloveInfo[] OwnedGloves => User.OwnedGloves.ToArray();

        public Address EquippedGlove => user.EquippedGlove;
    }
}
