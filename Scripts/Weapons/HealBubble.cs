using Godot;
using System;

public partial class HealBubble : Area2D
{
	[Export]
    public float Speed;

	public Unit Target;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(double delta)
    {
		LookAt(Target.GlobalPosition);
		GlobalPosition = GlobalPosition.MoveToward(Target.GlobalPosition, (float)delta * Speed);

		if(GlobalPosition.IsEqualApprox(Target.GlobalPosition))
		{
			if(Target.Health < Target.MaxHealth)
			{
				Target.Health += 1;
			}
			QueueFree();
		}
		else if(!IsInstanceValid(Target))
		{
			QueueFree();
		}
    }
}
