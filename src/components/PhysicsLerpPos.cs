using Godot;

public partial class PhysicsLerpPos : Marker2D
{
    private RigidBody2D _parent;
    private Vector2 _savedLastPos;
    private Vector2 _lastPos;
    public Vector2 LerpedPosition { get; private set; }

    public override void _Ready()
    {
        _parent = GetParent<RigidBody2D>();
        _lastPos = GetOffsetPos();
        _savedLastPos = GetOffsetPos();
        LerpedPosition = GetOffsetPos();
    }

    public override void _Process(double delta)
    {
        var currentPos = GetOffsetPos();
        if (_lastPos != currentPos) _savedLastPos = _lastPos;
        _lastPos = currentPos;

        LerpedPosition = _savedLastPos.Lerp(currentPos,
            (float)Engine.GetPhysicsInterpolationFraction());
    }


    private Vector2 GetOffsetPos()
    {
        return _parent.GlobalPosition + Position;
    }
}