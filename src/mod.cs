using HarmonyLib;
using HarmonyLib.Tools;

using Ignitron.Aluminium.Assets;
using Ignitron.Aluminium.Assets.Providers;
using Ignitron.Aluminium.Events;
using Ignitron.Aluminium.Registries;
using Ignitron.Aluminium.Translation;

using EvesThunder.Blocks;
using EvesThunder.UI;

using Ignitron.Aluminium.Assets.IO;
using Ignitron.Loader;

using AssetManager = Ignitron.Aluminium.Assets.AssetManager;
using AllumAssetManager = Allumeria.DataManagement.AssetLoading.AssetManager;
using Logger = Allumeria.Logger;

namespace EvesThunder;

public sealed class EvesThunderMod : IModEntrypoint {
    private readonly struct SpriteAssetPredicate(string rootDirectory) : IAssetPredicate {
        public bool Invoke(string path, string relpath) {
            bool starts_with = relpath.StartsWith(rootDirectory);
            bool is_png = relpath.EndsWith(".png");
            Logger.Info($"    Checking {relpath} vs {rootDirectory}: {starts_with}, {is_png}");
            return starts_with && is_png;
        }
    }


    public const string MOD_ID = "evelyntsmg.EvesThunder";

    private AssetManager _assets = null!;

    public void Main(ModBox box) {
#if DEBUG
        HarmonyFileLog.Enabled = true;
#endif

        Harmony harmony = new("evelyntsmg.EvesThunder");
        harmony.PatchAll();

        _assets = AssetManager.CreateDefault(box.RootPath, $"res/ignitron/{MOD_ID}");

        Logger.Info("Finding assets...");
        int found = 0;
        foreach (IAsset asset in _assets.FileSystem.EnumerateAssets(new SpriteAssetPredicate(box.RootPath))) {
            Logger.Info($"  Found asset at '{asset.Path}'");
            found++;
        }
        Logger.Info($"Found {found} assets!");

        AllumAssetManager.blockAtlas.ScanDirectory(_assets, "textures/atlas/blocks", 16);
        AllumAssetManager.itemAtlas.ScanDirectory(_assets, "textures/atlas/items", 16);
        AllumAssetManager.blockAtlas.ScanDirectory(_assets, "textures/atlas/particles", 16);

        AluminiumRegistries.Translators.Register(
            box.Metadata.Id,
            new DefaultTranslator(
                _assets.Load("translations/keys.txt",
                TranslationAssetProvider.Default)
            )
        );

        ContentRegistryEvents.Blocks += EvesBlocks.init;
        ContentRegistryEvents.Items  += EvesItems.init;
        EvesGuide.init();

        Logger.Init("Initialized Eve's Thunder!");
    }
}
