using Bencodex;
using GraphQL.AspNet.Attributes;
using GraphQL.AspNet.Controllers;
using HandRoyal.Actions;
using HandRoyal.Explorer.Types;
using HandRoyal.States;
using Libplanet.Crypto;

namespace HandRoyal.Explorer.Queries;

internal sealed class ActionQueryController : GraphController
{
    private static readonly Codec _codec = new();

    [Query("CreateUser")]
    public HexValue CreateUser()
    {
        var createUser = new CreateUser();
        return _codec.Encode(createUser.PlainValue);
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
        var createSession = new CreateSession(
            sessionId: sessionId,
            prize: prize,
            maximumUser: maximumUser,
            minimumUser: minimumUser,
            remainingUser: remainingUser,
            roundInterval: roundInterval,
            waitingInterval: waitingInterval);
        return _codec.Encode(createSession.PlainValue);
    }

    [Query("JoinSession")]
    public HexValue JoinSession(Address sessionId, Address? gloveId)
    {
        var joinSession = new JoinSession(sessionId, gloveId);
        return _codec.Encode(joinSession.PlainValue);
    }

    [Query("RegisterGlove")]
    public HexValue RegisterGlove(Address gloveId)
    {
        var registerGlove = new RegisterGlove(gloveId);
        return _codec.Encode(registerGlove.PlainValue);
    }

    [Query("SubmitMove")]
    public HexValue SubmitMove(Address sessionId, MoveType move)
    {
        var submitMove = new SubmitMove(sessionId, move);
        return _codec.Encode(submitMove.PlainValue);
    }
}
