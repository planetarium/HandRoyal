using System.Collections.Immutable;
using System.Text;
using GraphQL.AspNet.Attributes;
using GraphQL.AspNet.Controllers;
using HandRoyal.Actions;
using HandRoyal.Explorer.Types;
using Libplanet.Crypto;
using Libplanet.Node.Services;
using Libplanet.Types.Tx;

namespace HandRoyal.Explorer.Mutations;

internal sealed class MutationController(IBlockChainService blockChainService) : GraphController
{
    [MutationRoot("CreateUser")]
    public TxId CreateUser(PrivateKey privateKey, string name)
    {
        var createUser = new CreateUser
        {
            Name = name,
        };
        var txSettings = new TxSettings
        {
            PrivateKey = privateKey,
            Actions = [createUser],
        };
        return txSettings.StageTo(blockChainService.BlockChain);
    }

    [MutationRoot("CreateSession")]
    public TxId CreateSession(
        PrivateKey privateKey,
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
        int numberOfInitialGloves,
        int numberOfActiveGloves)
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
            NumberOfInitialGloves = numberOfInitialGloves,
            NumberOfActiveGloves = numberOfActiveGloves,
        };
        var txSettings = new TxSettings
        {
            PrivateKey = privateKey,
            Actions = [createSession],
        };
        return txSettings.StageTo(blockChainService.BlockChain);
    }

    [MutationRoot("RegisterGlove")]
    public TxId RegisterGlove(PrivateKey privateKey, Address gloveId)
    {
        var registerGlove = new RegisterGlove
        {
            Id = gloveId,
        };
        var txSettings = new TxSettings
        {
            PrivateKey = privateKey,
            Actions = [registerGlove],
        };
        return txSettings.StageTo(blockChainService.BlockChain);
    }

    [MutationRoot("JoinSession")]
    public TxId JoinSession(PrivateKey privateKey, Address sessionId, IEnumerable<Address> gloves)
    {
        var joinSession = new JoinSession
        {
            SessionId = sessionId,
            Gloves = gloves.ToImmutableArray(),
        };
        var txSettings = new TxSettings
        {
            PrivateKey = privateKey,
            Actions = [joinSession],
        };
        return txSettings.StageTo(blockChainService.BlockChain);
    }

    [MutationRoot("SubmitMove")]
    public TxId SubmitMove(PrivateKey privateKey, Address sessionId, int gloveIndex)
    {
        var submitMove = new SubmitMove
        {
            SessionId = sessionId,
            GloveIndex = gloveIndex,
        };
        var txSettings = new TxSettings
        {
            PrivateKey = privateKey,
            Actions = [submitMove],
        };
        return txSettings.StageTo(blockChainService.BlockChain);
    }

    [MutationRoot("PickUp")]
    public TxId PickUp(PrivateKey privateKey)
    {
        var pickUp = new PickUp();
        var txSettings = new TxSettings
        {
            PrivateKey = privateKey,
            Actions = [pickUp],
        };
        return txSettings.StageTo(blockChainService.BlockChain);
    }

    [MutationRoot("PickUpMany")]
    public TxId PickUpMany(PrivateKey privateKey)
    {
        var pickUpMany = new PickUpMany();
        var txSettings = new TxSettings
        {
            PrivateKey = privateKey,
            Actions = [pickUpMany],
        };
        return txSettings.StageTo(blockChainService.BlockChain);
    }

    [MutationRoot("RegisterMatching")]
    public TxId RegisterMatching(PrivateKey privateKey, IEnumerable<Address> gloves)
    {
        var registerMatching = new RegisterMatching { Gloves = [.. gloves] };
        var txSettings = new TxSettings
        {
            PrivateKey = privateKey,
            Actions = [registerMatching],
        };
        return txSettings.StageTo(blockChainService.BlockChain);
    }

    [MutationRoot("CancelMatching")]
    public TxId CancelMatching(PrivateKey privateKey)
    {
        var cancelMatching = new CancelMatching();
        var txSettings = new TxSettings
        {
            PrivateKey = privateKey,
            Actions = [cancelMatching],
        };
        return txSettings.StageTo(blockChainService.BlockChain);
    }

    [MutationRoot("RefillActionPoint")]
    public TxId RefillActionPoint(PrivateKey privateKey)
    {
        var refillActionPoint = new RefillActionPoint();
        var txSettings = new TxSettings
        {
            PrivateKey = privateKey,
            Actions = [refillActionPoint],
        };
        return txSettings.StageTo(blockChainService.BlockChain);
    }

    [MutationRoot("StageTransaction")]
    public TxId StageTransaction(HexValue unsignedTransaction, HexValue signature)
    {
        var json = Encoding.UTF8.GetString((byte[])unsignedTransaction);
        var unsignedTx = TxMarshaler.DeserializeUnsignedTx(json);
        var tx = new Transaction(unsignedTx, signature.ToImmutableArray());
        var blockChain = blockChainService.BlockChain;
        if (!blockChain.StageTransaction(tx))
        {
            throw new InvalidOperationException("Failed to stage transaction.");
        }

        return tx.Id;
    }

    [MutationRoot("MintSinkAddress")]
    public TxId MintSinkAddress(PrivateKey privateKey, long amount)
    {
        if (!privateKey.Address.Equals(Currencies.SinkAddress))
        {
            throw new InvalidOperationException("Can sink only sink address");
        }

        var mintAsset = new MintAsset
        {
            Amount = amount,
        };
        var txSettings = new TxSettings
        {
            PrivateKey = privateKey,
            Actions = [mintAsset],
        };
        return txSettings.StageTo(blockChainService.BlockChain);
    }
}
