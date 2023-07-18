using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class UILayer : CanvasLayer
{
	private Panel _reinforceDetails;
	private Panel _shipAbilityDetails;
	private LevelManager _levelManager;
	private Button _reinforceButton;
	private Button _shipAbilityButton;
	private RichTextLabel _doomsdayClockText;
	private RichTextLabel _message;
	private Timer _doomsDayTimer;

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

		_doomsdayClockText = GetNode<RichTextLabel>("DoomsdayClock");

		_doomsDayTimer = GetNode<Timer>("DoomsdayTimer");

		_doomsDayTimer.WaitTime = _levelManager.DoomsdayTime;
		_doomsDayTimer.Start();

		_message = GetNode<RichTextLabel>("Message");
		_message.Visible = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		_shipAbilityDetails.Visible = _levelManager.SelectedShip != null || _levelManager.AreMultipleUnitsSelected;
		_reinforceButton.Text = $"Reinforce - {_levelManager.PlayerReinforcePoints}";
		
		HandleAbilityButtonText();
		HandleDoomsDayTimerText();
	}

	public void OnDoomsdayTimerExpired()
	{
		_levelManager.GameMode.OnDoomsdayClockExpired();
	}

	public async Task DisplayMessage(string message)
	{
		_message.Text = message;
		_message.Visible = true;

		await ToSignal(GetTree().CreateTimer(5), "timeout");

		_message.Visible = false;
	}

	public void HandleDoomsDayTimerText()
	{
		_doomsdayClockText.Text = $"{Math.Round(_doomsDayTimer.TimeLeft)} UNTIL ENEMY REINFORCEMENTS";
	}

	public void ReinforceButtonPressed()
	{
		_reinforceDetails.Visible = !_reinforceDetails.Visible;
	}

	public void RetreatButtonPressed()
	{
		if(_levelManager.AreMultipleUnitsSelected)
		{
			_levelManager.HighlightedShips.ForEach(x => x.UnitMovement.WarpOut(_levelManager.PlayerReinforceCorridorStart.GlobalPosition));
		}
		else
		{
			_levelManager.SelectedShip.UnitMovement.WarpOut(_levelManager.PlayerReinforceCorridorStart.GlobalPosition);
		}
	}

	public void TacticalAbilityButtonPressed()
	{
		if(_levelManager.AreMultipleUnitsSelected)
		{
			_levelManager.HighlightedShips.ForEach(x => x.IsTacticalAbilityPressed = true);
		}
		else
		{
			_levelManager.SelectedShip.IsTacticalAbilityPressed = true;
		}

		//TODO: Handle displaying cooldown, make it so the button can't be pressed during cooldown or while ability is active
	}

	private void HandleAbilityButtonText()
	{
		if(_levelManager.SelectedShip != null && _levelManager.SelectedShip.IsTacticalInUse && _levelManager.SelectedShip.TacticalDurationTimer != null && IsInstanceValid(_levelManager.SelectedShip.TacticalDurationTimer))
		{
			_shipAbilityButton.Text = $"Active {Math.Round(_levelManager.SelectedShip.TacticalDurationTimer.TimeLeft)}s";
		}
		else if(IsInstanceValid(_levelManager.SelectedShip) && _levelManager.SelectedShip.IsTacticalOnCoolDown && IsInstanceValid(_levelManager.SelectedShip.TacticalCoolDownTimer))
		{
			_shipAbilityButton.Text = $"Cool Down {Math.Round(_levelManager.SelectedShip.TacticalCoolDownTimer.TimeLeft)}s";
		}
		else
		{
			_shipAbilityButton.Text = "Ability";
		}
	}
}
