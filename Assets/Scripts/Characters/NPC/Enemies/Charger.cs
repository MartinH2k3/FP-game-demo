using System;
using Characters.Player;
using Physics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Helpers;

namespace Characters.NPC.Enemies
{

public class Charger : Enemy
{
    private enum State
    {
        Roaming,
        Charging,
        Attacking
    }
    private State _currentState = State.Roaming;
    [SerializeField] private float roamingRange;
    private Vector2 _roamingStartPosition;
    [SerializeField] private float turningTimer;
    private float _activeTurningTimer;
    private bool _turningLocked = true;
    [SerializeField] private float groundCheckDistance;
    // Charging for attack
    [SerializeField] private float windUpTimer;
    private float _activeWindUpTimer;
    // Attack
    [SerializeField] private int contactDamage;
    [SerializeField] private int chargeDamage;
    [SerializeField] private float chargeSpeed;
    [SerializeField] private float chargeDuration;
    private float _activeChargeDuration;
    // Knockback
    [SerializeField] private float knockbackDuration;
    [SerializeField] private float knockbackStrength;
    // Vision
    [SerializeField] private float visionRange = 8f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private Transform rayOrigin;
    [SerializeField] private Vector2 walkingDirection;


    protected override void Start() {
        base.Start();
        walkingDirection = Vector2.left;
        _roamingStartPosition = transform.position;
        _activeWindUpTimer = windUpTimer;
    }

    protected override void Update() {
        base.Update();
        switch (_currentState) {
            case State.Roaming:
                if (CanSeePlayer()) {
                    Stop();
                    _currentState = State.Charging;
                    _activeWindUpTimer = windUpTimer;
                } else {
                    Roam();
                }
                break;
            case State.Charging:
                Stop();
                _activeWindUpTimer -= Time.deltaTime;
                if (_activeWindUpTimer <= 0f) {
                    _currentState = State.Attacking;
                    _activeChargeDuration = chargeDuration;
                }
                break;
            case State.Attacking:
                _activeChargeDuration -= Time.deltaTime;
                Attack();
                if (_activeChargeDuration <= 0f) {
                    _currentState = State.Roaming;
                    _activeTurningTimer = 0;
                }
                break;
            default:
                Debug.LogError("Charger: Invalid state encountered in Update method.");
                break;

        }

    }


    private bool CanSeePlayer() {
        var hit = Physics2D.Raycast(rayOrigin.position, walkingDirection.normalized, visionRange, playerLayer | obstacleLayer);
        // Debug.DrawRay(rayOrigin.position, walkingDirection.normalized * visionRange, Color.red);

        if (hit.collider is null) return false;

        // If we hit something on the player layer
        return ((1 << hit.collider.gameObject.layer) & playerLayer) != 0;
    }

    private void Roam() {
        if (!CanMove()) return;

        DetectFallAndResetRoamingStart();

        // Check if the enemy is within the roaming range
        if ((Vector2.Distance(transform.position, _roamingStartPosition) < roamingRange || _turningLocked)
            && CanWalkForward()) {
            // Move in the current direction
            SetVelocity(walkingDirection.x * movementSpeed, rb.linearVelocity.y);

            if (!_turningLocked) return;

            _activeTurningTimer -= Time.deltaTime;
            if (_activeTurningTimer <= 0f) {
                _turningLocked = false;
                _activeTurningTimer = turningTimer;
            }
        }
        else {
            TurnAround();
            SetVelocity(walkingDirection.x * movementSpeed, rb.linearVelocity.y);
        }
    }

    private void TurnAround() {
        walkingDirection = -walkingDirection;
        _turningLocked = true;
    }

    private bool CanWalkForward() {
        // Position slightly ahead of current position
        Vector2 origin = (Vector2)transform.position + new Vector2(groundCheckDistance * walkingDirection.x, 0f);

        // Raycast down to check for ground
        RaycastHit2D groundHit = Physics2D.Raycast(origin, Vector2.down, groundCheckDistance, obstacleLayer);
        // Debug.DrawRay(origin, Vector2.down * groundCheckDistance, groundHit.collider ? Color.green : Color.red);
        // Raycast forward to check for wall
        RaycastHit2D wallHit = Physics2D.Raycast(transform.position, walkingDirection.normalized, groundCheckDistance, obstacleLayer);
        // Debug.DrawRay(transform.position, walkingDirection.normalized * groundCheckDistance, wallHit.collider ? Color.red : Color.green);


        // We can walk forward only if there's ground ahead and no wall
        return groundHit.collider is not null && wallHit.collider is null;
    }


    private void Stop() {
        if (!CanMove()) return;
        SetVelocity(0, rb.linearVelocity.y);
    }


    private void Attack() {
        SetVelocity(walkingDirection.x * chargeSpeed, rb.linearVelocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        var touchingPlayer = TouchingPlayer(collision);
        var touchingWall = TouchingWall(collision);
        var isAttacking = _currentState == State.Attacking;

        if (touchingPlayer) {
            HitPlayer(collision, isAttacking);
        }

        if (isAttacking && (touchingWall || touchingPlayer)) {
            CrowdControl.Knockback(this,
                knockbackStrength/2,
                knockbackStrength/2,
                (int)-walkingDirection.x,
                knockbackDuration);
        }


    }

    private void HitPlayer(Collision2D collision, bool isCharge = true) {
        var player = collision.gameObject.GetComponent<PlayerMain>();
        if (isCharge) {
            player?.TakeDamage(chargeDamage);
            _currentState = State.Roaming;
        }
        else {
            player?.TakeDamage(contactDamage);
        }
        var contact = collision.contacts[0];
        CrowdControl.Knockback(player,
            isCharge ? knockbackStrength : knockbackStrength / 2,
            isCharge ? knockbackStrength : knockbackStrength / 2,
            contact.normal.x > 0 ? -1 : 1,
            knockbackDuration);

    }

    private bool TouchingPlayer(Collision2D collision) {
        return HelperMethods.LayerInLayerMask(collision.gameObject.layer, playerLayer);
    }

    private bool TouchingWall(Collision2D collision) {
        if (!HelperMethods.LayerInLayerMask(collision.gameObject.layer, obstacleLayer)) return false;

        foreach (var contact in collision.contacts) {
            if (Math.Abs(contact.normal.x) > 0.5f) return true;
        }
        return false;
    }

    private void DetectFallAndResetRoamingStart() {
        if (Math.Abs(transform.position.y - _roamingStartPosition.y) <= 0.001f) return;
        _roamingStartPosition = transform.position;
    }
}


}