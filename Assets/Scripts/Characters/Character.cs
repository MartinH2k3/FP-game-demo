using System.Collections.Generic;
using UnityEngine;
using GameMechanics.StatusEffects;
using Physics;

namespace Characters
{
public abstract class Character: MonoBehaviour, IPhysicsMovable {
    [SerializeField] protected int maxHealthPoints = 100;
    [SerializeField] protected int healthPoints;
    [SerializeField] protected Rigidbody2D rb;
    public Rigidbody2D Rigidbody => rb;

    // movement
    [SerializeField] protected float baseMovementSpeed = 3;
    protected float movementSpeed;

    private float _movementTimeout; // Unable to move after knockback or something alike
    private bool _isVulnerable = true;
    private bool _canBeMovedByOutsideForces = true;

    private class ActiveEffectInfo {
        public int Strength;
        public float Duration;
        public float RemainingTime;
        public ActiveEffectInfo(int strength, float duration) {
            Strength = strength;
            Duration = duration;
            RemainingTime = duration;
        }

    }
    private Dictionary<Effect, ActiveEffectInfo> _activeEffects = new();

    protected virtual void Start() {
        healthPoints = maxHealthPoints;
        movementSpeed = baseMovementSpeed;
    }

    protected virtual void Update() {
        ManageCooldowns();
    }

    protected void LateUpdate() {
        ApplyMovementEffects();
    }

    private void ManageCooldowns() {
        if (_movementTimeout > 0f) {
            _movementTimeout -= Time.deltaTime;
        }

        foreach (var effect in _activeEffects.Keys) {
            if (_activeEffects.TryGetValue(effect, out var activeEffectInfo)) {
                activeEffectInfo.RemainingTime -= Time.deltaTime;
                if (activeEffectInfo.RemainingTime <= 0f) {
                    _activeEffects.Remove(effect);
                }
            }
        }
    }

    // movement and velocity
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
    public void AddEffect(Effect effect, int strength) {
        _activeEffects.TryGetValue(effect, out var activeEffectInfo);
        if (activeEffectInfo != null) {
            // If new effect is stronger, replace it
            if (strength > activeEffectInfo.Strength) {
                activeEffectInfo.Strength = strength;
                activeEffectInfo.RemainingTime = StatusDurations.Instance.GetDuration(effect, strength);
            }
            // If the effect is already active with the same strength, reset its duration
            else if (strength == activeEffectInfo.Strength) {
                activeEffectInfo.RemainingTime = StatusDurations.Instance.GetDuration(effect, strength);
            }
            // If the effect is weaker, extend its duration fractionally
            else {
                activeEffectInfo.RemainingTime = Mathf.Min(activeEffectInfo.RemainingTime +
                                                           activeEffectInfo.Duration * strength / activeEffectInfo.Strength,
                    activeEffectInfo.Duration);
            }
        }
        else {
            Debug.Log("Added new effect: " + effect + " with strength: " + strength);
            // If the effect is not active, add it to the dictionary
            _activeEffects[effect] = new ActiveEffectInfo(strength, StatusDurations.Instance.GetDuration(effect, strength));
        }
    }

    public void ApplyMovementEffects() {
        if (_activeEffects.TryGetValue(Effect.Slow, out var slowEffect)) {
            Debug.Log("Applying slow");
            ApplySlow(slowEffect);
        }
        if (_activeEffects.TryGetValue(Effect.Stun, out _)) {
            ApplyStun();
        }
    }


    private void ApplySlow(ActiveEffectInfo slowInfo) {
        if (rb is null) return;
        var slowFactor = SlowFactors.Instance.GetSlowFactor(slowInfo.Strength);
        movementSpeed = baseMovementSpeed * (1 - slowFactor);
    }

    private void ApplyStun() {
        if (rb is null) return;
        this.SetVelocity(Vector2.zero);
    }
}
}