using Allumeria.ChunkManagement.TerrainGeneration;

namespace EvesThunder.WorldGen;

public class EvesNoiseProfiles {
    public static NoiseProfile iceberg = new(
        [
            (NoiseProfile.seaLevel - 9, 0.0f),
            (NoiseProfile.seaLevel - 7, 0.2f),
            (NoiseProfile.seaLevel - 3, 0.3f),
            (NoiseProfile.seaLevel + 1, 0.3f),
            (NoiseProfile.seaLevel + 5, 0.7f),
            (160, 1f),
            (256, 1f),
        ],
        Block.glass
    );

    public static NoiseProfile mountainous = new(
        [
            (NoiseProfile.seaLevel -  4, 0.00f),
            (NoiseProfile.seaLevel -  3, 0.07f),
            (NoiseProfile.seaLevel +  3, 0.07f),
            (NoiseProfile.seaLevel + 10, 0.12f),
            (NoiseProfile.seaLevel + 30, 0.2f),
            (NoiseProfile.seaLevel + 50, 0.4f),
            (NoiseProfile.seaLevel + 70, 1.0f),
            (160, 1.0f),
            (256, 1.0f),
        ],
        Block.glass
    );
}
