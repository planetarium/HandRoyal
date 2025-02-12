using Libplanet.Crypto;
using Libplanet.Types.Blocks;

namespace HandRoyal.Node.Explorer.Types;

public sealed class BlockHeaderValue(Block block)
{
    public long Height => block.Index;

    public BlockHash Id => block.Hash;

    public string Hash => block.Hash.ToString();

    public Address Miner => block.Miner;
}
