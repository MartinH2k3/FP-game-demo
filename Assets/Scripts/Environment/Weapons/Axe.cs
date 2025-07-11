using Characters;
using Helpers;
using Physics;
using UnityEngine;

namespace Environment.Weapons {
public class Axe : ThrowableWeapon {
    [SerializeField] private GameObject axeHead;
    [SerializeField] private GameObject axeHandle;

    public override void Launch() {
        this.SetVelocity(speed, 0);
        State = WeaponStatus.Active;
    }

    public override void Launch(float x, float y) {
        var direction = (new Vector2(x, y) - (Vector2)transform.position).normalized;
        this.SetVelocity(direction * speed);
        State = WeaponStatus.Active;
    }

    protected override void Update() {
        base.Update();
        if (State != WeaponStatus.Active) return;
        var spinningSpeed = -this.GetVelocity().x * 50;
        transform.Rotate(0, 0, spinningSpeed * Time.deltaTime);
    }

    protected override void OnTriggerEnter2D(Collider2D other) {
        base.OnTriggerEnter2D(other);
        if (HelperMethods.LayerInLayerMask(other.gameObject.layer, targetLayers)) {
            var target = other.gameObject.GetComponent<Character>();
            HitTarget(target);
        }
    }

    protected override void OnCollisionEnter2D(Collision2D other) {
        base.OnCollisionEnter2D(other);
        if (HelperMethods.LayerInLayerMask(other.gameObject.layer, obstacleLayers)) {
            if (!HitWithHead(other)) return;

            this.Freeze();
            State = WeaponStatus.Idle;
        }
    }

    private bool HitWithHead(Collision2D collision) {
        return collision.otherCollider.gameObject.name == axeHead.name;
    }
}
}

