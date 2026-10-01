using System.Diagnostics;

using Allumeria.ChunkManagement.TerrainFeatures;
using Allumeria.ChunkManagement.TerrainGeneration;
using Allumeria.DataManagement.Saving;

using HarmonyLib;

namespace EvesThunder.WorldGen;

internal class IslandChunkColumn : ChunkColumn {
    public IslandChunkColumn(int x, int z) : base(x, z) { }

    public override void GenerateData(
        FastNoiseLite noise,
        Vector2i[] biome_points,
        bool super_flat = false,
        bool void_world = false,
        bool forest_only = false
    ) {
        throw new NotSupportedException();
    }

    public void GenerateData(
        World world,
        FastNoiseLite noise,
        NoiseBlender noise_blender,
        bool super_flat,
        bool void_world,
        bool forest_only
    ) {
        if (void_world) {
            seaLevel = -1;
            for (int cx = 0; cx < 32; cx++)
            for (int cz = 0; cz < 32 ; cz++) {
                biomeMap[cx, cz] = GeneratorBiome.forest;

                generationHeightMap[cx, cz]   = -5;
                highestBlockHeightMap[cx, cz] = -5;
            }

            return;
        }

        if (super_flat) {
            seaLevel = 95;

            for (int cx = 0; cx < 32; cx++)
            for (int cz = 0; cz < 32; cz++) {
                biomeMap[cx, cz] = GeneratorBiome.forest;

                generationHeightMap[cx, cz]   = seaLevel + 4;
                highestBlockHeightMap[cx, cz] = seaLevel + 4;
            }

            return;
        }

        seaLevel = 95;

        Vector2i[] biome_points = new Vector2i[4];
        const double DEGREES = 360.0;
        const float TWO_PI = MathF.PI * 2f;
        for (int index = 0; index < biome_points.Length; ++index) {
            float t = index / (float)biome_points.Length;

            biome_points[index] = new Vector2i(
                (int)(MathF.Sin(TWO_PI * t) * DEGREES),
                (int)(MathF.Cos(TWO_PI * t) * DEGREES)
            );
        }

        GeneratorBiome[] biome_generators = [
            GeneratorBiome.desert,
            GeneratorBiome.beach,
            GeneratorBiome.snow,
            ThunderBiomeGenerator.thunder_gen,
        ];

        NoiseBlender biome_blend = noise_blender.CreateCopy();

        for (int cx = 0; cx < 32; cx++)
        for (int cz = 0; cz < 32; cz++) {
            int x = cx * posX * 32;
            int z = cz * posZ * 32;

            biome_blend.SampleNoiseXZ(x, 0, z, noise);

            float biome_nv = biome_blend.GetBlendedThreshold(x, 0, z, noise) * 2f;

            // float left_edge = world.worldLength * 0.3f;
            // float right_edge = world.worldLength - left_edge;

            // left_edge  += biome_nv * 10f;
            // right_edge += biome_nv * 10f;

            // if (0 > 485.0 + biome_nv * 4f) {
            //     biomeMap[cx, cz] = GeneratorBiome.beach;
            //     continue;
            // }

            if (!forest_only) {
                for (int biome_idx = 0; biome_idx < 4; biome_idx++) {
                    Vector2 biome_center = biome_points[biome_idx];
                    Vector2 pos = new(x, z);
                    const float BIOME_SIZE = 200.0f;

                    float dist_to_biome = Vector2.Distance(biome_center, pos);
                    bool in_biome = dist_to_biome < BIOME_SIZE + biome_nv * 10f;

                    if (!in_biome) continue;

                    biomeMap[cx, cz] = biome_generators[biome_idx];
                    break;
                }
            }


            noise.SetFrequency(0.005f);
            float forest_nv1 = biome_blend.GetBlendedThreshold(x + 2000, 0, z + 1232, noise) * 4f;
            float forest_nv2 = biome_blend.GetBlendedThreshold(x - 3812, 0, z + 5383, noise) * 4f;
            biomeMap[cx, cz] =
                forest_nv1 <= 0.0// || forest_nv2 <= 0.0
                    ? GeneratorBiome.forest
                    : GeneratorBiome.desert;
                    // ? forest_nv1 >= 0.0 || forest_nv2 <= 0.0
                        // : GeneratorBiome.dry_forest
        }

    }
}

internal class IslandWorld : World {
    public static readonly NoiseBlender terrain_noise =
        new NoiseBlendTwoWay(0.03f, 13041f, -5831f, 0.3f, -0.4f)
            .AddEntry(
                new NoiseBlendTwoWay(0.04f, 32485f, -249f, 0.6f, -0.2f)
                    .AddEntry(new NoiseBlendOneWay(EvesNoiseProfiles.iceberg))
                    .AddEntry(new NoiseBlendOneWay(NoiseProfile.rolling_hills))
            )
            .AddEntry(
                new NoiseBlendTwoWay(0.03f, 20350f, 3517f, 1.4f, -0.4f)
                    .AddEntry(new NoiseBlendOneWay(EvesNoiseProfiles.mountainous))
                    .AddEntry(new NoiseBlendOneWay(NoiseProfile.rolling_hills))
            );


    public static readonly NoiseBlender biome_noise =
        new NoiseBlendTwoWay(0.03f, 32485f, -249f, 1f, 0.0f)
            .AddEntry(new NoiseBlendOneWay(NoiseProfile.rolling_hills))
            .AddEntry(new NoiseBlendOneWay(NoiseProfile.rolling_hills));

    public IslandWorld(int width = 32, int length = 32, int height = 8)
        : base(width, height, length) { }

    private void generate_biomes() {
        for (int x = -worldWidth  / 2; x < worldWidth  / 2; x++)
        for (int z = -worldLength / 2; z < worldLength / 2; z++) {
            IslandChunkColumn column = new(x, z);
            column.GenerateData(this, noise, biome_noise, ignoreFeatures, voidWorld, forestOnly);
            chunkManager.columns.Add(new Vector2i(x, z), column);
        }
    }

    private void generate_terrain() {
        for (int x = -worldWidth / 2; x < worldWidth / 2; x++)
        for (int z = -worldWidth / 2; z < worldWidth / 2; z++) {
            Vector2i pos = new(x, z);
            if (!chunkManager.columns.TryGetValue(pos, out ChunkColumn? column)) {
                return;
            }

            for (int y = 0; y < worldHeight; y++) {
                chunkManager.RequestChunk(x, y, z)
                    .Generate(
                        column,
                        chunkManager.lightEngine,
                        new FastNoiseLite(worldSeed),
                        terrain_noise,
                        ignoreFeatures,
                        worldSeed
                    );

                ++worldGenProgress;
                ++generatedChunksComplete;
            }
        }
    }

    private void generate_soil() {
        for (int x = -worldWidth / 2; x < worldWidth / 2; x++)
        for (int z = -worldLength / 2; z < worldLength / 2; z++) {
            Vector2i pos = new(x, z);

            if (chunkManager.columns.TryGetValue(pos, out ChunkColumn? column)) {
                column.GeneateTopsoil(chunkManager, !ignoreFeatures);
            }
        }
    }

    private void generate_caves() {
        for (int x = -worldWidth / 2; x < worldWidth / 2; x++)
        for (int z = -worldLength / 2; z < worldLength / 2; z++) {
            Vector2i pos = new(x, z);

            if (!chunkManager.columns.TryGetValue(pos, out ChunkColumn? column)) {
                return;
            }

            column.GenerateCaves(chunkManager);

            ++worldGenProgress;
            ++featuredColumnsComplete;
        }
    }

    private void generate_features() {
        for (int x = -worldWidth / 2; x < worldWidth / 2; x++)
        for (int z = -worldLength / 2; z < worldLength / 2; z++) {
            Vector2i pos = new(x, z);

            if (!chunkManager.columns.TryGetValue(pos, out ChunkColumn? column)) {
                return;
            }

            column.GenerateFeatures(chunkManager, this);

            ++worldGenProgress;
            ++featuredColumnsComplete;
        }
    }

    private void generate_structures() {
        for (int x = -worldWidth / 2; x < worldWidth / 2; x++)
        for (int z = -worldLength / 2; z < worldLength / 2; z++) {
            Vector2i pos = new(x, z);

            if (!chunkManager.columns.TryGetValue(pos, out ChunkColumn? column)) {
                return;
            }

            column.GenerateStructures(chunkManager, this);

            ++worldGenProgress;
            ++featuredColumnsComplete;
        }
    }

    private void generate_dungeons() {
        for (int x = -worldWidth / 2; x < worldWidth / 2; x++)
        for (int z = -worldLength / 2; z < worldLength / 2; z++) {
            Vector2i pos = new(x, z);

            if (!chunkManager.columns.TryGetValue(pos, out ChunkColumn? column)) {
                return;
            }

            column.GenerateDungeons(chunkManager, this);

            ++worldGenProgress;
            ++featuredColumnsComplete;
        }
    }

    private void fill_sunlight() {
        for (int x = -worldWidth / 2; x < worldWidth / 2; x++)
        for (int z = -worldLength / 2; z < worldLength / 2; z++) {
            Vector2i pos = new(x, z);

            if (!chunkManager.columns.TryGetValue(pos, out ChunkColumn? column)) {
                return;
            }

            column.FillSunlight(chunkManager.lightEngine);
            for (int y = 0; y < worldHeight; y++) {
                chunkManager.RequestChunk(x, y, z)
                    .FillSunlight(column, chunkManager.lightEngine);

                ++worldGenProgress;
                ++litChunksComplete;
            }
        }
    }

    private void propagate_light() {
        chunkManager.lightEngine.UpdateAllAddition(0);
        chunkManager.lightEngine.UpdateAllAddition(1);
        chunkManager.lightEngine.UpdateAllAddition(2);
        chunkManager.lightEngine.UpdateAllAddition(3);
    }

    private void perform_worldgen_step(
        string artistic_text,
        string descriptive_text,
        Action worldgen_step
    ) {
        Game.menu_loadScreen.loadStepConsole.Add(artistic_text);
        worldGenStep = descriptive_text;

        Stopwatch stopwatch = new();

        stopwatch.Start();
        worldgen_step();
        stopwatch.Stop();

        Logger.Info($"Completed {worldGenStep} in {stopwatch.Elapsed.Milliseconds}ms");
    }

    public new void GenerateWorld(object _) {
        noise.SetSeed(worldSeed);

        generatedChunksComplete = 0;
        featuredColumnsComplete = 0;
        litChunksComplete = 0;
        worldGenProgress = 0;

        worldChunkCount = worldWidth * worldHeight * worldLength;

        perform_worldgen_step(
            "Painting biomes",
            "Generating Heightmap & Biome map",
            generate_biomes
        );

        perform_worldgen_step(
            "Sculpting terrain",
            "Generating Base Terrain",
            generate_terrain
        );

        perform_worldgen_step(
            "Planting Grass",
            "Generating Grass & Soil",
            generate_soil
        );

        if (voidWorld) {
            TerrainFeatureOld.GenerateCube(chunkManager, -2, 96, -2, 5, 1, 5, Block.glass);
            ignoreFeatures = true;
        }

        if (!ignoreFeatures) {
            perform_worldgen_step(
                "Drilling Caves",
                "Generating Caves",
                generate_caves
            );

            perform_worldgen_step(
                "Simulating Geology and Biology",
                "Generating Features",
                generate_features
            );

            // perform_worldgen_step(
            //     "Building Structures",
            //     "Generating Structures",
            //     generate_structures
            // );
            //
            // perform_worldgen_step(
            //     "Assembling Dungeons",
            //     "Generating Dungeons",
            //     generate_dungeons
            // );
        }

        perform_worldgen_step(
            "Lettting in the Light",
            "Simulating Sunlight",
            fill_sunlight
        );

        perform_worldgen_step(
            "Letting the Light Bounce",
            "Propagating Lights",
            propagate_light
        );

        if (saveOnGenerate) {
            perform_worldgen_step(
                "Ensuring Safety",
                "Saving World to Disk",
                () => GameSaver.SaveGame()
            );
        }

        perform_worldgen_step(
            "Embracing Adventure",
            "Entering World",
            () => InitAfterLoad(GameSaver.storedPlayer)
        );

        worldGenStep = "Done";
        generationComplete = true;

        timeManager.SetTime(24000);

        BGMPlayer.FadeOut();

        Game.menu_HUD.show = true;
        Game.menu_loadScreen.show = false;

        InputManager.queueLockMouse = true;
    }
}

[HarmonyPatch]
internal static class GenerateIslandWorld {
    [HarmonyPatch(typeof(WorldManager), nameof(WorldManager.GenerateNewWorld))]
    public static class WorldManager_GenerateNewWorld {
        [HarmonyPrefix]
        public static bool Prefix(WorldManager __instance) {
            BGMPlayer.bgm_world_creation.Play();

            Logger.Info("Generating Island World...");

            __instance.world = new IslandWorld();

            //__instance.world.GenerateWorldOnNewThread(__instance.worldSeed);
            __instance.world.worldSeed = __instance.worldSeed;
            ThreadPool.QueueUserWorkItem(((IslandWorld)__instance.world).GenerateWorld!);

            __instance.charData = new Dictionary<string, PerWorldCharData>();

            return false;
        }
    }
}
