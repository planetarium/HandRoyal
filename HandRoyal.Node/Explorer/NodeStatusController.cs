using GraphQL.AspNet.Attributes;
using GraphQL.AspNet.Controllers;
using HandRoyal.Node.Explorer.Types;
using Libplanet.Node.Services;

namespace HandRoyal.Node.Explorer;

public sealed class NodeStatusController(IBlockChainService blockChainService) : GraphController
{
    [Query("Tip")]
    public BlockHeaderValue GetTip()
    {
        var blockChain = blockChainService.BlockChain;
        return new(blockChain.Tip);
    }
}
