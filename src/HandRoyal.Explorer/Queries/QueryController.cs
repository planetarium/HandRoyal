using GraphQL.AspNet.Attributes;
using GraphQL.AspNet.Controllers;
using Libplanet.Action.State;
using Libplanet.Crypto;
using Libplanet.Node.Services;

namespace HandRoyal.Explorer.Queries;

internal sealed class QueryController(IBlockChainService blockChainService) : GraphController
{
    [QueryRoot("IsValidSessionId")]
    public bool IsValidSessionId(Address sessionId)
    {
        var address = sessionId;
        var blockChain = blockChainService.BlockChain;

        var worldState = blockChain.GetNextWorldState() ?? blockChain.GetWorldState();
        var currentSessionAccount = worldState.GetAccountState(Addresses.Sessions);
        if (currentSessionAccount.GetState(address) is not null)
        {
            return false;
        }

        return true;
    }

    [QueryRoot("NextTxNonce")]
    public long NextTxNonce(Address address)
    {
        var blockChain = blockChainService.BlockChain;
        return blockChain.GetNextTxNonce(address);
    }

    [QueryRoot("IsGloveRegistered")]
    public bool IsGloveRegistered(Address gloveId)
    {
        var blockChain = blockChainService.BlockChain;
        var worldState = blockChain.GetNextWorldState() ?? blockChain.GetWorldState();
        return worldState.GetAccountState(Addresses.Gloves).GetState(gloveId) is not null;
    }

    [QueryRoot("GetBalance")]
    public long GetBalance(Address address)
    {
        var blockChain = blockChainService.BlockChain;
        var worldState = blockChain.GetWorldState();
        return (long)worldState.GetBalance(address, Currencies.Royal).MajorUnit;
    }
}
