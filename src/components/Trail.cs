using Godot;

public partial class Trail : Line2D
{
    [Export] public int DefaultPointCount = 10;
    [Export] public PhysicsLerpPos LerpPos; // MAKE SURE LerpPos node is rendered before the trail

    public int? PointCount;


    public override void _Ready()
    {
        PointCount ??= DefaultPointCount;
    }

    public override void _Process(double delta)
    {
        AddPoint(LerpPos.LerpedPosition, 0);
        if (GetPointCount() > PointCount) RemovePoint(GetPointCount() - 1);
    }
}