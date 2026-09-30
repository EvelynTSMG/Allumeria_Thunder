namespace EvesThunder;

public static class EvesUtils {
    internal static string id(string id) {
        return $"{EvesThunderMod.MOD_ID}.{id}";
    }

    public static class Keys {
        internal static string item(string id) {
            return $"ignitron.{EvesThunderMod.MOD_ID}.textures.atlas.items.{id}";
        }

        internal static string block(string id) {
            return $"ignitron.{EvesThunderMod.MOD_ID}.textures.atlas.blocks.{id}";
        }

        internal static string block_items(string id) {
            return $"ignitron.{EvesThunderMod.MOD_ID}.textures.atlas.block_items.{id}";
        }
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

    public static bool AddBefore<T>(this List<T> list, T item, T before) {
        int index = list.IndexOf(before);
        if (index < 0)
            return false;

        list.Insert(index, item);
        return true;
    }

    public static bool AddAfter<T>(this List<T> list, T item, T after) {
        int index = list.IndexOf(after);
        if (index < 0)
            return false;

        list.Insert(index + 1, item);
        return true;
    }
}
