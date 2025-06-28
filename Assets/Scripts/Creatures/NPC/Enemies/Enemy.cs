using UnityEngine;

namespace Creatures.NPC.Enemies
{
public abstract class Enemy: MonoBehaviour
{

    [SerializeField] protected int maxHealthPoints = 100;
    [SerializeField] protected int healthPoints;

    [SerializeField] protected Rigidbody2D rb;

    private void Start() {
        healthPoints = maxHealthPoints;
    }

    public void TakeDamage(int damage)
    {
        healthPoints -= damage;
        if (healthPoints <= 0)
        {
            Die();
        }
    }

    public void Heal(int heal) {
        healthPoints = Mathf.Max(maxHealthPoints, healthPoints + heal);
    }

    private void Die()
    {
        // Handle enemy death logic here, e.g., play animation, drop loot, etc.
        Destroy(gameObject);
    }
}
}