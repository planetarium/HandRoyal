using GraphQL.AspNet.Attributes;
using GraphQL.AspNet.Controllers;
using GraphQL.AspNet.Interfaces.Controllers;
using HandRoyal.Explorer.Types;
using Libplanet.Crypto;
using Libplanet.Node.Services;
using Libplanet.Types.Tx;

namespace HandRoyal.Explorer.Subscriptions;

internal sealed class SubscriptionController(IBlockChainService blockChainService) : GraphController
{
    public const string TipChangedEventName = "TIP_CHANGED";
    public const string MoveChangedEventName = "MOVE_CHANGED";
    public const string SessionChangedEventName = "SESSION_CHANGED";
    public const string UserChangedEventName = "USER_CHANGED";
    public const string TransactionChangedEventName = "TRANSACTION_CHANGED";
    public const string GloveRegisteredEventName = "GLOVE_REGISTERED";
    public const string PickUpResultEventName = "PICKUP_RESULT";
    public const string MatchMadeEventName = "MATCH_MADE";
    public const string SessionResultChangedEventName = "SESSION_RESULT_CHANGED";

    [SubscriptionRoot("onTipChanged", typeof(TipEventData), EventName = TipChangedEventName)]
    public IGraphActionResult OnTipChanged(TipEventData eventData)
    {
        return Ok(eventData);
    }

    [SubscriptionRoot(
        "onMoveChanged", typeof(SubmitMoveEventData), EventName = MoveChangedEventName)]
    public IGraphActionResult OnMoveChanged(
        SubmitMoveEventData eventData, Address sessionId, Address userId)
    {
        if (eventData.SessionId == sessionId && eventData.UserId == userId)
        {
            return Ok(eventData);
        }

        return this.SkipSubscriptionEvent();
    }

    [SubscriptionRoot(
        "onSessionChanged", typeof(SessionEventData), EventName = SessionChangedEventName)]
    public IGraphActionResult OnSessionChanged(
        SessionEventData eventData, Address sessionId, Address userId)
    {
        var session = eventData.Session;
        if (session.Metadata.Id == sessionId)
        {
            eventData.UserId = userId;
            return Ok(eventData);
        }

        return this.SkipSubscriptionEvent();
    }

    [SubscriptionRoot(
        "onUserChanged", typeof(UserEventData), EventName = UserChangedEventName)]
    public IGraphActionResult OnUserChanged(UserEventData eventData, Address userId)
    {
        var user = eventData.User;
        if (user.Id == userId)
        {
            return Ok(eventData);
        }

        return this.SkipSubscriptionEvent();
    }

    [SubscriptionRoot(
        "onGloveRegistered",
        typeof(GloveRegisteredEventData),
        EventName = GloveRegisteredEventName)]
    public IGraphActionResult OnGloveRegistered(GloveRegisteredEventData eventData, Address gloveId)
    {
        var glove = eventData.Glove;
        if (glove.Id == gloveId)
        {
            return Ok(eventData);
        }

        return this.SkipSubscriptionEvent();
    }

    [SubscriptionRoot(
        "onTransactionChanged",
        typeof(TransactionEventData),
        EventName = TransactionChangedEventName)]
    public IGraphActionResult OnTransactionChanged(TransactionEventData eventData, TxId txId)
    {
        var blockChain = blockChainService.BlockChain;
        var blockHash = eventData.BlockHash;
        if (blockChain.GetTxExecution(blockHash, txId) is { } execution)
        {
            eventData.TxId = txId;
            eventData.Status = execution.Fail ? TxStatus.Failure : TxStatus.Success;
            eventData.BlockHash = execution.BlockHash;
            eventData.BlockHeight = blockChain[blockHash].Index;
            return this.OkAndComplete(eventData);
        }

        if (!blockChain.GetStagedTransactionIds().Contains(txId))
        {
            eventData.TxId = txId;
            eventData.Status = TxStatus.Invalid;
            return this.OkAndComplete(eventData);
        }

        eventData.TxId = txId;
        eventData.Status = TxStatus.Staging;
        return this.OkAndComplete(eventData);
    }

    [SubscriptionRoot(
        "onPickUpResult",
        typeof(PickUpResultEventData),
        EventName = PickUpResultEventName)]
    public IGraphActionResult OnPickUpResult(PickUpResultEventData eventData, TxId txId)
    {
        if (eventData.TxId == txId)
        {
            return this.OkAndComplete(eventData);
        }

        return this.SkipSubscriptionEvent();
    }

    [SubscriptionRoot(
        "onMatchMade",
        typeof(MatchMadeEventData),
        EventName = MatchMadeEventName)]
    public IGraphActionResult OnMatchMade(MatchMadeEventData eventData, Address userId)
    {
        if (eventData.Players.Any(player => player.Equals(userId)))
        {
            return this.OkAndComplete(eventData);
        }

        return this.SkipSubscriptionEvent();
    }

    [SubscriptionRoot(
        "onSessionResultChanged",
        typeof(SessionResultEventData),
        EventName = SessionResultChangedEventName)]
    public IGraphActionResult OnSessionResultChanged(SessionResultEventData eventData)
    {
        return Ok(eventData);
    }
}
