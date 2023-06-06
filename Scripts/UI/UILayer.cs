using Godot;
using System;
using System.Collections.Generic;

public partial class UILayer : CanvasLayer
{
	private Panel _reinforceDetails;
	private Panel _shipAbilityDetails;
	private LevelManager _levelManager;
	private Button _reinforceButton;
	private Button _shipAbilityButton;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_reinforceDetails = GetNode<Panel>("ReinforceDetails");
		_reinforceDetails.Visible = false;

		_shipAbilityDetails = GetNode<Panel>("ShipAbilityDetails");
		_shipAbilityDetails.Visible = false;

		_levelManager = GetTree().Root.GetNode<LevelManager>("Level");

		_reinforceButton = GetNode<Button>("ReinforceButton");

		_shipAbilityButton = _shipAbilityDetails.GetNode<Button>("TacticalAbilityButton");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		_shipAbilityDetails.Visible = _levelManager.SelectedShip != null;
		_reinforceButton.Text = $"Reinforce - {_levelManager.PlayerReinforcePoints}";
		
		HandleAbilityButtonText();
	}

	public void ReinforceButtonPressed()
	{
		_reinforceDetails.Visible = !_reinforceDetails.Visible;
	}

	public void RetreatButtonPressed()
	{
		_levelManager.SelectedShip.WarpOut(_levelManager.PlayerReinforceCorridorStart.GlobalPosition);
	}

	public void TacticalAbilityButtonPressed()
	{
		_levelManager.SelectedShip.IsTacticalAbilityPressed = true;

		//TODO: Handle displaying cooldown, make it so the button can't be pressed during cooldown or while ability is active
	}

	private void HandleAbilityButtonText()
	{
		if(_levelManager.SelectedShip != null && _levelManager.SelectedShip.IsTacticalInUse && _levelManager.SelectedShip.TacticalCooldownTimer != null && IsInstanceValid(_levelManager.SelectedShip.TacticalCooldownTimer))
		{
			_shipAbilityButton.Text = $"{Math.Round(_levelManager.SelectedShip.TacticalCooldownTimer.TimeLeft)}s";
		}
		else
		{
			_shipAbilityButton.Text = "Ability";
		}
	}
}
