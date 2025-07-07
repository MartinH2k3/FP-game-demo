using Characters;
using GameMechanics.StatusEffects;
using Helpers;
using Physics;
using UnityEngine;

namespace Environment.Movable.Projectiles
{
public abstract class Projectile: MonoBehaviour
{
    [SerializeField] protected float speed = 15f;
    [SerializeField] private bool destroyedOnHit = true;
    // Set if damages players, enemies or both
    [SerializeField] private LayerMask targetLayers;
    [SerializeField] private LayerMask obstacleLayers;
    [SerializeField] private int damage;
    [SerializeField] private float knockBack; // a lot for physical projectiles, low or 0 for spells
    public Effect effect;
    [SerializeField] private int effectStrength;


    protected virtual void Start() {
        // Initialize any necessary components or variables
    }

    protected virtual void Update() {
        // Handle any updates needed for the projectile
    }

    protected void OnCollisionEnter2D(Collision2D collision) {
        if (HelperMethods.LayerInLayerMask(collision.gameObject.layer, obstacleLayers)) {
            Debug.Log("Collision with obstacle: " + collision.gameObject.name);
        }
        else if (HelperMethods.LayerInLayerMask(collision.gameObject.layer, targetLayers)) {
            var target = collision.gameObject.GetComponent<Character>();
            if (target != null) {
                // Apply damage
                target.TakeDamage(damage);

                // Apply knockback
                if (knockBack > 0) {
                    var direction = (collision.transform.position - transform.position).normalized;
                    CrowdControl.Knockback(target, knockBack, 0, direction.x > 0 ? 1 : -1);
                }

                // Apply status effect
                if (effect != Effect.None) {
                    target.AddEffect(effect, effectStrength);
                }
            }
        }
        else return;

        if (destroyedOnHit) {
            Destroy(gameObject);
        }
    }

    public abstract void Launch();

    public abstract void Launch(float x, float y);
}
}