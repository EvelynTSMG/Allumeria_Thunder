using Allumeria.Biomes;
using Allumeria.Biomes.Generators;
using Allumeria.Blocks.Blocks;
using Allumeria.Blocks.Fluids;
using Allumeria.ChunkManagement;
using Allumeria.ChunkManagement.TerrainFeatures;
using Allumeria.EntitySystem.Entities;
using Allumeria.Items;

using EvesThunder.Effects;

namespace EvesThunder.Biomes;

public class ThunderBiome : WorldBiome {
    public static Atmosphere thunder_atmosphere = new() {
        day = new AtmosphereColourGroup {
            skyColor     = new Vector4(0.11f , 0.184f, 0.678f, 1.0f),
            horizonColor = new Vector4(0.529f, 0.573f, 0.808f, 1.0f),
            ambientColor = new Vector4(0.663f, 0.729f, 0.8f  , 1.0f),
            cloudColor   = new Vector4(0.408f, 0.455f, 0.553f, 1.0f),
        },
        sunrise = new AtmosphereColourGroup {
            skyColor     = new Vector4(0.039f, 0.22f , 0.298f, 1.0f),
            horizonColor = new Vector4(0.871f, 0.647f, 0.086f, 1.0f),
            ambientColor = new Vector4(0.208f, 0.161f, 0.082f, 1.0f),
            cloudColor   = new Vector4(0.871f, 0.647f, 0.086f, 1.0f),
        },
        night = new AtmosphereColourGroup {
            skyColor     = new Vector4(0.0f  , 0.0f  , 0.0f  , 1.0f),
            horizonColor = new Vector4(0.098f, 0.063f, 0.22f , 1.0f),
            ambientColor = new Vector4(0.098f, 0.063f, 0.22f , 1.0f),
            cloudColor   = new Vector4(0.098f, 0.063f, 0.22f , 1.0f),
        },
        fogDensity = 0.7f,
    };

    public static Atmosphere thunderstorm_atmosphere = new() {
        day = new AtmosphereColourGroup {
            skyColor     = new Vector4(0.122f, 0.153f, 0.294f, 1.0f),
            horizonColor = new Vector4(0.357f, 0.396f, 0.545f, 1.0f),
            ambientColor = new Vector4(0.325f, 0.325f, 0.408f, 1.0f),
            cloudColor   = new Vector4(0.224f, 0.227f, 0.318f, 1.0f),
        },
        sunrise = new AtmosphereColourGroup {
            skyColor     = new Vector4(0.059f, 0.176f, 0.275f, 1.0f),
            horizonColor = new Vector4(0.871f, 0.545f, 0.086f, 1.0f),
            ambientColor = new Vector4(0.208f, 0.129f, 0.086f, 1.0f),
            cloudColor   = new Vector4(0.871f, 0.545f, 0.086f, 1.0f),
        },
        night = new AtmosphereColourGroup {
            skyColor     = new Vector4(0.0f  , 0.0f  , 0.0f  , 1.0f),
            horizonColor = new Vector4(0.043f, 0.039f, 0.094f, 1.0f),
            ambientColor = new Vector4(0.043f, 0.039f, 0.094f, 1.0f),
            cloudColor   = new Vector4(0.043f, 0.039f, 0.094f, 1.0f),
        },
        fogDensity = 0.35f,
    };

    public static readonly ThunderBiome thunder =
        new(++IdHelper.biome_id, id(nameof(thunder)), thunder_atmosphere);

    public ThunderBiome(byte id, string name, Atmosphere atmosphere)
        : base(id, name, atmosphere) { }

    public override void OnPlayerInsideTick(PlayerEntity player) {
        ItemStack? helmet = player.inventory.inventory.GetItemInSlot(72);
        ItemStack? chest  = player.inventory.inventory.GetItemInSlot(73);

        bool has_metal_helmet = helmet is not null && helmet.item.strID switch {
            nameof(Item.copper_helmet)    => true,
            nameof(Item.iron_helmet)      => true,
            nameof(Item.silver_helmet)    => true,
            nameof(Item.gold_helmet)      => true,
            nameof(Item.cobalt_helmet)    => true,
            nameof(Item.palladium_helmet) => true,
            _ => false,
        };

        bool has_metal_chest = chest is not null && chest.item.strID switch {
            nameof(Item.copper_chestplate)    => true,
            nameof(Item.iron_chestplate)      => true,
            nameof(Item.silver_chestplate)    => true,
            nameof(Item.gold_chestplate)      => true,
            nameof(Item.cobalt_chestplate)    => true,
            nameof(Item.palladium_chestplate) => true,
            _ => false,
        };

        if (!has_metal_helmet && !has_metal_chest) return;

        player.effects.effectManager.TryAddEffect(EffectLightningRod.lightning_rod, 5);
    }
}

public class ThunderBiomeGenerator : GeneratorBiome {
    public static ThunderBiomeGenerator thunder_gen =
        new(id(nameof(thunder_gen)));

    public ThunderBiomeGenerator(string name) : base(name) {
        associatedBiome = ThunderBiome.thunder;

        grass = Block.gravel;
        soil  = Block.cobblestone;
        stone = Block.stone;
        hell  = Block.undergrowth_rock;

        sand = Block.gravel;
        sand_underneath = Block.cobblestone;

        underwater  = Block.gravel;
        clay_pocket = Block.clay;
        dirt_pocket = Block.packed_dirt;

        gem = Block.sapphire_ore;
    }

    public override void RegisterFeatures() {
        string[] lava_lakes = [
            "lava_lake_1",
            "lava_lake_2",
            "lava_lake_3",
        ];

        string[] underground_lakes = [
            "underground_lake_1",
            "underground_lake_2",
            "underground_lake_3",
        ];

        features.AddRange([
            new StructureFeature(lava_lakes, -4, true)
                .SetBase(TerrainFeature.RequiredBase.Solid)
                .AmountPerChunk(35f)
                .SetHeights(3, seaLevel - 48),

            new StructureFeature(underground_lakes, -4, true)
                .SetBase(TerrainFeature.RequiredBase.Solid)
                .AmountPerChunk(15f)
                .SetHeights(15, seaLevel - 32),

            new CrateFeature(Block.glowing_mushroom)
                .SetBase(TerrainFeature.RequiredBase.Solid)
                .AmountPerChunk(20f)
                .SetHeights(seaLevel + 1, seaLevel + 32),

            new CrateFeature(Block.spider_web)
                .SetBase(TerrainFeature.RequiredBase.Solid)
                .AmountPerChunk(100f)
                .SetHeights(3, seaLevel - 16),

            new GlowWormFeature()
                .AmountPerChunk(120f)
                .SetHeights(3, ChunkColumn.seaLevel - 24),

            new GrassFeature(Block.lilypad, 2)
                .SetBase(TerrainFeature.RequiredBase.Fluid)
                .AmountPerChunk(30f)
                .SetHeights(seaLevel + 1, seaLevel + 2),

            new CrateFeature(Block.undergrowth_crate)
                .SetBase(TerrainFeature.RequiredBase.Solid)
                .AmountPerChunk(50f)
                .SetHeights(2, 27),

            new BushFeature(Block.underground_bush, 1.5f)
                .AmountPerChunk(20f)
                .SetHeights(4, 27),

            new BushFeature(Block.spider_web, 1.5f)
                .AmountPerChunk(20f)
                .SetHeights(4, 27),

            new HangingVineFeature(Block.hanging_vines)
                .AmountPerChunk(100f)
                .SetHeights(16, 48),

            new OreFeature(Block.cobalt_ore, 2, 6, 2)
                .AmountPerChunk(64f)
                .SetHeights(0, 32),

            new OreFeature(Block.sapphire_ore, 3, 8, 2)
                .AmountPerChunk(48f)
                .SetHeights(16, 48),

            new OreFeature(Block.allumerium_ore, 2, 6, 1, true)
                .AmountPerChunk(8f)
                .SetHeights(0, 32),

            new OreFeature(Block.gold_ore, 5, 10, 2)
                .AmountPerChunk(32f)
                .SetHeights(8, 40),

            new OreFeature(Block.silver_ore, 6, 16, 3)
                .AmountPerChunk(96f)
                .SetHeights(8, seaLevel - 16),

            new CrateFeature(Block.spike_trap)
                .SetBase(TerrainFeature.RequiredBase.Solid)
                .AmountPerChunk(300f)
                .SetHeights(2, seaLevel - 16),

            new WaterPlantFeature(Block.pebbles)
                .SetBase(TerrainFeature.RequiredBase.Solid)
                .AmountPerChunk(60f)
                .SetHeights(64, seaLevel)
                .FollowHeightmap(),

            new WaterPlantFeature(Block.flint)
                .SetBase(TerrainFeature.RequiredBase.Solid)
                .AmountPerChunk(15f)
                .SetHeights(64, seaLevel)
                .FollowHeightmap(),

            new GlowWormFeature()
                .AmountPerChunk(90f)
                .SetHeights(seaLevel + 8, seaLevel + 32),

            new WaterfallFeature(Fluid.water)
                .AmountPerChunk(10f)
                .SetHeights(seaLevel + 8, seaLevel + 32),
        ]);
    }
}
