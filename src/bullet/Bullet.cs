using Godot;

public partial class Bullet : RigidBody2D
{
    private Trail _trail;
    private CollisionShape2D _collisionShape;
    private Sprite2D _sprite;

    public override void _Ready()
    {
        // ContactMonitor = true;
        // MaxContactsReported = 10;
        _trail = GetNode<Line2D>("Trail") as Trail;
        _collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
        _sprite = GetNode<Sprite2D>("Sprite2D");
    }

    public void SetScale(float scale)
    {
        _collisionShape.Scale = Vector2.One * 0.4f * scale;
        _sprite.Scale = Vector2.One * 0.4f * scale;
        _trail.Width = 7.0f * scale;
        _trail.PointCount = _trail.DefaultPointCount * (int)scale;
    }

    public void _on_body_entered(Node body)
    {
        // QueueFree();
    }
}