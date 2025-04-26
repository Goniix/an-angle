using System;
using Godot;

namespace Angle.player;

public partial class Player : CharacterBody2D
{
    [Export] public AnimatedSprite2D AnimationNode;

    //WALK CONSTANTS
    [Export] public int MaxWalkSpeed = 300;
    [Export] public float WalkTraction = 0.1f;
    [Export] public float StopTraction = 0.2f;

    //JUMP CONSTANTS
    [Export] public int Gravity = 1400;
    [Export] public int JumpStrength = 600;
    [Export] public int JumpApexTresHold = 20;
    [Export] public float CoyoteTime = 0.1f;
    private float _coyoteTimer;
    [Export] public float JumpBufferTime = 0.1f;
    private float _jumpBufferTimer;


    private static Vector2 GetInput()
    {
        var left = Input.IsActionPressed("left") ? 1 : 0;
        var right = Input.IsActionPressed("right") ? 1 : 0;

        return new Vector2(right - left, 0);
    }

    private void ApplyHorizontalMovement(ref Vector2 movement, Vector2 input, double delta)
    {
        if (input.X == 0)
        {
            movement.X = Mathf.Lerp(movement.X, 0, StopTraction);
            AnimationNode.Play("idle");
        }
        else
        {
            if (Mathf.Sign(input.X) != Mathf.Sign(movement.X)) movement.X = 0;
            movement.X = Mathf.Lerp(movement.X, MaxWalkSpeed * input.X, WalkTraction);
            AnimationNode.Play("run");
        }
    }

    private void ApplyVerticalMovement(ref Vector2 movement, Vector2 input, double delta)
    {
        TickCoyoteTimer(delta);
        TickJumpBufferTimer(delta);
        ApplyGravity(ref movement, delta);
        if (_jumpBufferTimer > 0 && _coyoteTimer > 0)
        {
            _jumpBufferTimer = 0;
            _coyoteTimer = 0;
            ApplyJump(ref movement, delta);
        }
    }

    private void TickCoyoteTimer(double delta)
    {
        if (IsOnFloor()) _coyoteTimer = CoyoteTime;
        else if (_coyoteTimer > 0) _coyoteTimer -= (float)delta;
    }

    private void TickJumpBufferTimer(double delta)
    {
        var jump = Input.IsActionJustPressed("jump");

        if (jump) _jumpBufferTimer = JumpBufferTime;
        else if (_jumpBufferTimer > 0) _jumpBufferTimer -= (float)delta;
    }

    private void ApplyJump(ref Vector2 movement, double delta)
    {
        movement.Y = -JumpStrength;
    }


    private void ApplyGravity(ref Vector2 movement, double delta)
    {
        movement.Y += Gravity * GetGravityScale(movement) * (float)delta;
    }

    private float GetGravityScale(Vector2 movement)
    {
        var jump = Input.IsActionPressed("jump");


        var isFalling = movement.Y > 0;
        var isAtApex = Math.Abs(movement.Y) < JumpApexTresHold;
        var jumpCancel = !isFalling && !jump;

        if (isAtApex) return 0.9f;
        if (jumpCancel || isFalling) return 2f;

        return 1.0f;
    }

    private void ClampVelocity(ref Vector2 movement)
    {
        movement.X = Mathf.Clamp(movement.X, -MaxWalkSpeed, MaxWalkSpeed);
    }


    public override void _Ready()
    {
        AnimationNode.Play("idle");
    }

    public override void _PhysicsProcess(double delta)
    {
        var movement = Velocity;
        var input = GetInput();

        ApplyHorizontalMovement(ref movement, input, delta);
        ApplyVerticalMovement(ref movement, input, delta);
        ClampVelocity(ref movement);

        Velocity = movement;
        MoveAndSlide();
        if (!IsOnFloor()) AnimationNode.Play("air");
        AnimationNode.FlipH = movement.X < 0;
    }
}