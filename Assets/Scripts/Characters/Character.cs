using UnityEngine;

namespace Characters
{
public class Character: MonoBehaviour {
    [SerializeField] protected int maxHealthPoints = 100;
    [SerializeField] protected int healthPoints;
    [SerializeField] protected Rigidbody2D rb;

    private float _movementTimeout; // Unable to move after knockback or something alike

    protected virtual void Start() {
        healthPoints = maxHealthPoints;
    }

    protected virtual void Update() {
        if (_movementTimeout > 0f) {
            _movementTimeout -= Time.deltaTime;
        }
    }

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


    public virtual void TakeDamage(int damage) {
        healthPoints -= damage;
    }

    public virtual void Heal(int health) {
        healthPoints += health;
    }

    protected bool CanMove() {
        return _movementTimeout <= 0f;
    }

    public void SetMovementTimeout(float timeout) {
        _movementTimeout = timeout;
    }
}
}