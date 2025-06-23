using System;
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
            // wall jumping
    public bool _touchingRightWall;
    public bool _touchingLeftWall;
    public int _lastWallJump; // -1: left wall, 1: right wall, 0: no wall jump
    // stats
    public int healthPoints;
    // input
    private InputSystemActions _inputActions;
    private InputAction _jump;
    private InputAction _move;
    private Vector2 _moveInput;
    private InputAction _sprint;

    // Creating helper instances
    private void Awake()
    {
        _inputActions = new InputSystemActions();
    }

    private void OnEnable()
    {
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

    private void OnDisable()
    {
        _jump.Disable();
        _move.Disable();
    }

    private void Start()
    {
        _movementSpeed = baseMovementSpeed;
    }

    private void Update()
    {
        rb.linearVelocity = new Vector2(_moveInput.x * _movementSpeed, rb.linearVelocity.y);
        _activeJumpCooldown = Math.Max(_activeJumpCooldown - Time.deltaTime, 0);

    }

    public void Attack()
    {

    }

    private void Jump(InputAction.CallbackContext context)
    {
        if ((CanWallJump() || _onGround) && _activeJumpCooldown == 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpStrength);
            _activeJumpCooldown = jumpCooldown;

            if (_onGround) {
                _lastWallJump = 0;
            } else if (_touchingRightWall) {
                _lastWallJump = 1;
            } else if (_touchingLeftWall) {
                _lastWallJump = -1;
            }
        }
    }

    private bool CanWallJump()
    {
        return _touchingLeftWall && _lastWallJump != -1 ||
               _touchingRightWall && _lastWallJump != 1;
    }

    private void Move(InputAction.CallbackContext context)
    {
        if (context.performed) _moveInput = new Vector2(context.ReadValue<Vector2>().x, 0);
        else if (context.canceled) _moveInput = Vector2.zero;
    }

    private void Sprint(InputAction.CallbackContext context)
    {
        if (context.performed) _movementSpeed = baseMovementSpeed * 2;
        else if (context.canceled) _movementSpeed = baseMovementSpeed;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        EvaluateCollision(collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        _onGround = false;
        _touchingRightWall = false;
        _touchingLeftWall = false;
    }


    // silly little function name
    private void EvaluateCollision(Collision2D collision)
    {
        if (collision.gameObject.layer != 3) return;

        foreach (var contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                _onGround = true;
                _lastWallJump = 0;
            }
            // still using Math.Abs if we want to add more functionality for wall touch later
            if (Math.Abs(contact.normal.x) > 0.5f)
            {
                if (contact.normal.x > 0) _touchingRightWall = true;
                else if (contact.normal.x < 0) _touchingLeftWall = true;
            }
        }
    }

}
}