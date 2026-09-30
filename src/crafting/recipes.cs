namespace EvesThunder.Crafting;

public class EveRecipe : CraftingRecipe {
    public EveRecipe(
        Item item,
        int amount,
        CraftingStation? station
    ) : base(new ItemStack(item, amount), station ?? CraftingStation.inventory) {
        if (recipes.Contains(this)) {
            Logger.Info($"Added recipe for {amount}x {item.translatedName}");
        }
    }

    public EveRecipe AddReq(Item item, int amount) {
        base.AddReq(new RecipeEntry(item, amount));
        return this;
    }
}

public class EvesCraftingRecipes {
    public static void init() {
        // Fulminite Ingot
        new EveRecipe(EvesItems.fulminite_ingot, 1, CraftingStation.furnace)
            .AddReq(EvesItems.fulminite_shard, 4)
            .AddReq(Block.copper_ore.item, 2);
    }
}
