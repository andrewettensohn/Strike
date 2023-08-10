using Godot;
using System;

public partial class ToolTipInfo : Node
{

	[Export]
	public string Title;

	[Export]
	public string Stats;

	[Export]
	public string Description;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
