using Allumeria.Blocks.BlockModels;

namespace EvesThunder.Blocks;

public class EvesBlockModels {
    public static BlockModelQuads fulminite_crystal = (BlockModelQuads)
        new BlockModelQuads()
            .AddQuad(
                [
                    new Vector3( 0f,  0f, 16f),
                    new Vector3( 0f, 16f, 16f),
                    new Vector3(16f, 16f,  0f),
                    new Vector3(16f,  0f,  0f),
                ],
                new FaceUV(0, 0, 16, 16),
                0
            )
            .AddQuad(
                [
                    new Vector3(16f,  0f, 16f),
                    new Vector3(16f, 16f, 16f),
                    new Vector3( 0f, 16f,  0f),
                    new Vector3( 0f,  0f,  0f),
                ],
                new FaceUV(0, 0, 16, 16),
                0
            )
            .AddCollider(
                 3f, 0f,  3f,
                10f, 9f, 10f
            );
}
