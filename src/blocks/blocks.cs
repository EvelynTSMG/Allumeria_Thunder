namespace EvesThunder.Blocks;

public static class EvesBlocks {
    public static readonly Block fulminite_shard_block =
        new Block(id(nameof(fulminite_shard_block)))
            .MakeNeedSupport()
            .MakeSemiSolid()
            .SetTexture(id(nameof(fulminite_shard_block)))
            .SetDropItem(EvesItems.fulminite_shard)
            .SetCategory([ ItemCategory.natural ])
            .SetMaterial(BlockMaterial.glass)
            .SetLightEmission(0, 3, 5);

    internal static void init() {
        ItemArranger.add_before(fulminite_shard_block, Block.ice_cap);

        Logger.Info($"Expecting texture called '{fulminite_shard_block.textureStrings[0]}'");
    }
}
