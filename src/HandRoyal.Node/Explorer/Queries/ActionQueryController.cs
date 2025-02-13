using Bencodex;
using GraphQL.AspNet.Attributes;
using GraphQL.AspNet.Controllers;
using HandRoyal.Actions;
using HandRoyal.Node.Explorer.Types;
using HandRoyal.States;
using Libplanet.Crypto;

namespace HandRoyal.Node.Explorer.Queries;

public sealed class ActionQueryController : GraphController
{
    private static readonly Codec _codec = new();

    [Query("CreateUser")]
    public HexValue CreateUser()
    {
        var createUser = new CreateUser();
        return _codec.Encode(createUser.PlainValue);
    }

    [Query("CreateSession")]
    public HexValue CreateSession(Address sessionId, Address prize)
    {
        var createSession = new CreateSession(
            sessionId: sessionId,
            prize: prize);
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
