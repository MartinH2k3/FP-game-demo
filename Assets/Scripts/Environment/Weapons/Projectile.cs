using Characters;
using GameMechanics.StatusEffects;
using Helpers;
using Physics;
using UnityEngine;

namespace Environment.Weapons
{
public abstract class Projectile: MonoBehaviour
{
    [SerializeField] protected float speed = 15f;
    [SerializeField] private bool destroyedOnHit = true;
    // Set if damages players, enemies or both
    [SerializeField] protected LayerMask targetLayers;
    [SerializeField] protected LayerMask obstacleLayers;
    [SerializeField] protected int damage;
    [SerializeField] protected float knockBack; // a lot for physical projectiles, low or 0 for spells
    public Effect effect;
    [SerializeField] private int effectStrength;


    protected virtual void Start() {
        // Initialize any necessary components or variables
    }

    protected virtual void Update() {
        // Handle any updates needed for the projectile
    }



    protected void HitTarget(Character target) {
        if (target is null) return;

        target.TakeDamage(damage);

        // Apply knockback
        if (knockBack > 0) {
            var direction = (target.transform.position - transform.position).normalized;
            CrowdControl.Knockback(target, knockBack, 0, direction.x > 0 ? 1 : -1);
        }

        // Apply status effect
        if (effect != Effect.None) {
            target.AddEffect(effect, effectStrength);
        }

        if (destroyedOnHit) {
            Destroy(gameObject);
        }
    }

    public abstract void Launch();

    public abstract void Launch(float x, float y);
}
}