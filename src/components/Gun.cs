using Godot;

public partial class Gun : Node
{
    [Export] public int Power = 1000;
    [Export] public float Cooldown = 0.5f;
    [Export] public float FallOf = 0.5f;
    [Export] public float Size = 1f;
    private float _cooldownCounter;
    private PackedScene _bulletScene = GD.Load<PackedScene>("res://assets/bullet/bullet.tscn");

    public void Handle(Vector2 position, Vector2 direction)
    {
        _cooldownCounter = Cooldown;
        var bullet = _bulletScene.Instantiate<Bullet>();
        bullet.Position = position;
        bullet.GravityScale = FallOf;
        bullet.SetScale(Size);
        bullet.ApplyImpulse(direction * Power);
        AddChild(bullet);
    }


    private void HandleCooldown(double delta)
    {
        if (_cooldownCounter > 0) _cooldownCounter -= (float)delta;
    }

    public bool CanFire()
    {
        return _cooldownCounter <= 0;
    }

    public override void _Process(double delta)
    {
        HandleCooldown(delta);
    }
}