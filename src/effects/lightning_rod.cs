using Allumeria.EntitySystem;
using Allumeria.EntitySystem.Effects;
using Allumeria.Particles;

namespace EvesThunder.Effects;

public sealed class EffectLightningRod : Effect {
    public static readonly Effect lightning_rod =
        new EffectLightningRod(++IdHelper.effect_id, id(nameof(lightning_rod)));

    private const int TICKS_PER_PARTICLE = 3;

    private const float MIN_VELOCITY = 0.2f;
    private const float MAX_VELOCITY = 1.6f;

    public static readonly ParticleBehaviour particle
        = new ParticleBehaviour().SetTexture("spark");

    public EffectLightningRod(int id, string name)
        : base(id, name, 0, 0, EffectType.Hidden) { }

    public override void OnTick(
        Entity entity,
        ActiveEffect activeEffect,
        EffectManager manager
    ) {
        if (activeEffect.ticksElapsed % TICKS_PER_PARTICLE != 0) return;

        particle.Burst(
            entity.position,
            rng_particle.NextVector3(MIN_VELOCITY, MAX_VELOCITY)
        );
    }
}
