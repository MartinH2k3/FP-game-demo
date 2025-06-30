using System;
using Creatures.Player;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

namespace Creatures.NPC.Enemies
{

public class Charger : Enemy
{
    public enum EnemyState
    {
        Roaming,
        Charging,
        Attacking
    }
    public EnemyState _currentState = EnemyState.Roaming;
    // Roaming
    [SerializeField] private float movementSpeed;
    [SerializeField] private float roamingRange;
    private Vector2 _roamingStartPosition;
    [SerializeField] private float turningTimer;
    public float _activeTurningTimer;
    private bool _turningLocked = true;
    // Charging for attack
    [SerializeField] private float windUpTimer;
    public float _activeWindUpTimer;
    // Attack
    [SerializeField] private int contactDamage;
    [SerializeField] private int chargeDamage;
    [SerializeField] private float chargeSpeed;
    [SerializeField] private float chargeDuration;
    public float _activeChargeDuration;
    private bool _lookingLeft = true;
    // Knockback
    [SerializeField] private float knockbackDuration;
    [SerializeField] private float knockbackStrength;
    // Vision
    [SerializeField] private float visionRange = 8f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private Transform rayOrigin;
    [SerializeField] private Vector2 rayDirection;


    protected override void Start() {
        base.Start();
        rayDirection = _lookingLeft ? Vector2.left : Vector2.right;
        _roamingStartPosition = transform.position;
        _activeWindUpTimer = windUpTimer;
    }

    protected override void Update() {
        base.Update();
        switch (_currentState) {
            case EnemyState.Roaming:
                if (CanSeePlayer()) {
                    Stop();
                    _currentState = EnemyState.Charging;
                    _activeWindUpTimer = windUpTimer;
                } else {
                    Roam();
                }
                break;
            case EnemyState.Charging:
                Stop();
                _activeWindUpTimer -= Time.deltaTime;
                if (_activeWindUpTimer <= 0f) {
                    _currentState = EnemyState.Attacking;
                    _activeChargeDuration = chargeDuration;
                }
                break;
            case EnemyState.Attacking:
                _activeChargeDuration -= Time.deltaTime;
                Attack();
                if (_activeChargeDuration <= 0f) {
                    _currentState = EnemyState.Roaming;
                    _activeTurningTimer = 0;
                    rayDirection = _lookingLeft ? Vector2.left : Vector2.right;
                }
                break;
            default:
                Debug.LogError("Charger: Invalid state encountered in Update method.");
                break;

        }

    }


    private bool CanSeePlayer() {
        var hit = Physics2D.Raycast(rayOrigin.position, rayDirection.normalized, visionRange, playerLayer | obstacleLayer);

        // Visual debug
        Debug.DrawRay(rayOrigin.position, rayDirection.normalized * visionRange, Color.red);

        if (hit.collider is null) return false;

        // If we hit something on the player layer
        return ((1 << hit.collider.gameObject.layer) & playerLayer) != 0;
    }

    private void Roam() {
        if (!CanMove()) return;

        // Check if the enemy is within the roaming range
        if (Vector2.Distance(transform.position, _roamingStartPosition) < roamingRange || _turningLocked) {
            // Move in the current direction
            SetVelocity(rayDirection.x * movementSpeed, rb.linearVelocity.y);

            if (!_turningLocked) return;

            _activeTurningTimer -= Time.deltaTime;
            if (_activeTurningTimer <= 0f) {
                _turningLocked = false;
                _activeTurningTimer = turningTimer;
            }
        }
        else {
            // Reverse direction when out of range
            _lookingLeft = !_lookingLeft;
            rayDirection = -rayDirection;
            SetVelocity(rayDirection.x * movementSpeed, rb.linearVelocity.y);
            _turningLocked = true;
        }
    }

    private void Stop() {
        if (!CanMove()) return;
        SetVelocity(0, rb.linearVelocity.y);
    }


    private void Attack() {
        SetVelocity(rayDirection.x * chargeSpeed, rb.linearVelocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        var touchingPlayer = TouchingPlayer(collision);
        var touchingWall = TouchingWall(collision);
        var isAttacking = _currentState == EnemyState.Attacking;

        if (touchingPlayer) {
            HitPlayer(collision, isAttacking);
        }

        if (isAttacking && (touchingWall || touchingPlayer)) {
            BounceBack();
        }


    }

    private void BounceBack() {
        SetVelocity(rayDirection.x * -knockbackStrength/2, knockbackStrength/2);
        SetMovementTimeout(knockbackDuration);
    }

    private void HitPlayer(Collision2D collision, bool isCharge = true) {
        var player = collision.gameObject.GetComponent<PlayerMain>();
        if (isCharge) {
            player?.TakeDamage(chargeDamage);
            _currentState = EnemyState.Roaming;
        }
        else {
            player?.TakeDamage(contactDamage);
        }
        KnockBackPlayer(collision, player, isCharge);

    }

    private void KnockBackPlayer(Collision2D collision, PlayerMain player, bool isCharge = true) {
        // get which direction the player collided on
        var contact = collision.contacts[0];
        var knockbackDirection = contact.normal.x > 0 ? -1 : 1;
        player?.SetVelocity(isCharge ?
                            new Vector2(knockbackDirection * knockbackStrength, knockbackStrength) :
                            new Vector2(knockbackDirection * knockbackStrength/2, knockbackStrength/2));
        player?.SetMovementTimeout(knockbackDuration);
    }

    private bool TouchingPlayer(Collision2D collision) {
        return (1 << collision.gameObject.layer & playerLayer) != 0;
    }

    private bool TouchingWall(Collision2D collision) {
        if ((1 << collision.gameObject.layer & obstacleLayer) == 0) return false;

        foreach (var contact in collision.contacts) {
            if (Math.Abs(contact.normal.x) > 0.5f) return true;
        }
        return false;
    }
}


}