using Physics;
using UnityEngine;

namespace Environment.Weapons
{

public class Ball : ThrowableWeapon {
    private bool _facingLeft = true;
    private bool _notMoving = false;

    protected override void Update() {
        base.Update();
        if (this.GetVelocity() == Vector2.zero) {
            Debug.Log("Stoped moving");
            _notMoving = true;
        }
        else if (_notMoving) Debug.Log("What the heck?");
    }

    public override void Launch(float x, float y) {
        base.Launch(x, y);
        var direction = (new Vector2(x, y) - (Vector2)transform.position).normalized;
        this.SetVelocity(direction * speed);
    }
}

}