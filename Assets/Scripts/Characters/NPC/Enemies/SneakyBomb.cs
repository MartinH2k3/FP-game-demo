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

    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private float diggingUpDuration = 2f;
    private float _activeDiggingUpDuration;
    // for performance reasons, to not check on every frame
    private float _detectionCooldown = 0.2f;
    private float _activeDetectionCooldown;
    // explosion
    [SerializeField] private int explosionDamage = 50;
    [SerializeField] private float knockbackStrength = 10f;

    [CanBeNull] private PlayerMain _player = null;
    [SerializeField] private Animator animator;

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
                if (_activeDiggingUpDuration <= 0)
                    _currentState = State.Pursuing;
                break;
            case State.Pursuing:
                Pursue();
                break;
        }
    }

    private void SyncVisuals() {

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
        var inspectedRange = Physics2D.OverlapCircleAll(transform.position, detectionRadius, playerLayer);
        if (inspectedRange.Length == 0) return false;
        foreach (var detectedPlayer in inspectedRange) {
            if (detectedPlayer.TryGetComponent(out PlayerMain player)) {
                _player = player;
                return true;
            }
        }
        Debug.LogError("SneakyBomb: Player layer detected, but no PlayerMain component found.");
        return false;

    }

    private void Pursue() {
        int direction = _player!.transform.position.x > transform.position.x ? 1 : -1;
        this.SetVelocity(direction * movementSpeed, rb.linearVelocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (_currentState != State.Pursuing) return;

        if (collision.gameObject.TryGetComponent(out PlayerMain player)) {
            player.TakeDamage(explosionDamage);
            CrowdControl.Knockback(player, knockbackStrength, knockbackStrength, player.transform.position.x > transform.position.x ? 1 : -1);
            Destroy(gameObject); // Destroy the bomb after it explodes
        }
    }
}

}
