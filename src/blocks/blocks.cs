using Allumeria.Blocks.BlockModels;

namespace EvesThunder.Blocks;

public class EveBlock : Block {
    public EveBlock(string name) : base(id(name)) {
        item.SetItemSprite(Keys.block_items(name));
    }
}

public static class EvesBlocks {
    public static readonly Block fulminite_shard_cluster =
        new EveBlock(nameof(fulminite_shard_cluster))
            .MakeNeedSupport()
            .MakeSemiSolid()
            .SetTexture(Keys.block(nameof(fulminite_shard_cluster)))
            .SetBlockModel(BlockModelQuads.small_plant)
            .SetDropItem(EvesItems.fulminite_shard)
            .SetCategory([ ItemCategory.natural ])
            .SetMaterial(BlockMaterial.glass)
            .SetLightEmission(0, 3, 5);


    internal static void init() {
        ItemArranger.add_before(fulminite_shard_cluster, Block.ice_cap);

        Logger.Warn("Possibilities:");
        Logger.Warn($"  '{fulminite_shard_cluster.item.itemTextureString}'");
        Logger.Warn($"  '{fulminite_shard_cluster.dropItem.itemTextureString}'");
    }
}
