using Godot;

public partial class Bullet : RigidBody2D
{
    public void SetScale(float scale)
    {
        GetNode<CollisionShape2D>("CollisionShape2D").Scale = Vector2.One * 0.4f * scale;
        GetNode<Sprite2D>("Sprite2D").Scale = Vector2.One * 0.4f * scale;
    }
}