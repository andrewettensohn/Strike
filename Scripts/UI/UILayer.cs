using Godot;
using System;
using System.Collections.Generic;

public partial class UILayer : CanvasLayer
{
	private Panel _reinforceDetails;
	private Panel _shipAbilityDetails;
	private LevelManager _levelManager;
	private Button _reinforceButton;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_reinforceDetails = GetNode<Panel>("ReinforceDetails");
		_reinforceDetails.Visible = false;

		_shipAbilityDetails = GetNode<Panel>("ShipAbilityDetails");
		_shipAbilityDetails.Visible = false;

		_levelManager = GetTree().Root.GetNode<LevelManager>("Level");

		_reinforceButton = GetNode<Button>("ReinforceButton");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		_shipAbilityDetails.Visible = _levelManager.SelectedShip != null;
		_reinforceButton.Text = $"Reinforce - {_levelManager.PlayerReinforcePoints}";
	}

	public void ReinforceButtonPressed()
	{
		_reinforceDetails.Visible = !_reinforceDetails.Visible;
	}

	public void RetreatButtonPressed()
	{
		_levelManager.SelectedShip.WarpOut(_levelManager.PlayerReinforceCorridorStart.GlobalPosition);
	}
}
