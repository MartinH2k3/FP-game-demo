using Characters;
using Helpers;
using Physics;
using UnityEngine;

namespace Environment.Weapons
{

public class Ball : ThrowableWeapon {
    [SerializeField] private float bouncinessDecayTime = 2f;
    private float _bouncinessTimer;
    [SerializeField] private float initialBounciness = 1f;
    [SerializeField] private Animator animator;

    protected override void Update() {
        base.Update();
        if (_bouncinessTimer > 0) {
            _bouncinessTimer -= Time.deltaTime;
            if (_bouncinessTimer <= 0) {
                _bouncinessTimer = 0;
                State = WeaponStatus.Idle;
                animator.SetTrigger("Deflated");
            }
        }
    }


    public override void Launch(float x, float y) {
        base.Launch(x, y);
        var direction = (new Vector2(x, y) - (Vector2)transform.position).normalized;
        this.SetVelocity(direction * speed);
        _bouncinessTimer = bouncinessDecayTime;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (HelperMethods.LayerInLayerMask(collision.gameObject.layer, obstacleLayers)) {
            var incomingVelocity = this.GetVelocity();
            var normal = collision.contacts[0].normal;
            Vector2 reflectedVelocity = Vector2.Reflect(-collision.relativeVelocity, normal) * GetCurrentBounceFactor();
            Debug.Log("Bouncing at bounce factor: " + GetCurrentBounceFactor() + " from velocity: " + collision.relativeVelocity + " to reflected velocity: " + reflectedVelocity);
            this.SetVelocity(reflectedVelocity);
        }
        if (HelperMethods.LayerInLayerMask(collision.gameObject.layer, targetLayers)) {
            HitTarget(collision.gameObject.GetComponent<Character>());
        }
    }

    private float GetCurrentBounceFactor() {
        float t = 1f - (_bouncinessTimer / bouncinessDecayTime);
        return Mathf.Lerp(initialBounciness, 0f, t);
    }

}

}