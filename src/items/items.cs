using Ignitron.Aluminium.Extensions;

namespace EvesThunder.Items;

public static class EvesItems {
    public static readonly Item fulminite_shard =
        new Item(id(nameof(fulminite_shard)))
            .SellValue(36);

    public static readonly Item fulminite_ingot =
        new Item(id(nameof(fulminite_ingot)))
            .SellValue(80);

    public static void init() {
        ItemArranger.add_before(fulminite_shard, Block.cobalt_ore);
        ItemArranger.add_after(fulminite_ingot, Item.forest_ingot);

        Logger.Info($"Expecting texture called '{fulminite_shard.itemTextureString}'");
        Logger.Info($"Expecting texture called '{fulminite_ingot.itemTextureString}'");
    }
}
