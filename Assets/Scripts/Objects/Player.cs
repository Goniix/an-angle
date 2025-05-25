using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(BoxProjector))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class Player : MonoBehaviour
{
    private static readonly int AirBorne = Animator.StringToHash("AirBorne");
    private static readonly int PlayerInput = Animator.StringToHash("PlayerInput");

    //WALK CONSTANTS
    public int walkSpeed;
    public float walkTraction;
    public float groundFriction;
    public float airFriction;

    //JUMP CONSTANTS
    public int jumpStrength;
    public int jumpApexTresHold;

    //TIMERS
    public float coyoteTime;
    public float jumpBufferTime;

    //COMPONENTS
    private BoxProjector _boxProjector;
    private Rigidbody2D _rb;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;

    //RUNTIME COMPONENTS
    private Timer _coyoteTimer;
    private Timer _jumpBufferTimer;

    //INPUT ACTIONS
    private InputAction _moveAction;
    private InputAction _jumpAction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        _moveAction = InputSystem.actions.FindAction("Move");
        _moveAction.Enable();
        _jumpAction = InputSystem.actions.FindAction("Jump");
        _jumpAction.Enable();

        _rb = GetComponent<Rigidbody2D>();
        _boxProjector = GetComponent<BoxProjector>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        _jumpBufferTimer = this.AddComponent<Timer>();
        _jumpBufferTimer.duration = jumpBufferTime;
        _jumpBufferTimer.ResetFunc = () => _jumpAction.triggered;

        _coyoteTimer = this.AddComponent<Timer>();
        _coyoteTimer.duration = coyoteTime;
        _coyoteTimer.ResetFunc = IsGrounded;
    }

    private void FixedUpdate()
    {
        var input = GetInput();

        ApplyHorizontalMovement(input);
        ApplyFriction(input);
        _rb.gravityScale = GetGravityScale();
        ApplyVerticalMovement();

        _animator.SetBool(AirBorne, !IsGrounded());
        _animator.SetBool(PlayerInput, input.magnitude > 0);

        _spriteRenderer.flipX = input.x < 0;
    }

    private Vector2 GetInput()
    {
        return _moveAction.ReadValue<Vector2>();
    }

    private bool IsGrounded()
    {
        return _boxProjector.Collides();
    }

    private void ApplyHorizontalMovement(Vector2 input)
    {
        var targetSpeed = input.x * walkSpeed;
        //calculate difference between current velocity and desired velocity
        var speedDif = targetSpeed - _rb.linearVelocity.x;
        //change acceleration rate depending on situation
        var accelRate = Mathf.Abs(targetSpeed) > 0.01f ? walkTraction : walkTraction * 0.5f;
        //applies acceleration to speed difference, the raises to a set power so acceleration increases with higher speeds
        //finally multiplies by sign to reapply direction
        var movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, 2) * Mathf.Sign(speedDif);

        //applies force force to rigidbody, multiplying by Vector2.right so that it only affects X axis
        _rb.AddForce(Vector2.right * (movement * Time.deltaTime));
    }

    private void ApplyFriction(Vector2 input)
    {
        if (input.x == 0)
        {
            var amount = Mathf.Min(Mathf.Abs(_rb.linearVelocity.x), IsGrounded() ? groundFriction : airFriction);

            amount *= Mathf.Sign(_rb.linearVelocity.x);

            _rb.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
        }
    }

    private void ApplyVerticalMovement()
    {
        if (!_jumpBufferTimer.TimedOut() && !_coyoteTimer.TimedOut())
        {
            _jumpBufferTimer.TimeOut();
            _coyoteTimer.TimeOut();
            ApplyJump();
        }

        var jumpCancel = _rb.linearVelocity.y > 0 && !_jumpAction.IsPressed();
        if (jumpCancel) CancelJump();
    }

    private void ApplyJump()
    {
        _rb.linearVelocityY = Mathf.Max(0, _rb.linearVelocityY); //reset velocity if falling
        _rb.AddForce(Vector2.up * jumpStrength, ForceMode2D.Impulse);
    }

    private float GetGravityScale()
    {
        var isFalling = _rb.linearVelocity.y < 0;
        var isAtApex = Mathf.Abs(_rb.linearVelocity.y) < jumpApexTresHold;

        if (isFalling) return 2f;
        if (isAtApex) return 0.9f;

        return 1.0f;
    }

    private void CancelJump()
    {
        _rb.AddForce(Vector2.down * (_rb.linearVelocity.y * 0.1f), ForceMode2D.Impulse);
    }
}