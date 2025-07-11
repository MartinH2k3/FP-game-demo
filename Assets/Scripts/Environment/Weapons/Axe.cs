using System;
using System.Linq;
using Characters;
using Helpers;
using Physics;
using UnityEngine;

namespace Environment.Weapons {
public class Axe : Projectile, IPhysicsMovable {
    private enum AxeState {
        Flying,
        Stuck
    }

    [SerializeField] private Rigidbody2D rb;
    public Rigidbody2D Rigidbody => rb;
    private AxeState _state = AxeState.Flying;

    [SerializeField] private GameObject axeHead;
    [SerializeField] private GameObject axeHandle;
    [SerializeField] private float spinningSpeed = 180;


    public override void Launch() {
        this.SetVelocity(speed, 0);
        _state = AxeState.Flying;
    }

    public override void Launch(float x, float y) {
        var direction = (new Vector2(x, y) - (Vector2)transform.position).normalized;
        this.SetVelocity(direction * speed);
        _state = AxeState.Flying;

        if (direction.x < 0) {
            transform.localScale = new Vector3(-1, 1, 1); // Flip the axe
            spinningSpeed = -spinningSpeed;
        }
    }

    private void Rotate() {

    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (HelperMethods.LayerInLayerMask(other.gameObject.layer, targetLayers)) {
            var target = other.gameObject.GetComponent<Character>();
            HitTarget(target);
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (HelperMethods.LayerInLayerMask(other.gameObject.layer, obstacleLayers)) {
            if (!HitWithHead(other)) return;

            this.Freeze();
            _state = AxeState.Stuck;
        }
    }

    private bool HitWithHead(Collision2D collision) {
        return collision.otherCollider.gameObject.name == axeHead.name;
    }
}
}

