using Godot;

namespace Angle.player;

public partial class GroundedHbox : Area2D
{
    public bool Grounded { get; private set; }

    private void OnBodyEntered(Node2D body)
    {
        Grounded = true;
    }

    private void OnBodyExited(Node2D body)
    {
        Grounded = false;
    }

    public override void _Ready()
    {
    }
}