using HarmonyLib;
using HarmonyLib.Tools;

using Ignitron.Aluminium.Assets;
using Ignitron.Aluminium.Assets.Providers;
using Ignitron.Aluminium.Events;
using Ignitron.Aluminium.Registries;
using Ignitron.Aluminium.Translation;

using EvesThunder.Blocks;
using EvesThunder.UI;

using Ignitron.Loader;

using AssetManager = Ignitron.Aluminium.Assets.AssetManager;
using Logger = Allumeria.Logger;

namespace EvesThunder;

public sealed class EvesThunderMod : IModEntrypoint {
    public const string MOD_ID = "evelyntsmg.EvesThunder";

    private AssetManager _assets = null!;

    public void Main(ModBox box) {
#if DEBUG
        HarmonyFileLog.Enabled = true;
#endif

        Harmony harmony = new("evelyntsmg.EvesThunder");
        harmony.PatchAll();

        // prepare your assets
        _assets = AssetManager.CreateDefault(box.RootPath, $"ignitron/{MOD_ID}");

        // register your sprites to load to the game atlases
        Allumeria.DataManagement.AssetLoading.AssetManager.blockAtlas.ScanDirectory(_assets, "textures/atlas/blocks", 16);
        Allumeria.DataManagement.AssetLoading.AssetManager.itemAtlas.ScanDirectory(_assets, "textures/atlas/items", 16);
        Allumeria.DataManagement.AssetLoading.AssetManager.blockAtlas.ScanDirectory(_assets, "textures/atlas/particles", 16);

        // register your base translation keys
        AluminiumRegistries.Translators.Register(box.Metadata.Id, new DefaultTranslator(_assets.Load("translations/keys.txt", TranslationAssetProvider.Default)));

        ContentRegistryEvents.Blocks += EvesBlocks.init;
        ContentRegistryEvents.Items  += EvesItems.init;
        EvesGuide.init();

        Logger.Init("Initialized Eve's Thunder!");
    }
}
