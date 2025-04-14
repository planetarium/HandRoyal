using System.Collections.Immutable;
using System.Text;
using Bencodex;
using GraphQL.AspNet.Attributes;
using GraphQL.AspNet.Controllers;
using HandRoyal.Actions;
using HandRoyal.Explorer.Jwt;
using HandRoyal.Wallet.Interfaces;
using Libplanet.Action;
using Libplanet.Crypto;
using Libplanet.Node.Services;
using Libplanet.Types.Tx;
using Microsoft.AspNetCore.Http;

namespace HandRoyal.Explorer.Mutations;

internal sealed class ActionController(
    IBlockChainService blockChainService,
    IWalletService walletService,
    IHttpContextAccessor httpContextAccessor,
    JwtValidator jwtValidator) : GraphController
{
    public async Task<TxId> StageAction(IAction action)
    {
        if (!httpContextAccessor.IsValidToken(jwtValidator))
        {
            throw new UnauthorizedAccessException("Invalid or missing authentication token");
        }

        var userId = httpContextAccessor.UserId();
        var address = await walletService.GetAddressAsync(userId);
        var blockChain = blockChainService.BlockChain;
        var nonce = blockChain.GetNextTxNonce(address);
        var invoice = new TxInvoice(
            genesisHash: blockChain.Genesis.Hash,
            actions: new TxActionList([action.PlainValue]),
            gasLimit: null,
            maxGasPrice: null);
        var metaData = new TxSigningMetadata(address, nonce);
        var unsignedTx = new UnsignedTx(invoice, metaData);
        var payload = Encoding.UTF8.GetBytes(unsignedTx.SerializeUnsignedTx());
        var signature = await walletService.Sign(userId, payload);
        var signedTx = new Transaction(unsignedTx, [..signature]);

        blockChain.StageTransaction(signedTx);
        return signedTx.Id;
    }

    [MutationRoot("CreateUserByWallet")]
    public async Task<TxId> CreateUser(string name)
    {
        var userId = httpContextAccessor.UserId();
        try
        {
            await walletService.GetAddressAsync(userId);
        }
        catch (KeyNotFoundException)
        {
            await walletService.CreateWalletAsync(userId);
        }

        var createUser = new CreateUser
        {
            Name = name,
        };

        return await StageAction(createUser);
    }

    [MutationRoot("CreateSessionByWallet")]
    public async Task<TxId> CreateSession(
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
        int numberOfActiveGloves,
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
            NumberOfInitialGloves = numberOfInitialGloves,
            NumberOfActiveGloves = numberOfActiveGloves,
            Users = [.. users],
        };

        return await StageAction(createSession);
    }

    [MutationRoot("RegisterGloveByWallet")]
    public async Task<TxId> RegisterGlove(Address gloveId)
    {
        var registerGlove = new RegisterGlove
        {
            Id = gloveId,
        };

        return await StageAction(registerGlove);
    }

    [MutationRoot("JoinSessionByWallet")]
    public async Task<TxId> JoinSession(Address sessionId, IEnumerable<Address> gloves)
    {
        var joinSession = new JoinSession
        {
            SessionId = sessionId,
            Gloves = gloves.ToImmutableArray(),
        };

        return await StageAction(joinSession);
    }

    [MutationRoot("SubmitMoveByWallet")]
    public async Task<TxId> SubmitMove(Address sessionId, int gloveIndex)
    {
        var submitMove = new SubmitMove
        {
            SessionId = sessionId,
            GloveIndex = gloveIndex,
        };

        return await StageAction(submitMove);
    }

    [MutationRoot("PickUpByWallet")]
    public async Task<TxId> PickUp()
    {
        var pickUp = new PickUp();

        return await StageAction(pickUp);
    }

    [MutationRoot("PickUpManyByWallet")]
    public async Task<TxId> PickUpMany()
    {
        var pickUpMany = new PickUpMany();

        return await StageAction(pickUpMany);
    }

    [MutationRoot("RegisterMatchingByWallet")]
    public async Task<TxId> RegisterMatching(IEnumerable<Address> gloves)
    {
        var registerMatching = new RegisterMatching { Gloves = [.. gloves] };

        return await StageAction(registerMatching);
    }

    [MutationRoot("CancelMatchingByWallet")]
    public async Task<TxId> CancelMatching()
    {
        var cancelMatching = new CancelMatching();

        return await StageAction(cancelMatching);
    }

    [MutationRoot("RefillActionPointByWallet")]
    public async Task<TxId> RefillActionPoint()
    {
        var refillActionPoint = new RefillActionPoint();

        return await StageAction(refillActionPoint);
    }
}
