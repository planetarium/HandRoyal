using GraphQL.AspNet.Attributes;
using GraphQL.AspNet.Controllers;
using Libplanet.Crypto;
using Libplanet.Node.Services;

namespace HandRoyal.Node.Explorer;

public class BakeryController(IBlockChainService blockChainService) : GraphController
{
    [QueryRoot("IsValidSessionId")]
    public bool IsValidSessionId(string sessionId)
    {
        var address = new Address(sessionId);
        var blockChain = blockChainService.BlockChain;

        var worldState = blockChain.GetWorldState();
        var currentSessionAccount = worldState.GetAccountState(Addresses.CurrentSession);
        if (currentSessionAccount.GetState(address) is not null)
        {
            return false;
        }

        var archivedSessionsAccount = worldState.GetAccountState(Addresses.ArchivedSessions);
        if (archivedSessionsAccount.GetState(address) is not null)
        {
            return false;
        }

        return true;
    }
}
