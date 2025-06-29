using System;
using Creatures.Player;
using UnityEngine;

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
    [SerializeField] private int chargeDamage;
    [SerializeField] private float chargeSpeed;
    [SerializeField] private float chargeDuration;
    public float _activeChargeDuration;
    private bool _lookingLeft = true;
    private Vector2 _chargeDirection;
    // Vision
    [SerializeField] private float visionRange = 8f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private Transform rayOrigin;
    [SerializeField] private Vector2 rayDirection;


    private void Start() {
        rayDirection = _lookingLeft ? Vector2.left : Vector2.right;
        _roamingStartPosition = transform.position;
        _activeWindUpTimer = windUpTimer;
    }

    private void Update() {
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
                _activeWindUpTimer -= Time.deltaTime;
                if (_activeWindUpTimer <= 0f) {
                    _currentState = EnemyState.Attacking;
                    _chargeDirection = rayDirection;
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
            rb.linearVelocity = rayDirection * movementSpeed;

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
            rb.linearVelocity = rayDirection * movementSpeed;
            _turningLocked = true;
        }
    }

    private void Stop() {
        rb.linearVelocity = Vector2.zero;
    }


    private void Attack() {
        rb.linearVelocity = rayDirection * chargeSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        // if collision player layer
        Debug.Log("Collided with: " + collision.gameObject.name);
        if ((1 << collision.gameObject.layer & playerLayer) != 0) {
            var player = collision.gameObject.GetComponent<PlayerMain>();
            player?.TakeDamage(chargeDamage);
            _currentState = EnemyState.Roaming;
        }
    }
}


}