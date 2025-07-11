using System;
using Characters.Player;
using JetBrains.Annotations;
using Physics;
using Unity.VisualScripting;
using UnityEngine;

namespace Characters.NPC.Enemies
{

public class SneakyBomb : Enemy
{
    private enum State
    {
        Idle,
        DiggingUp,
        Pursuing
    }

    private State _currentState = State.Idle;

    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private float diggingUpDuration = 2f;
    private float _activeDiggingUpDuration;
    // for performance reasons, to not check on every frame
    private float _detectionCooldown = 0.2f;
    private float _activeDetectionCooldown;
    // explosion
    [SerializeField] private int explosionDamage = 50;
    [SerializeField] private float knockbackStrength = 10f;

    [CanBeNull] private Character _target = null;
    // Visuals
    [SerializeField] private Animator animator;
    private bool _facingRight = true;

    protected override void Start() {
        base.Start();
        _activeDetectionCooldown = _detectionCooldown;
    }

    protected override void Update() {
        base.Update();
        switch (_currentState) {
            case State.Idle:
                Wait();
                break;
            case State.DiggingUp:
                _activeDiggingUpDuration -= Time.deltaTime;
                if (_activeDiggingUpDuration <= 0) {
                    _currentState = State.Pursuing;
                    animator.SetTrigger("Pursuing");
                }
                break;
            case State.Pursuing:
                Pursue();
                SyncVisuals();
                break;
        }
    }

    private void SyncVisuals() {
        if (_facingRight && this.GetVelocity().x < 0) {
            _facingRight = false;
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (!_facingRight && this.GetVelocity().x > 0) {
            _facingRight = true;
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    private void Wait() {
        _activeDetectionCooldown -= Time.deltaTime;

        if (_activeDetectionCooldown >= 0) return;

        _activeDetectionCooldown = _detectionCooldown;

        if (IsPlayerDetected()) {
            _currentState = State.DiggingUp;
            _activeDiggingUpDuration = diggingUpDuration;
            animator.SetTrigger("DiggingUp");
        }
    }

    private bool IsPlayerDetected() {
        var inspectedRange = Physics2D.OverlapCircleAll(transform.position, detectionRadius, targetLayer);
        if (inspectedRange.Length == 0) return false;
        foreach (var detectedPlayer in inspectedRange) {
            if (detectedPlayer.TryGetComponent(out Character target)) {
                _target = target;
                return true;
            }
        }
        Debug.LogError("SneakyBomb: Target layer detected, but no target object found.");
        return false;

    }

    private void Pursue() {
        int direction = _target!.transform.position.x > transform.position.x ? 1 : -1;
        this.SetVelocity(direction * movementSpeed, rb.linearVelocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (_currentState != State.Pursuing) return;

        if (collision.gameObject.TryGetComponent(out Character player)) {
            player.TakeDamage(explosionDamage);
            CrowdControl.Knockback(player, knockbackStrength, knockbackStrength, player.transform.position.x > transform.position.x ? 1 : -1);
            Destroy(gameObject); // Destroy the bomb after it explodes
        }
    }
}

}
