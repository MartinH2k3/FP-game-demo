using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using GameMechanics.StatusEffects;

namespace Characters
{
public abstract class Character: MonoBehaviour {
    [SerializeField] protected int maxHealthPoints = 100;
    [SerializeField] protected int healthPoints;
    [SerializeField] protected Rigidbody2D rb;

    private float _movementTimeout; // Unable to move after knockback or something alike
    private bool _isVulnerable = true;
    private bool _canBeMovedByOutsideForces = true;

    private class ActiveEffect {
        public Effect Effect;
        public int Strength;
        public float Duration;
        public float RemainingTime;
        public ActiveEffect(Effect effect, int strength, float duration) {
            Effect = effect;
            Strength = strength;
            Duration = duration;
            RemainingTime = duration;
        }

    }
    private List<ActiveEffect> _activeEffects = new();

    protected virtual void Start() {
        healthPoints = maxHealthPoints;
    }

    protected virtual void Update() {
        if (_movementTimeout > 0f) {
            _movementTimeout -= Time.deltaTime;
        }
    }

    // movement and velocity
    public void AddVelocity(Vector2 velocity) {
        if (rb != null) {
            rb.linearVelocity += velocity;
        }
    }

    public void AddVelocity(float x, float y) {
        if (rb != null) {
            rb.linearVelocity += new Vector2(x, y);
        }
    }

    public void SetVelocity(Vector2 velocity) {
        if (rb != null) {
            rb.linearVelocity = velocity;
        }
    }

    public void SetVelocity(float x, float y) {
        if (rb is not null) {
            rb.linearVelocity = new Vector2(x, y);
        }
    }

    protected bool CanMove() {
        return _movementTimeout <= 0f;
    }

    public bool CanBeMovedByOutsideForces() {
        return _canBeMovedByOutsideForces;
    }

    public void SetCanBeMovedByOutsideForces(bool canBeMoved) {
        _canBeMovedByOutsideForces = canBeMoved;
    }

    public void SetMovementTimeout(float timeout) {
        _movementTimeout = timeout;
    }

    // Health

    private void SetVulnerability(bool isVulnerable) {
        _isVulnerable = isVulnerable;
    }

    public virtual void TakeDamage(int damage) {
        if (_isVulnerable) healthPoints -= damage;
    }

    public virtual void Heal(int health) {
        healthPoints += health;
    }

    // Status effects
    public void ApplyEffect(Effect effect, int strength) {

    }
}
}