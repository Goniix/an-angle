using Godot;

public partial class Trail : Line2D
{
    [Export] public int DefaultPointCount = 10;
    public int? PointCount;
    private RigidBody2D _parent;
    private Vector2 _savedLastPos;
    private Vector2 _lastPos;


    public override void _Ready()
    {
        _parent = GetParent<RigidBody2D>();
        _lastPos = _parent.GlobalPosition;
        _savedLastPos = _parent.GlobalPosition;
        PointCount ??= DefaultPointCount;
    }

    public override void _Process(double delta)
    {
        var currentPos = _parent.GlobalPosition;
        if (_lastPos != currentPos) _savedLastPos = _lastPos;
        _lastPos = currentPos;

        var lerpPos = _savedLastPos.Lerp(currentPos,
            (float)Engine.GetPhysicsInterpolationFraction());

        AddPoint(lerpPos, 0);
        if (GetPointCount() > PointCount) RemovePoint(GetPointCount() - 1);
    }
}