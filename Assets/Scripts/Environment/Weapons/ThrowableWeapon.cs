using System;
using Characters.Player;
using Physics;
using Unity.VisualScripting;
using UnityEngine;

namespace Environment.Weapons
{
public abstract class ThrowableWeapon: Projectile, IPlayerWeapon, IPhysicsMovable {
    public WeaponStatus State { get; set; }
    [SerializeField] private Rigidbody2D rb;
    public Rigidbody2D Rigidbody => rb;
    public bool singleUse = false;

    protected override void Start() {
        base.Start();
        CreatePickupRadius();
    }

    protected virtual void OnCollisionEnter2D(Collision2D other) {
        AllowPlayerToPickUp(other.collider);
    }

    protected virtual void OnTriggerEnter2D(Collider2D other) {
        AllowPlayerToPickUp(other);
    }

    private void AllowPlayerToPickUp(Collider2D other) {
        if (State == WeaponStatus.Idle) {
            Debug.Log("Trynna find the playa");
            var player = other.gameObject.GetComponent<PlayerMain>();
            if (player is null) return;
            player.pickupableWeapon = this;
            Debug.Log("Me," + gameObject.name + " is pickupable by " + player.gameObject.name);
        }
    }

    private void CreatePickupRadius() {
        Debug.Log("Creating pickup radius for " + gameObject.name);
        var pickupRadius = new GameObject("PickupRadius");
        pickupRadius.transform.SetParent(transform);
        pickupRadius.transform.localPosition = Vector3.zero;
        pickupRadius.layer = LayerMask.NameToLayer("PickableItem");

        var boxColl = pickupRadius.AddComponent<BoxCollider2D>();
        boxColl.isTrigger = true;
        boxColl.size = new Vector2(2f, 0.7f);
    }
}
}