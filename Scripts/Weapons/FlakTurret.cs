using Godot;
using System;

public partial class FlakTurret : Node
{

 	[Export]
    public PackedScene BulletScene;

    [Export]
    public float CoolDownTime;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public void FireBullet(Node2D target, TargetGroup myTargetGroup, TargetGroup hostileTargetGroup, Vector2 spawnPosition)
    {
        Bullet bullet = (Bullet)BulletScene.Instantiate();

        float xPos = (float)GD.RandRange(spawnPosition.X, spawnPosition.X);
        float yPos = (float)GD.RandRange(spawnPosition.Y, spawnPosition.Y);

        bullet.GlobalPosition = new Vector2(xPos, yPos);
        bullet.TargetPosition = target.GlobalPosition;
		bullet.HostileTargetGroup = hostileTargetGroup;

        bullet.AddToGroup(myTargetGroup.ToString());

        GetTree().Root.AddChild(bullet);
    }
}
