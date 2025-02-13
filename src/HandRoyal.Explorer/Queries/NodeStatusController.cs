using GraphQL.AspNet.Attributes;
using GraphQL.AspNet.Controllers;
using HandRoyal.Explorer.Types;
using Libplanet.Node.Services;

namespace HandRoyal.Explorer.Queries;

internal sealed class NodeStatusController(IBlockChainService blockChainService) : GraphController
{
    [Query("Tip")]
    public BlockHeaderValue GetTip()
    {
        var blockChain = blockChainService.BlockChain;
        return new(blockChain.Tip);
    }
}
