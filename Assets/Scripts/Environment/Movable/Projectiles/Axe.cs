using Physics;
using UnityEngine;

namespace Environment.Movable.Projectiles {
public class Axe : Projectile, IPhysicsMovable {
    [SerializeField] private Rigidbody2D rb;
    public Rigidbody2D Rigidbody => rb;

    public override void Launch() {
        this.SetVelocity(speed, 0);
    }

    public override void Launch(float x, float y) {
        var direction = (new Vector2(x, y) - (Vector2)transform.position).normalized;
        this.SetVelocity(direction * speed);
    }
}
}