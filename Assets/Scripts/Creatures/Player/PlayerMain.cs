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
    private bool _onGround;
    // stats
    public int healthPoints;
    // input
    public InputSystemActions inputActions;
    private InputAction _jump;

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
    }

    private void OnDisable()
    {
        _jump.Disable();
    }

    void Start()
    {

    }

    void Update()
    {

    }

    public void Attack()
    {

    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && _onGround)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpStrength);
        }
    }


    private void OnCollisionStay2D(Collision2D collision)
    {
        _onGround = TouchesGrass(collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        Debug.Log("OnCollisionExit2D");
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