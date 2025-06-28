using System;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Creatures.Player
{

public class PlayerMain : MonoBehaviour
{
    // controls
    public Rigidbody2D rb;
    public int jumpStrength;
    public float jumpCooldown = 0.2f;
    public float baseMovementSpeed = 3;
    private float _movementSpeed;
        // jumping
    private bool _onGround;
    private float _activeJumpCooldown;
    private bool _doubleJumped;
            // wall jumping
    private bool _touchingRightWall;
    private bool _touchingLeftWall;
    private int _lastWallJump; // -1: left wall, 1: right wall, 0: no
    // dashing
    public float dashStrength = 15f;
    public float dashCooldown = 1f;
    public float dashDuration = 0.2f; // duration of the dash
    private float _activeDashCooldown;
    private float _activeDashDuration;
    private bool _dashedInAir; // in air, only one dash possible
        // dash implementation
    private Vector2 _dashVelocity;
    private bool _isDashing;
    // climbing
    private bool _canClimb;
    // stats
    public int healthPoints;
    // input
    private InputSystemActions _inputActions;
    private InputAction _jump;
    private InputAction _move;
    private Vector2 _moveInput;
    private InputAction _sprint;
    private InputAction _dash;
    // visual && animations
    private bool _isFacingRight = true; // used for flipping the sprite
    public Animator animator;
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
    }

    private void OnDisable() {
        _jump.Disable();
        _move.Disable();
    }

    private void Start() {
        _movementSpeed = baseMovementSpeed;
    }

    private void Update() {
        HandleMovement();
        SyncAnimator();
    }

    private void SyncAnimator() {
        animator.SetBool("OnGround", _onGround);
        animator.SetBool("TouchingRightWall", _touchingRightWall);
        animator.SetBool("TouchingLeftWall", _touchingLeftWall);
        animator.SetBool("Moving", _moveInput.x != 0);
    }

    private void HandleMovement() {
        if (_canClimb) {
            rb.gravityScale = 0;
            rb.linearVelocity = new Vector2(_moveInput.x * _movementSpeed, _moveInput.y * _movementSpeed);
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
            rb.linearVelocity = new Vector2(Math.Sign(_moveInput.x) * _movementSpeed, rb.linearVelocity.y);
        }
        _activeJumpCooldown = Math.Max(_activeJumpCooldown - Time.deltaTime, 0);
        _activeDashCooldown = Math.Max(_activeDashCooldown - Time.deltaTime, 0);
    }

    public void Attack() {

    }

    private void Jump(InputAction.CallbackContext context) {
        if (!CanJump()) return;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpStrength);
        _activeJumpCooldown = jumpCooldown;

        if (_onGround) {
            _lastWallJump = 0;
        } else if (!CanWallJump()){
            _doubleJumped = true;
        } else if (_touchingRightWall) {
            _lastWallJump = 1;
            _doubleJumped = false;
        } else if (_touchingLeftWall) {
            _lastWallJump = -1;
            _doubleJumped = false;
        }

    }

    // public as it will be displayed in HUD
    public bool CanJump() {
        return (CanWallJump() || _onGround || (GameManager.Instance.SkillData.IsUnlocked(Skill.DoubleJump) && !_doubleJumped))
               && _activeJumpCooldown == 0
               && !_isDashing;
    }

    private bool CanWallJump() {
        return _touchingLeftWall && _lastWallJump != -1 ||
               _touchingRightWall && _lastWallJump != 1;
    }

    private void Dash(InputAction.CallbackContext context) {
        if (context.performed) {
            if (!CanDash()) return;

            _isDashing = true;
            _dashedInAir = !_onGround;
            _activeDashCooldown = dashCooldown;
            _activeDashDuration = dashDuration;

            _dashVelocity = new Vector2(_isFacingRight ? dashStrength : -dashStrength, 0);
            rb.linearVelocity = _dashVelocity;
        }
    }

    private bool CanDash() {
        return _activeDashCooldown <= 0 && (_onGround || !_dashedInAir);
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
        if (context.performed) _movementSpeed = baseMovementSpeed * 2;
        else if (context.canceled) _movementSpeed = baseMovementSpeed;
    }

    private void OnCollisionStay2D(Collision2D collision) {
        EvaluateCollision(collision);
    }
    private void OnCollisionExit2D(Collision2D collision) {
        CancelCollisions(collision);
    }
    private void OnTriggerStay2D(Collider2D collision) {
        EvaluateTrigger(collision);
    }
    private void OnTriggerExit2D(Collider2D collision) {
        CancelTriggers();
    }

    // silly little function name
    private void EvaluateCollision(Collision2D collision) {
        var layerName = LayerMask.LayerToName(collision.gameObject.layer);
        switch (layerName)
        {
            case "Wall":
                foreach (var contact in collision.contacts)
                {
                    if (contact.normal.y > 0.5f)
                    {
                        _onGround = true;
                        ResetJump();
                    }

                    // still using Math.Abs if we want to add more functionality for wall touch later
                    if (Math.Abs(contact.normal.x) > 0.5f)
                    {
                        if (contact.normal.x > 0) _touchingRightWall = true;
                        else if (contact.normal.x < 0) _touchingLeftWall = true;

                    }
                }
                break;
        }
    }

    private void CancelCollisions(Collision2D collision) {
        _onGround = false;
        _touchingRightWall = false;
        _touchingLeftWall = false;
    }

    private void EvaluateTrigger(Collider2D collision) {
        var layerName = LayerMask.LayerToName(collision.gameObject.layer);
        switch (layerName)
        {
            case "Climbable":
                _canClimb = true;
                break;
        }
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
}
}