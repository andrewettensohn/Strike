using Godot;
using System;

public partial class ReinforcePos : Area2D
{

	public bool IsAvailable = true;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Visible = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnPosEntered(Node2D body)
	{
		GD.Print(body.Name);
		IsAvailable = false;
	}

	public void OnPosExit(Node2D body)
	{
		IsAvailable = true;
	}
}
