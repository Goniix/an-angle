using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    //WALK CONSTANTS
    public int maxWalkSpeed = 100;
    public float walkTraction = 0.1f;
    public float stopTraction = 0.2f;

    //JUMP CONSTANTS
    public int gravity = 1400;
    public int jumpStrength = 600;
    public int jumpApexTresHold = 20;
    public float groundedDistance = 0.2f;

    //TIMERS
    public float coyoteTime = 0.1f;
    public float jumpBufferTime = 0.1f;
    private float _coyoteTimer;
    private InputAction _jumpAction;
    private float _jumpBufferTimer;

    //MOVEMENT VARIABLES
    // private Vector2 _velocity;

    //INPUT ACTIONS
    private InputAction _moveAction;

    //COMPONENTS
    private Rigidbody2D _rigidbody;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _moveAction = InputSystem.actions.FindAction("Move");
        _moveAction.Enable();
        _jumpAction = InputSystem.actions.FindAction("Jump");
        _jumpAction.Enable();

        _rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        var delta = Time.deltaTime;
        var movement = _rigidbody.linearVelocity;
        var input = GetInput();

        ApplyHorizontalMovement(ref movement, input);
        ApplyVerticalMovement(ref movement, delta);
        ClampVelocity(ref movement);

        // HandleGun();

        _rigidbody.MovePosition((Vector2)transform.position + movement * delta);
        // MoveAndSlide();
        //
        // if (!IsOnFloor()) AnimationNode.Play("air");
        // AnimationNode.FlipH = movement.X < 0;
    }

    private Vector2 GetInput()
    {
        return _moveAction.ReadValue<Vector2>();
    }

    private bool IsGrounded()
    {
        return _rigidbody.linearVelocity.y == 0 &&
               Physics.Raycast(transform.position, Vector3.down, out _, groundedDistance);
    }

    private void ApplyHorizontalMovement(ref Vector2 movement, Vector2 input)
    {
        if (input.x == 0)
        {
            movement.x = Mathf.Lerp(movement.x, 0, stopTraction);
            // AnimationNode.Play("idle");
        }
        else
        {
            if ((int)Mathf.Sign(input.x) != (int)Mathf.Sign(movement.x)) movement.x = 0;
            movement.x = Mathf.Lerp(movement.x, maxWalkSpeed * input.x, walkTraction);
            // AnimationNode.Play("run");
        }
    }

    private void ApplyVerticalMovement(ref Vector2 movement, double delta)
    {
        TickCoyoteTimer(delta);
        TickJumpBufferTimer(delta);
        ApplyGravity(ref movement, delta);
        if (_jumpBufferTimer > 0 && _coyoteTimer > 0)
        {
            _jumpBufferTimer = 0;
            _coyoteTimer = 0;
            ApplyJump(ref movement);
        }
    }

    private void TickCoyoteTimer(double delta)
    {
        if (IsGrounded()) _coyoteTimer = coyoteTime;
        else if (_coyoteTimer > 0) _coyoteTimer -= (float)delta;
    }

    private void TickJumpBufferTimer(double delta)
    {
        var jump = _jumpAction.WasPressedThisFrame();

        if (jump) _jumpBufferTimer = jumpBufferTime;
        else if (_jumpBufferTimer > 0) _jumpBufferTimer -= (float)delta;
    }

    private void ApplyJump(ref Vector2 movement)
    {
        movement.y = -jumpStrength;
    }

    private void ApplyGravity(ref Vector2 movement, double delta)
    {
        movement.y -= gravity * GetGravityScale(movement) * (float)delta;
    }


    private float GetGravityScale(Vector2 movement)
    {
        var jump = _jumpAction.WasPressedThisFrame();


        var isFalling = movement.y > 0;
        var isAtApex = Mathf.Abs(movement.y) < jumpApexTresHold;
        var jumpCancel = !isFalling && !jump;

        if (isAtApex) return 0.9f;
        if (jumpCancel || isFalling) return 2f;

        return 1.0f;
    }

    private void ClampVelocity(ref Vector2 movement)
    {
        movement.x = Mathf.Clamp(movement.x, -maxWalkSpeed, maxWalkSpeed);
    }
}