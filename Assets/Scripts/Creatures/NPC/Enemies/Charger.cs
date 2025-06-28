using UnityEngine;

namespace Creatures.NPC.Enemies
{

public class Charger : Enemy
{
    // Roaming
    [SerializeField] private float movementSpeed = 3f;
    [SerializeField] private float roamingRange = 5f;
    private Vector2 _roamingStartPosition;
    // Attack
    [SerializeField] private float chargeSpeed = 10f;
    [SerializeField] private float chargeDuration = 1f;
    [SerializeField] private float chargeTime;
    private float _activeChargeTime;
    private bool _isCharging;
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
    }

    private void Update() {
        if (CanSeePlayer()) {
            Attack();
        }
        else {
            Roam();
        }
    }


    private bool CanSeePlayer()
    {
        var hit = Physics2D.Raycast(rayOrigin.position, rayDirection.normalized, visionRange, playerLayer | obstacleLayer);

        // Visual debug
        Debug.DrawRay(rayOrigin.position, rayDirection.normalized * visionRange, Color.red);

        if (hit.collider is null) return false;

        // If we hit something on the player layer
        return ((1 << hit.collider.gameObject.layer) & playerLayer) != 0;
    }

    private bool Roam() {
        // Check if the enemy is within the roaming range
        if (Vector2.Distance(transform.position, _roamingStartPosition) < roamingRange)
        {
            // Move in the current direction
            rb.linearVelocity = rayDirection * movementSpeed;
            return true;
        }
        else
        {
            // Reverse direction when out of range
            _lookingLeft = !_lookingLeft;
            rayDirection = -rayDirection;
            rb.linearVelocity = rayDirection * movementSpeed;
            return false;
        }
    }

    private void Attack() {
        rb.linearVelocity = rayDirection * chargeSpeed;
    }
}


}