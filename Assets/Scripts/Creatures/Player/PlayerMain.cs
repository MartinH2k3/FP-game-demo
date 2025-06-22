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
    public bool canJump;
    public float baseMovementSpeed = 3;
    private float _movementSpeed;
    private bool _onGround;
    // stats
    public int healthPoints;
    // input
    public InputSystemActions inputActions;
    private InputAction _jump;
    private InputAction _move;
    private Vector2 _moveInput;

    // Creating helper instances
    private void Awake()
    {
        inputActions = new InputSystemActions();
    }

    private void OnEnable()
    {
        _jump = inputActions.Player.Jump;
        _jump.Enable();
        _jump.performed += Jump;
        _move = inputActions.Player.Move;
        _move.Enable();
        _move.performed += Move;
        _move.canceled += Stop;
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
        transform.Translate(_moveInput * Time.deltaTime);
    }

    public void Attack()
    {

    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (_onGround)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpStrength);
        }
    }

    private void Move(InputAction.CallbackContext context)
    {
        _moveInput = new Vector2(context.ReadValue<Vector2>().x, 0) * _movementSpeed;
    }

    private void Stop(InputAction.CallbackContext context)
    {
        _moveInput = Vector2.zero;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        _onGround = TouchesGrass(collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        _onGround = false;
    }

    // silly little function name
    private bool TouchesGrass(Collision2D collision)
    {
        foreach (var contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                return true;
            }
        }

        return false;
    }

}
}