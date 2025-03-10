using Bencodex;
using GraphQL.AspNet.Attributes;
using GraphQL.AspNet.Controllers;
using HandRoyal.Actions;
using HandRoyal.Explorer.Types;
using HandRoyal.States;
using Libplanet.Action;
using Libplanet.Crypto;

namespace HandRoyal.Explorer.Queries;

internal sealed class ActionQueryController : GraphController
{
    private static readonly Codec _codec = new();

    [Query("CreateUser")]
    public HexValue CreateUser()
    {
        var createUser = new CreateUser();
        return Encode(createUser);
    }

    [Query("CreateSession")]
    public HexValue CreateSession(
        Address sessionId,
        Address prize,
        int maximumUser,
        int minimumUser,
        int remainingUser,
        long roundInterval,
        long waitingInterval)
    {
        var createSession = new CreateSession
        {
            SessionId = sessionId,
            Prize = prize,
            MaximumUser = maximumUser,
            MinimumUser = minimumUser,
            RemainingUser = remainingUser,
            RoundInterval = roundInterval,
            WaitingInterval = waitingInterval,
        };
        return Encode(createSession);
    }

    [Query("JoinSession")]
    public HexValue JoinSession(Address sessionId)
    {
        var joinSession = new JoinSession
        {
            SessionId = sessionId,
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
    public HexValue SubmitMove(Address sessionId, MoveType move)
    {
        var submitMove = new SubmitMove
        {
            SessionId = sessionId,
            Move = move,
        };
        return Encode(submitMove);
    }

    [Query("EquipGlove")]
    public HexValue EquipGlove(Address? gloveId)
    {
        var equipGlove = new EquipGlove
        {
            Glove = gloveId ?? default,
        };
        return Encode(equipGlove);
    }

    private static HexValue Encode(IAction action) => _codec.Encode(action.PlainValue);
}
