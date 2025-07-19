using System.Collections.Generic;
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
	[SerializeField] private float idleThreshold = 0.1f; // Speed below which the ball is considered idle


	private Dictionary<Character, float> _recentHits = new();
	[SerializeField] private float hitCooldown = 0.5f;
    protected override void Update() {
        base.Update();
        if (_bouncinessTimer > 0) {
            _bouncinessTimer -= Time.deltaTime;
            if (_bouncinessTimer <= 0) {
                _bouncinessTimer = 0;
                state = WeaponStatus.Idle;
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

    protected override void OnCollisionEnter2D(Collision2D collision) {
        base.OnCollisionEnter2D(collision);
        if (HelperMethods.LayerInLayerMask(collision.gameObject.layer, obstacleLayers)) {
            var incomingVelocity = this.GetVelocity();
            var normal = collision.contacts[0].normal;
            Vector2 reflectedVelocity = Vector2.Reflect(-collision.relativeVelocity, normal) * GetCurrentBounceFactor();
            this.SetVelocity(reflectedVelocity);
			if (state == WeaponStatus.Active && reflectedVelocity.magnitude < idleThreshold) {
				state = WeaponStatus.Idle;
			}
        }
        if (HelperMethods.LayerInLayerMask(collision.gameObject.layer, targetLayers) && state != WeaponStatus.Idle) {
            var target = collision.gameObject.GetComponent<Character>();
            if (!CanHitTarget(target)) return;
            HitTarget(target);
        }
    }

    private bool CanHitTarget(Character target) {
        return target is not null && (!_recentHits.ContainsKey(target) || Time.time - _recentHits[target] > hitCooldown);
    }

    private float GetCurrentBounceFactor() {
        float t = 1f - (_bouncinessTimer / bouncinessDecayTime);
        return Mathf.Lerp(initialBounciness, 0f, t);
    }

}

}