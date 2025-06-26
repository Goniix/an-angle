using Mirror;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(BoxProjector))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Ownership))]
public class PlayerBehavior : NetworkBehaviour
{
    private static readonly int AirBorne = Animator.StringToHash("AirBorne");
    private static readonly int PlayerInput = Animator.StringToHash("PlayerInput");

    [Header("Movement")] public int walkSpeed;

    public float walkTraction;
    public float groundFriction;
    public float airFriction;
    public int jumpStrength;

    [Header("Timers")] public float coyoteTime;

    public float jumpBufferTime;

    //COMPONENTS
    private BoxProjector _boxProjector;
    private Rigidbody2D _rb;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private Ownership _ownership;

    //RUNTIME COMPONENTS
    private Timer _coyoteTimer;
    private Timer _jumpBufferTimer;

    //INPUT ACTIONS
    private InputAction _moveAction;
    private InputAction _jumpAction;
    private Vector2 _clientInput;
    private bool _clientJumpPressed;

    [SyncVar] private bool _flipSpriteX;

    public void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _boxProjector = GetComponent<BoxProjector>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _ownership = GetComponent<Ownership>();
        _ownership.ownerId = (int)netId;
    }

    private void Update()
    {
        _spriteRenderer.flipX = _flipSpriteX;

        if (!isLocalPlayer) return;

        CmdUpdateInput(_moveAction.ReadValue<Vector2>(), _jumpAction.IsPressed());
        if (_jumpAction.triggered) CmdJumpPressed();
    }

    private void FixedUpdate()
    {
        if (!isServer) return;

        var input = GetInput();

        ApplyHorizontalMovement(input);
        ApplyFriction(input);
        _rb.gravityScale = GetGravityScale();
        ApplyVerticalMovement();

        _animator.SetBool(AirBorne, !IsGrounded());
        _animator.SetBool(PlayerInput, input.magnitude > 0);

        _flipSpriteX = input.x < 0;
    }

    [Client]
    public override void OnStartLocalPlayer()
    {
        _moveAction = InputSystem.actions.FindAction("Move");
        _moveAction.Enable();
        _jumpAction = InputSystem.actions.FindAction("Jump");
        _jumpAction.Enable();
    }

    [Server]
    public override void OnStartServer()
    {
        _coyoteTimer = this.AddComponent<Timer>();
        _coyoteTimer.duration = coyoteTime;
        _coyoteTimer.ResetFunc = IsGrounded;

        _jumpBufferTimer = this.AddComponent<Timer>();
        _jumpBufferTimer.duration = jumpBufferTime;
    }

    [Server]
    private Vector2 GetInput()
    {
        return _clientInput;
    }

    [Command]
    private void CmdUpdateInput(Vector2 input, bool jump)
    {
        _clientInput = input;
        _clientJumpPressed = jump;
    }

    [Command]
    private void CmdJumpPressed()
    {
        _jumpBufferTimer.Restart();
    }

    [Server]
    private bool IsGrounded()
    {
        return _boxProjector.Collides();
    }

    [Server]
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

    [Server]
    private void ApplyFriction(Vector2 input)
    {
        if (input.x == 0)
        {
            var amount = Mathf.Min(Mathf.Abs(_rb.linearVelocity.x), IsGrounded() ? groundFriction : airFriction);

            amount *= Mathf.Sign(_rb.linearVelocity.x);

            _rb.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
        }
    }

    [Server]
    private void ApplyVerticalMovement()
    {
        if (!_jumpBufferTimer.TimedOut() && !_coyoteTimer.TimedOut())
        {
            _jumpBufferTimer.TimeOut();
            _coyoteTimer.TimeOut();
            ApplyJump();
        }

        var jumpCancel = _rb.linearVelocity.y > 0 && !_clientJumpPressed;
        if (jumpCancel) CancelJump();
    }

    [Server]
    private void ApplyJump()
    {
        _rb.linearVelocityY = Mathf.Max(0, _rb.linearVelocityY); //reset velocity if falling
        _rb.AddForce(Vector2.up * jumpStrength, ForceMode2D.Impulse);
    }

    [Server]
    private float GetGravityScale()
    {
        var isFalling = _rb.linearVelocity.y < 0;

        return isFalling ? 2f : 1.0f;
    }

    [Server]
    private void CancelJump()
    {
        _rb.AddForce(Vector2.down * (_rb.linearVelocity.y * 0.1f), ForceMode2D.Impulse);
    }
}