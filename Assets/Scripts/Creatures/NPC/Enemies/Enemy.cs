using UnityEngine;

namespace Creatures.NPC.Enemies
{
public abstract class Enemy: Character
{

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        if (healthPoints <= 0)
        {
            Die();
        }
    }


    private void Die()
    {
        // Handle enemy death logic here, e.g., play animation, drop loot, etc.
        Destroy(gameObject);
    }
}
}