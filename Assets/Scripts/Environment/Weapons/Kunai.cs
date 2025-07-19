using Characters;
using Helpers;
using Physics;
using UnityEngine;

namespace Environment.Weapons
{
public class Kunai : ThrowableWeapon {
    private bool _shouldRotate;
    [SerializeField] private float rotationSpeed = 180f; // degrees per second
    private Quaternion _targetRotation;

    protected override void Update() {
        base.Update();
        Rotate();
    }

    private void Rotate() {
        if (!_shouldRotate) return;
        // Spin baby, spin!
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            _targetRotation,
            rotationSpeed * Time.deltaTime
        );
        // Stop rotating when close
        if (Quaternion.Angle(transform.rotation, _targetRotation) < 0.1f) {
            _shouldRotate = false;
        }
    }

    public override void Launch(float x, float y) {
        base.Launch(x, y);
        var direction = (new Vector2(x, y) - (Vector2)transform.position).normalized;
        this.SetVelocity(direction * speed);

        _shouldRotate = true;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90; // - 90 to account for original rotation
        Debug.Log(angle);
        _targetRotation = Quaternion.Euler(0, 0, angle);
    }

    protected override void OnCollisionEnter2D(Collision2D other) {
        base.OnCollisionEnter2D(other);
        if (HelperMethods.LayerInLayerMask(other.gameObject.layer, obstacleLayers)) {
            HitWall();
        }
        else if (HelperMethods.LayerInLayerMask(other.gameObject.layer, targetLayers) && state != WeaponStatus.Idle) {
            HitTarget(other.gameObject.GetComponent<Character>());
        }

    }

}
}