using GameMechanics.StatusEffects;
using Unity.VisualScripting;
using UnityEngine;

namespace Environment.Movable.Projectiles
{
public abstract class Projectile: MovableEnvElement
{
    public bool destroyedOnHit;
    // Set if damages players, enemies or both
    [SerializeField] private LayerMask targetLayers;
    [SerializeField] private int damage;
    [SerializeField] private float knockBack; // a lot for physical projectiles, low or 0 for spells
    public StatusEffect effect;
    [SerializeField] private int effectStrength;

    private void OnCollisionEnter2D(Collision2D collision) {

    }
}
}