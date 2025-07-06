using UnityEngine;

namespace Environment.Movable
{
public abstract class MovableEnvElement: EnvElement
{
    // used for velocity and gravity scale
    [SerializeField] protected Rigidbody2D rb;

    public void AddVelocity(Vector2 velocity) {
        if (rb != null) {
            rb.linearVelocity += velocity;
        }
    }

    public void AddVelocity(float x, float y) {
        if (rb != null) {
            rb.linearVelocity += new Vector2(x, y);
        }
    }

    public void SetVelocity(Vector2 velocity) {
        if (rb != null) {
            rb.linearVelocity = velocity;
        }
    }

    public void SetVelocity(float x, float y) {
        if (rb is not null) {
            rb.linearVelocity = new Vector2(x, y);
        }
    }


}
}