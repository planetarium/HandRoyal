using GraphQL.AspNet.Attributes;
using GraphQL.AspNet.Controllers;
using Libplanet.Crypto;
using Libplanet.Node.Services;

namespace HandRoyal.Node.Explorer.Queries;

public sealed class QueryController(IBlockChainService blockChainService) : GraphController
{
    [QueryRoot("IsValidSessionId")]
    public bool IsValidSessionId(Address sessionId)
    {
        var address = sessionId;
        var blockChain = blockChainService.BlockChain;

        var worldState = blockChain.GetWorldState();
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
}
