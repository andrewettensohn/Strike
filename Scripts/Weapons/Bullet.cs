using Godot;
using System;

public partial class Bullet : Area2D
{
	[Export]
    public float Speed;

	public Vector2 TargetPosition;

	public TargetGroup HostileTargetGroup;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(double delta)
    {
		//TODO: Bullet lifetime, check then queue free
		LookAt(TargetPosition);
		GlobalPosition = GlobalPosition.MoveToward(TargetPosition, (float)delta * Speed);

		if(GlobalPosition.IsEqualApprox(TargetPosition))
		{
			QueueFree();
		}
    }
}
