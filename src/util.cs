namespace EvesThunder;

public static class EvesUtils {
    internal static string id(string id) {
        return $"{EvesThunderMod.MOD_ID}.{id}";
    }

    internal static readonly Random rng_particle = new();
}

internal static class EvesExt {
    extension (Random rng) {
        public Vector3 NextVector3() {
            return new Vector3 {
                X = (rng.NextSingle() - 0.5f) * 2f,
                Y = (rng.NextSingle() - 0.5f) * 2f,
                Z = (rng.NextSingle() - 0.5f) * 2f,
            };
        }

        public Vector3 NextVector3(float min, float max) {
            min = float.Abs(min);
            max = float.Abs(max);

            float delta = max - min;

            float x = (rng.NextSingle() - 0.5f) * 2f * delta;
            float y = (rng.NextSingle() - 0.5f) * 2f * delta;
            float z = (rng.NextSingle() - 0.5f) * 2f * delta;

            x += min * float.Sign(x);
            y += min * float.Sign(y);
            z += min * float.Sign(z);

            return new Vector3(x, y, z);
        }
    }
}
