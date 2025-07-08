using Physics;
using UnityEngine;

namespace Environment.Movable.Projectiles
{
public class Kunai : Projectile, IPhysicsMovable {
    private bool _shouldRotate;
    [SerializeField] private float rotationSpeed = 180f; // degrees per second
    private Quaternion _targetRotation;
    [SerializeField] private Rigidbody2D rb;
    public Rigidbody2D Rigidbody => rb;

    protected override void Update() {
        base.Update();
        Rotate();
    }

    private void Rotate() {
        if (!_shouldRotate) return;
        Debug.Log("Kunai angle: " + transform.rotation.eulerAngles.z);
        // Spin baby, spin!
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            _targetRotation,
            rotationSpeed * Time.deltaTime
        );
        // Stop rotating when close
        if (Quaternion.Angle(transform.rotation, _targetRotation) < 0.1f) {
            Debug.Log("Not spinning");
            _shouldRotate = false;
        }
    }

    public override void Launch() {
        Launch(1, 0);
    }

    public override void Launch(float x, float y) {
        var direction = (new Vector2(x, y) - (Vector2)transform.position).normalized;
        this.SetVelocity(direction * speed);

        _shouldRotate = true;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90; // - 90 to account for original rotation
        Debug.Log(angle);
        _targetRotation = Quaternion.Euler(0, 0, angle);
    }

}
}