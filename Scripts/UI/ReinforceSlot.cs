using Godot;
using System;

public partial class ReinforceSlot : Control
{
	[Export]
	public ShipClass ShipClass;

	private LevelManager _levelManager;

	private bool _isHovered;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_levelManager = GetTree().Root.GetNode<LevelManager>("Level");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("ui_select") && _isHovered)
		{
			_levelManager.ReinforcePlayerShip(ShipClass);
		}
	}

	public void Hovered()
	{
		_isHovered = true;
	}

	public void Unhovered()
	{
		_isHovered = false;
	}
}
