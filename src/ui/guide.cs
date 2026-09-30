using Allumeria.UI.Guide;

namespace EvesThunder.UI;

public class EvesGuide {
    public static readonly GuideTask visit_thunder;

    public static readonly GuideTask get_fulminite_shard;

    public static readonly GuideTask get_fulminite_ingot;

    static EvesGuide() {
        visit_thunder =
            new GuideTask(
                    id(nameof(visit_thunder)),
                    17, -4,
                    Block.gravel.item,
                    dontUseItem: true,
                    autoUnlock: true,
                    unlockPrevious: false
                )
                .AddPrerequisite(GuideTask.silver_armour);

        get_fulminite_shard =
            new GuideTask(id(nameof(get_fulminite_shard)), 15, -8, EvesItems.fulminite_shard)
                .AddPrerequisite(visit_thunder);

        get_fulminite_ingot =
            new GuideTask(id(nameof(get_fulminite_ingot)), 15, -11, EvesItems.fulminite_ingot)
                .AddPrerequisite(get_fulminite_shard);
    }

    internal static void init() { }
}
