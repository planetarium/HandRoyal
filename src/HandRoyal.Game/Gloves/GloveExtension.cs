using Libplanet.Crypto;

namespace HandRoyal.Game.Gloves;

public static class GloveExtension
{
    // gloveAddress의 첫 바이트가 0이면 rock, 1이면 paper, 2면 scissors, 그렇지 않으면 special
    public static GloveType GetGloveType(this Address gloveAddress) =>
        gloveAddress.ToHex()[0] switch
        {
            '0' => GloveType.Rock,
            '1' => GloveType.Paper,
            '2' => GloveType.Scissors,
            _ => GloveType.Special,
        };
}
