using Allumeria;
using Allumeria.Biomes.Generators;
using Allumeria.ChunkManagement;

using EvesThunder.World;

using HarmonyLib;

namespace EvesThunder.World;

[HarmonyPatch]
internal static class GenerateBiomes {
    [HarmonyPatch(typeof(ChunkColumn), nameof(ChunkColumn.GenerateData))]
    private static class GenerateDataPatch {
        [HarmonyPostfix]
        private static void Postfix(
            ChunkColumn __instance,
            FastNoiseLite noise,
            Vector2i[] biomePoints,
            bool superFlat = false,
            bool voidWorld = false,
            bool forestOnly = false
        ) {
            if (voidWorld || superFlat || forestOnly) {
                return;
            }

            for (int col_idx_x = 0; col_idx_x < 32; col_idx_x++)
            for (int col_idx_z = 0; col_idx_z < 32; col_idx_z++) {
                int idx_x = col_idx_x + __instance.posX * 32;
                int idx_z = col_idx_z + __instance.posZ * 32;

                // if (__instance.biomeMap[col_idx_x, col_idx_z] == GeneratorBiome.desert) {
                //     found_desert = true;
                //     __instance.biomeMap[col_idx_x, col_idx_z] = ThunderBiomeGenerator.thunder_gen;
                // }

                float noise_value = noise.GetNoise(idx_z, idx_x) * 2f;

                Vector2 world_point = new(idx_x, idx_z);
                float point_distance = Vector2.Distance(biomePoints[1], world_point);

                bool is_beach = 0 > 485.0 + noise_value * 4.0;

                if (!is_beach && point_distance < 200.0 + noise_value * 10.0) {
                    __instance.biomeMap[col_idx_x, col_idx_z] = ThunderBiomeGenerator.thunder_gen;
                }
            }
        }
    }
}
