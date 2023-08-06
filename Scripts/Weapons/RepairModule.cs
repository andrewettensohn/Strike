using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public partial class RepairModule : Node2D
{

	[Export]
	public PackedScene HealBubbleScene;

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

	public void RepairShip(Unit target)
	{
		HealBubble healBubble = (HealBubble)HealBubbleScene.Instantiate();

		healBubble.GlobalPosition = GlobalPosition;
		healBubble.Target = target;

		GetTree().Root.AddChild(healBubble);
	}
}
