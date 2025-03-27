using System.Collections.Immutable;
using Bencodex;
using GraphQL.AspNet.Attributes;
using GraphQL.AspNet.Controllers;
using HandRoyal.Actions;
using HandRoyal.Explorer.Types;
using Libplanet.Action;
using Libplanet.Crypto;

namespace HandRoyal.Explorer.Queries;

internal sealed class ActionQueryController : GraphController
{
    private static readonly Codec _codec = new();

    [Query("CreateUser")]
    public HexValue CreateUser(string name)
    {
        var createUser = new CreateUser
        {
            Name = name,
        };
        return Encode(createUser);
    }

    [Query("CreateSession")]
    public HexValue CreateSession(
        Address sessionId,
        Address prize,
        int maximumUser,
        int minimumUser,
        int remainingUser,
        long startAfter,
        int maxRounds,
        long roundLength,
        long roundInterval,
        int initialHealthPoint,
        IEnumerable<Address> users)
    {
        var createSession = new CreateSession
        {
            SessionId = sessionId,
            Prize = prize,
            MaximumUser = maximumUser,
            MinimumUser = minimumUser,
            RemainingUser = remainingUser,
            StartAfter = startAfter,
            MaxRounds = maxRounds,
            RoundLength = roundLength,
            RoundInterval = roundInterval,
            InitialHealthPoint = initialHealthPoint,
            Users = [.. users],
        };
        return Encode(createSession);
    }

    [Query("JoinSession")]
    public HexValue JoinSession(Address sessionId, IEnumerable<Address> gloves)
    {
        var joinSession = new JoinSession
        {
            SessionId = sessionId,
            Gloves = gloves.ToImmutableArray(),
        };
        return Encode(joinSession);
    }

    [Query("RegisterGlove")]
    public HexValue RegisterGlove(Address gloveId)
    {
        var registerGlove = new RegisterGlove
        {
            Id = gloveId,
        };
        return Encode(registerGlove);
    }

    [Query("SubmitMove")]
    public HexValue SubmitMove(Address sessionId, int gloveIndex)
    {
        var submitMove = new SubmitMove
        {
            SessionId = sessionId,
            GloveIndex = gloveIndex,
        };
        return Encode(submitMove);
    }

    [Query("PickUp")]
    public HexValue PickUp()
    {
        var pickUp = new PickUp();
        return Encode(pickUp);
    }

    [Query("PickUpMany")]
    public HexValue PickUpMany()
    {
        var pickUpMany = new PickUpMany();
        return Encode(pickUpMany);
    }

    private static HexValue Encode(IAction action) => _codec.Encode(action.PlainValue);
}
