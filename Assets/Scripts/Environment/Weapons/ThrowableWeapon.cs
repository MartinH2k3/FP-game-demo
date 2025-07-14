using System;
using System.Collections;
using Characters.Player;
using Helpers;
using Physics;
using UnityEngine;
using UnityEngine.InputSystem;

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
        if (HelperMethods.LayerInLayerMask(other.gameObject.layer, LayerMask.NameToLayer("Player"))
            && State == WeaponStatus.Idle) {
            AllowPlayerToPickUp(other.collider);
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D other) {
        AllowPlayerToPickUp(other);
    }

    private void AllowPlayerToPickUp(Collider2D other) {
        if (State == WeaponStatus.Idle) {
            var player = other.gameObject.GetComponent<PlayerMain>();
            if (player is null) return;
            player.pickupableWeapon = this;
        }
    }

    private void CreatePickupRadius() {
        var pickupRadius = new GameObject("PickupRadius");
        pickupRadius.transform.SetParent(transform);
        pickupRadius.transform.localPosition = Vector3.zero;
        pickupRadius.layer = LayerMask.NameToLayer("PickableItem");

        var boxColl = pickupRadius.AddComponent<BoxCollider2D>();
        boxColl.isTrigger = true;
        boxColl.size = new Vector2(2f, 0.7f);
    }

    // By default, launch at cursor position
    public override void Launch() {
        var cursorPos = Mouse.current.position.ReadValue();
        var inGameCursorPos = Camera.main.ScreenToWorldPoint(new Vector3(cursorPos.x, cursorPos.y, -Camera.main.transform.position.z));
        Launch(inGameCursorPos.x, inGameCursorPos.y);
    }

    public override void Launch(float x, float y) {
        State = WeaponStatus.Active;
        var col = GetComponent<Collider2D>();
        if (col is null) {
            Debug.LogWarning("No collider found on " + gameObject.name + ". Cannot pause collider.");
            return;
        }
        StartCoroutine(PauseCollider(col));


    }

    private IEnumerator PauseCollider(Collider2D col, float duration = 0.1f) {
        col.enabled = false;
        yield return new WaitForSeconds(duration);
        col.enabled = true;
    }
}
}