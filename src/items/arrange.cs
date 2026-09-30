using System.Runtime.InteropServices;

using HarmonyLib;

namespace EvesThunder.Items;

[HarmonyPatch]
public static class ItemArranger {
    private static List<Item> _remove = [];
    private static Dictionary<Item, Item> _add_before = [];
    private static Dictionary<Item, Item> _add_after  = [];
    private static bool done;

    public static void add_before(Item item, Item before) {
        if (done)
            throw new NotSupportedException("Cannot add items after item array has been fitted.");

        _add_before.Add(item, before);
        Logger.Info($"Adding '{item.strID}' before '{before.strID}'.");
    }

    public static void add_after(Item item, Item after) {
        if (done)
            throw new NotSupportedException("Cannot add items after item array has been fitted.");

        _add_after.Add(item, after);
        Logger.Info($"Adding '{item.strID}' after '{after.strID}'.");
    }

    public static void add_before(Item item, Block before) => add_before(item, before.item);
    public static void add_after(Item item, Block after)   => add_after (item, after.item);

    public static void add_before(Block block, Item before) => add_before(block.item, before);
    public static void add_after(Block block, Item after)   => add_after (block.item, after);

    public static void add_before(Block block, Block before) => add_before(block.item, before.item);
    public static void add_after(Block block, Block after)   => add_after (block.item, after.item);



    [HarmonyPatch(typeof(Item), nameof(Item.AssignCategories))]
    public static class Item_AssignCategories {
        [HarmonyPostfix]
        private static void Postfix() {
            foreach (ItemCategory category in ItemCategory.categories) {
                foreach (Item item in _add_before.Keys) {
                    Item before = _add_before[item];

                    if (category.items.Remove(item))
                        category.items.AddBefore(item, before);
                }

                foreach (Item item in _add_after.Keys) {
                    Item after = _add_after[item];

                    if (category.items.Remove(item))
                        category.items.AddAfter(item, after);
                }
            }

            done = true;

            _add_before.Clear();
            _add_after.Clear();
        }
    }
}
