using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class UILayer : CanvasLayer
{
	private Panel _reinforceDetails;
	private LevelManager _levelManager;
	private RichTextLabel _reinforcePoints;
	private RichTextLabel _missionTimerText;
	private RichTextLabel _message;
	private RichTextLabel _objectiveText;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_reinforceDetails = GetNode<Panel>("ReinforceDetails");

		_reinforcePoints = GetNode<Panel>("ReinforceDetails").GetNode<RichTextLabel>("ReinforcePointsText");

		_levelManager = GetTree().Root.GetNode<LevelManager>("Level");

		_missionTimerText = GetNode<RichTextLabel>("TimerText");

		_message = GetNode<RichTextLabel>("Message");
		_message.Visible = false;

		_objectiveText = GetNode<RichTextLabel>("ObjectiveText");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		_reinforcePoints.Text = $"Reinforce - {_levelManager.PlayerReinforcePoints}";

		HandleMissionTimerText();
		HandleObjectiveText();
	}

	public void HandleMissionTimerText()
	{
		_missionTimerText.Text = $"{Math.Round(_levelManager.GameMode.MissionTimer.TimeLeft)} {_levelManager.GameMode.TimerAdditionalText}";
	}

	public void HandleObjectiveText()
	{
		_objectiveText.Text = _levelManager.GameMode.ObjectiveText;
	}

	public async Task DisplayMessage(string message)
	{
		_message.Text = message;
		_message.Visible = true;

		await ToSignal(GetTree().CreateTimer(5), "timeout");

		_message.Visible = false;
	}
}
