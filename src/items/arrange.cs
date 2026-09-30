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

        _add_before.Add(before, item);
        Logger.Info($"Adding {item.strID} before {before.strID}");
    }

    public static void add_after(Item item, Item after) {
        if (done)
            throw new NotSupportedException("Cannot add items after item array has been fitted.");

        _add_after.Add(after, item);
        Logger.Info($"Adding {item.strID} after {after.strID}");
    }

    public static void add_before(Item item, Block before) {
        if (done)
            throw new NotSupportedException("Cannot add items after item array has been fitted.");

        _add_before.Add(before.item, item);
        Logger.Info($"Adding {item.strID} before {before.item.strID}");
    }

    public static void add_after(Item item, Block after) {
        if (done)
            throw new NotSupportedException("Cannot add items after item array has been fitted.");

        _add_after.Add(after.item, item);
        Logger.Info($"Adding {item.strID} after {after.item.strID}");
    }

    public static void add_before(Block block, Item before) {
        if (done)
            throw new NotSupportedException("Cannot add items after item array has been fitted.");

        _add_before.Add(before, block.item);
        Logger.Info($"Adding {block.item.strID} before {before.strID}");
    }

    public static void add_after(Block block, Item after) {
        if (done)
            throw new NotSupportedException("Cannot add items after item array has been fitted.");

        _add_after.Add(after, block.item);
        Logger.Info($"Adding {block.item.strID} after {after.strID}");
    }

    public static void add_before(Block block, Block before) {
        if (done)
            throw new NotSupportedException("Cannot add items after item array has been fitted.");

        _add_before.Add(before.item, block.item);
        Logger.Info($"Adding {block.item.strID} before {before.item.strID}");
    }

    public static void add_after(Block block, Block after) {
        if (done)
            throw new NotSupportedException("Cannot add items after item array has been fitted.");

        _add_after.Add(after.item, block.item);
        Logger.Info($"Adding {block.item.strID} after {after.item.strID}");
    }

    [HarmonyPatch(typeof(Item), nameof(Item.FitItemArray))]
    public static class Item_FitItemArray {
        [HarmonyPrefix]
        private static void Prefix() {
            List<Item> items = [ ..Item.items ];

            items.RemoveAll(item => _remove.Contains(item));

            foreach (Item before in _add_before.Keys) {
                Item item = _add_before[before];

                // First remove the item to avoid duplicates.
                // This fails silently if the item is not already in the array.
                items.Remove(item);

                if (!items.AddBefore(item, before)) {
                    Logger.Warn($"Tried to add item {item.strID} before {before.strID}, but {before.strID} is not in item array.");
                }
            }

            foreach (Item after in _add_after.Keys) {
                Item item = _add_after[after];

                // First remove the item to avoid duplicates.
                // This fails silently if the item is not already in the array.
                items.Remove(item);

                if (!items.AddAfter(item, after)) {
                    Logger.Warn($"Tried to add item {item.strID} after {after.strID}, but {after.strID} is not in item array.");
                }
            }

            Item.items = [ ..items ];

            done = true;
        }

        [HarmonyPostfix]
        private static void Postfix() {
            // Now that we've gone and changed the array, the IDs are all wrong!
            for (int i = 0; i < Item.items.Length; i++) {
                Item.items[i].itemID = i;
            }
        }
    }

    [HarmonyPatch(typeof(Item), nameof(Item.AssignCategories))]
    public static class Item_AssignCategories {
        [HarmonyPostfix]
        private static void Postfix() {
            ItemCategory[] dirty_categories = [
                ItemCategory.all,
                ItemCategory.blocks,
                ItemCategory.nonblocks,
                ItemCategory.weapons,
                ItemCategory.tools,
            ];

            foreach (ItemCategory category in dirty_categories) {
                foreach (Item before in _add_before.Keys) {
                    Item item = _add_before[before];

                    if (category.items.Remove(item))
                        category.items.AddBefore(item, before);
                }

                foreach (Item after in _add_after.Keys) {
                    Item item = _add_after[after];

                    if (category.items.Remove(item))
                        category.items.AddAfter(item, after);
                }
            }
        }
    }
}
