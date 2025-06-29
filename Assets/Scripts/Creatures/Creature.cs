using UnityEngine;

namespace Creatures
{
public class Creature: MonoBehaviour
{
    [SerializeField] protected int maxHealthPoints = 100;
    [SerializeField] protected int healthPoints;

    [SerializeField] protected Rigidbody2D rb;

    private void Start() {
        healthPoints = maxHealthPoints;
    }

    public void AddVelocity(Vector2 velocity)
    {
        if (rb != null)
        {
            rb.linearVelocity += velocity;
        }
    }

    public void AddVelocity(float x, float y)
    {
        if (rb != null)
        {
            rb.linearVelocity += new Vector2(x, y);
        }
    }

    public void SetVelocity(Vector2 velocity)
    {
        if (rb != null)
        {
            rb.linearVelocity = velocity;
        }
    }

    public void SetVelocity(float x, float y)
    {
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(x, y);
        }
    }
}
}