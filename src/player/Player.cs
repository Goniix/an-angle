using Godot;

namespace Angle.player;

public partial class Player : CharacterBody2D
{
    [Export] public AnimatedSprite2D AnimationNode;
    [Export] public GroundedHbox GroundedCollision;

    [Export] public int Gravity = 200;
    [Export] public int JumpLength = 200;

    [Export] public int WalkSpeed = 200;
    [Export] public int WalkTraction = 200;
    [Export] public float StopSpeed = 0.2f;

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
            movement.X = Mathf.Lerp(movement.X, 0, StopSpeed);
            AnimationNode.Play("idle");
        }
        else
        {
            if (Mathf.Sign(input.X) != Mathf.Sign(movement.X)) movement.X = 0;
            movement.X += input.X * WalkTraction * (float)delta;
            AnimationNode.Play("run");
        }
    }

    private void ApplyVerticalMovement(ref Vector2 movement, Vector2 input, double delta)
    {
        var jump = Input.IsActionPressed("jump");

        if (jump && GroundedCollision.Grounded) ApplyJump(ref movement, delta);


        ApplyGravity(ref movement, delta);
    }

    private void ApplyJump(ref Vector2 movement, double delta)
    {
        movement.Y = -JumpLength;
    }


    private void ApplyGravity(ref Vector2 movement, double delta)
    {
        movement.Y += Gravity * (float)delta;
    }

    private void ClampVelocity(ref Vector2 movement)
    {
        movement.X = Mathf.Clamp(movement.X, -WalkSpeed, WalkSpeed);
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
        var collided = MoveAndSlide();
        if (!collided) AnimationNode.Play("air");
        AnimationNode.FlipH = movement.X < 0;
    }
}