using System;
using System.Collections.Generic;
using Environment.Weapons;
using Managers;
using Physics;
using Helpers;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Characters.Player
{

public class PlayerMain : Character
{
        // jumping
    private bool _onGround;
    public int jumpStrength = 5;
    [SerializeField] private float jumpCooldown = 0.2f;
    private float _activeJumpCooldown;
    private bool _doubleJumped;
            // wall jumping
    private bool _touchingRightWall;
    private bool _touchingLeftWall;
    private int _lastWallJump; // -1: left wall, 1: right wall, 0: none
    [SerializeField] private float wallCoyoteTime = 0.15f; // time to jump after leaving the wall for better user experience
    private float _leftWallCoyoteTimer;
    private float _rightWallCoyoteTimer;
    // dashing
    public float dashStrength = 15f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private float dashDuration = 0.2f; // duration of the dash
    private float _activeDashCooldown;
    private float _activeDashDuration;
    private bool _dashedInAir; // in air, only one dash possible
        // dash implementation
    private bool _isDashing;
    // climbing
    private bool _canClimb;
    // attack
    [SerializeField] private float attackRange;
    private float _attackCooldown;
    private AttackType _activeAttackType = AttackType.Melee;
    [SerializeField] private ThrowableWeapon rangedWeapon;
    // TODO add spell field

    // interaction
    public HashSet<ThrowableWeapon> NearbyWeapons = new();

    // stats
    public BaseStats baseStats;
    // game object references
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask obstacleLayerMask ;
    [SerializeField] private LayerMask hittableLayerMask;
    [SerializeField] private LayerMask climbableLayerMask;

    // input
    private InputSystemActions _inputActions;
    private InputAction _jump;
    private InputAction _move;
    private Vector2 _moveInput;
    private InputAction _sprint;
    private InputAction _dash;
    private InputAction _attack;
    private InputAction _attackTypeChoice;
    private InputAction _interact;
    // visual && animations
    private bool _isFacingRight = true; // used for flipping the sprite
    [SerializeField] private Animator animator;
    [SerializeField] private UIManager uiManager;

    // Creating helper instances
    private void Awake() {
        _inputActions = new InputSystemActions();
    }

    private void OnEnable() {
        _jump = _inputActions.Player.Jump;
        _jump.Enable();
        _jump.performed += Jump;
        _move = _inputActions.Player.Move;
        _move.Enable();
        _move.performed += Move;
        _move.canceled += Move;
        _sprint = _inputActions.Player.Sprint;
        _sprint.Enable();
        _sprint.performed += Sprint;
        _sprint.canceled += Sprint;
        _dash = _inputActions.Player.Dash;
        _dash.Enable();
        _dash.performed += Dash;
        _attack = _inputActions.Player.Attack;
        _attack.Enable();
        _attack.performed += Attack;
        _attackTypeChoice = _inputActions.Player.OptionChoice;
        _attackTypeChoice.Enable();
        _attackTypeChoice.performed += AttackTypeChoice;
        _interact = _inputActions.Player.Interact;
        _interact.Enable();
        _interact.performed += Interact;
    }

    private void OnDisable() {
        _jump.Disable();
        _move.Disable();
        _sprint.Disable();
        _dash.Disable();
        _attack.Disable();
        _attackTypeChoice.Disable();
        _interact.Disable();
    }

    protected override void Start() {
        base.Start();
        SetActiveAttackType(AttackType.Melee);
    }

    protected override void Update() {
        base.Update();
        HandleMovement();
        SyncVisuals();
        ManageCooldowns();
    }

    protected void FixedUpdate() {
        // resetting flags for jumping - on collision will be called after this
        _onGround = false;
        _touchingRightWall = false;
        _touchingLeftWall = false;
    }

    private void SyncVisuals() {
        animator.SetBool("OnGround", _onGround);
        animator.SetBool("TouchingRightWall", _touchingRightWall);
        animator.SetBool("TouchingLeftWall", _touchingLeftWall);
        animator.SetBool("Moving", _moveInput.x != 0);
        animator.SetBool("Climbing", _canClimb);
        uiManager.SetJumpAvailable(CanJump());
    }

    private void ManageCooldowns() {
        _activeJumpCooldown = Math.Max(_activeJumpCooldown - Time.deltaTime, 0);
        _activeDashCooldown = Math.Max(_activeDashCooldown - Time.deltaTime, 0);
        _attackCooldown = Math.Max(_attackCooldown - Time.deltaTime, 0);
        _leftWallCoyoteTimer = Math.Max(_leftWallCoyoteTimer - Time.deltaTime, 0);
        _rightWallCoyoteTimer = Math.Max(_rightWallCoyoteTimer - Time.deltaTime, 0);
    }

    private void HandleMovement() {
        if (!CanMove()) {
            return; // can't change velocity if movement is locked
        }

        if (_canClimb) {
            rb.gravityScale = 0;
            this.SetVelocity(_moveInput.x * movementSpeed, _moveInput.y * movementSpeed);
        } else if (_isDashing) {
            _activeDashDuration = Math.Max(_activeDashDuration - Time.deltaTime, 0);
            if (_activeDashDuration <= 0) {
                rb.linearVelocity = Vector2.zero;
                _isDashing = false;
            }
        }
        else {
            rb.gravityScale = 1;
            // using Math instead of Mathf, because in Update() method, Mathf.Sign(0) returns 1 (some sort of bug)
            this.SetVelocity(Math.Sign(_moveInput.x) * movementSpeed, rb.linearVelocity.y);
        }
    }

    private void Interact(InputAction.CallbackContext context) {
        if (!context.performed) return;

        if (NearbyWeapons.Count != 0) {
            DropWeapon();
            PickupWeapon();
        }
    }

    private void DropWeapon() {
        if (rangedWeapon is null) return;
        Instantiate(rangedWeapon, transform.position, Quaternion.Euler(0, 0, -25));
        rangedWeapon = null;
    }

    private void PickupWeapon() {
        ThrowableWeapon closest = null;
        float closestDist = float.MaxValue;
        foreach (var weapon in NearbyWeapons) {
            float dist = Vector2.Distance(transform.position, weapon.transform.position);
            if (dist < closestDist) {
                closest = weapon;
                closestDist = dist;
            }
        }

        rangedWeapon = closest.Prefab();
        Destroy(closest!.gameObject);
    }

    public void Attack(InputAction.CallbackContext context) {
        if (_attackCooldown > 0) return;
        _attackCooldown = 1/baseStats.attackSpeed;



        // attack point on left but facing right
        if (attackPoint.position.x < transform.position.x && _isFacingRight) {
            var diff = transform.position.x - attackPoint.position.x;
            attackPoint.position = new Vector2(transform.position.x + diff, transform.position.y);
        }
        // attack point on right but facing left
        else if (attackPoint.position.x > transform.position.x && !_isFacingRight) {
            var diff = attackPoint.position.x - transform.position.x;
            attackPoint.position = new Vector2(transform.position.x - diff, transform.position.y);
        }

        switch (_activeAttackType) {
            case AttackType.Melee:
                MeleeAttack();
                break;
            case AttackType.Spell:
                // TODO implement
                break;
            case AttackType.Ranged:
                RangedAttack();
                break;
        }

    }

    private void MeleeAttack() {
        int damage = baseStats.strength + baseStats.attackDamage * baseStats.attackDamageModifier;
        float knockback = (float)baseStats.strength / 2; // TODO add some table for this

        var targets = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, hittableLayerMask);

        foreach (var target in targets) {
            var enemy = target.gameObject.GetComponent<Character>();
            enemy.TakeDamage(damage);
            CrowdControl.Knockback(enemy, knockback, knockback, _isFacingRight ? 1 : -1);
        }
    }

    private void RangedAttack() {
        var weaponInstance = Instantiate(rangedWeapon, transform.position, Quaternion.identity);
        weaponInstance.Launch();
        if (weaponInstance.singleUse) rangedWeapon = null;
    }

    private void Jump(InputAction.CallbackContext context) {
        if (!CanJump()) return;

        this.SetVelocity(rb.linearVelocity.x, jumpStrength);
        _activeJumpCooldown = jumpCooldown;

        if (_onGround) {
            _lastWallJump = 0;
        } else if (!CanWallJump()){
            _doubleJumped = true;
        } else if (_touchingRightWall || _rightWallCoyoteTimer > 0) {
            _lastWallJump = 1;
            _doubleJumped = false;
            _rightWallCoyoteTimer = 0f;
        } else if (_touchingLeftWall || _leftWallCoyoteTimer > 0) {
            _lastWallJump = -1;
            _doubleJumped = false;
            _leftWallCoyoteTimer = 0f;
        }

    }

    // public as it will be displayed in HUD
    private bool CanJump() {
        return (CanWallJump() || _onGround || (GameManager.Instance.SkillData.IsUnlocked(Skill.DoubleJump) && !_doubleJumped))
               && _activeJumpCooldown == 0
               && !_isDashing
               && CanMove();
    }

    private bool CanWallJump() {
        var canLeft  = (_touchingLeftWall  || _leftWallCoyoteTimer  > 0) && _lastWallJump != -1;
        var canRight = (_touchingRightWall || _rightWallCoyoteTimer > 0) && _lastWallJump != 1;
        return canLeft || canRight;
    }


    private void Dash(InputAction.CallbackContext context) {
        if (context.performed) {
            if (!CanDash()) return;

            _isDashing = true;
            _dashedInAir = !_onGround;
            _activeDashCooldown = dashCooldown;
            _activeDashDuration = dashDuration;

            this.SetVelocity(_isFacingRight ? dashStrength : -dashStrength, 0);
        }
    }

    private bool CanDash() {
        return _activeDashCooldown <= 0 && (_onGround || !_dashedInAir) && CanMove();
    }

    private void Move(InputAction.CallbackContext context) {
        if (context.performed) {
            _moveInput = context.ReadValue<Vector2>();
            // Flip the sprite based on movement direction
            if (_moveInput.x > 0 && !_isFacingRight) {
                _isFacingRight = true;
                transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
            } else if (_moveInput.x < 0 && _isFacingRight) {
                _isFacingRight = false;
                transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
            }
        } else if (context.canceled) {
            _moveInput = Vector2.zero;
        }
    }

    private void Sprint(InputAction.CallbackContext context) {
        if (context.performed) movementSpeed = baseMovementSpeed * 2;
        else if (context.canceled) movementSpeed = baseMovementSpeed;
    }

    private void OnCollisionStay2D(Collision2D collision) {
        EvaluateCollision(collision);
    }
    private void OnTriggerStay2D(Collider2D collision) {
        EvaluateTrigger(collision);
    }
    private void OnTriggerExit2D(Collider2D collision) {
        CancelTriggers();
    }

    private void EvaluateCollision(Collision2D collision) {
        if (HelperMethods.LayerInLayerMask(collision.gameObject.layer, obstacleLayerMask)) {
            foreach (var contact in collision.contacts) {
                if (contact.normal.y > 0.5f) {
                    _onGround = true;
                    ResetJump();
                }

                // still using Math.Abs if we want to add more functionality for wall touch later
                if (Math.Abs(contact.normal.x) > 0.5f) {
                    if (contact.normal.x > 0) {
                        _touchingRightWall = true;
                        _rightWallCoyoteTimer = wallCoyoteTime;
                    }
                    else if (contact.normal.x < 0) {
                        _leftWallCoyoteTimer = wallCoyoteTime;
                        _touchingLeftWall = true;
                    }

                }
            }
        }
    }

    private void EvaluateTrigger(Collider2D collision) {
        if (HelperMethods.LayerInLayerMask(collision.gameObject.layer, climbableLayerMask))
            _canClimb = true;
    }

    private void CancelTriggers() {
        _canClimb = false;
    }

    // After touching the floor, jumping restrictions are reset
    private void ResetJump() {
        _doubleJumped = false;
        _lastWallJump = 0;
        _activeJumpCooldown = 0;
        _dashedInAir = false;
    }


    // Combat
    public override void TakeDamage(int damage) {
        base.TakeDamage(damage);
        uiManager.SetHealth(healthPoints, baseStats.healthPoints);
    }

    public override void Heal(int health) {
        base.Heal(health);
        uiManager.SetHealth(healthPoints, baseStats.healthPoints);
    }

    // Debug
    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    public void AttackTypeChoice(InputAction.CallbackContext context) {
        if (!context.performed) return;

        var value = _attackTypeChoice.ReadValue<float>();
        var index = Mathf.Clamp(
            Mathf.RoundToInt(value),
            0,
            System.Enum.GetValues(typeof(AttackType)).Length - 1);

        var chosenAttackType = (AttackType)index;
        if (chosenAttackType == _activeAttackType) return;
        if (chosenAttackType == AttackType.Ranged && rangedWeapon == null) return; // Possibly add a tooltip or something

        SetActiveAttackType(chosenAttackType);

    }
    private void SetActiveAttackType(AttackType attackType) {
        _activeAttackType = attackType;
        uiManager.SetActiveAttackType(attackType);
    }
}
}