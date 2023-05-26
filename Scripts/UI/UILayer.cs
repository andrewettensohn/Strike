using Godot;
using System;
using System.Collections.Generic;

public partial class UILayer : CanvasLayer
{
	private Panel _reinforceDetails;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_reinforceDetails = GetNode<Panel>("ReinforceDetails");
		_reinforceDetails.Visible = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void ReinforceButtonPressed()
	{
		_reinforceDetails.Visible = !_reinforceDetails.Visible;
	}
}
