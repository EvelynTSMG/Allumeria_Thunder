namespace EvesThunder.Blocks;

public static class EvesBlocks {
    public static readonly Block fulminite_shard_block =
        new Block(id(nameof(fulminite_shard_block)))
            .MakeNeedSupport()
            .MakeSemiSolid()
            .SetTexture(id(nameof(fulminite_shard_block)))
            .SetDropItem(EvesItems.fulminite_shard)
            .SetMaterial(BlockMaterial.glass)
            .SetLightEmission(0, 3, 5);

    internal static void init() {
        //ItemArranger.add_before(fulminite_shard_block, Block.cobalt_ore);
    }
}
