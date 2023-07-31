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
	private RichTextLabel _playerScoreText;
	private RichTextLabel _enemyScoreText;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_reinforceDetails = GetNode<Panel>("ReinforceDetails");

		_reinforcePoints = GetNode<Panel>("ReinforceDetails").GetNode<RichTextLabel>("ReinforcePointsText");

		_levelManager = GetTree().Root.GetNode<LevelManager>("Level");

		_missionTimerText = GetNode<Panel>("GameModeDetails").GetNode<RichTextLabel>("TimerText");
		_objectiveText = GetNode<Panel>("GameModeDetails").GetNode<RichTextLabel>("ObjectiveText");
		_enemyScoreText = GetNode<Panel>("GameModeDetails").GetNode<RichTextLabel>("EnemyScore");
		_playerScoreText = GetNode<Panel>("GameModeDetails").GetNode<RichTextLabel>("PlayerScore");

		_message = GetNode<RichTextLabel>("Message");
		_message.Visible = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		_reinforcePoints.Text = $"Reinforce - {_levelManager.GameMode.PlayerReinforcePoints}";

		HandleGameModeDetailsText();
	}

	public void HandleGameModeDetailsText()
	{
		_missionTimerText.Text = $"[center] {Math.Round(_levelManager.GameMode.MissionTimer.TimeLeft)} {_levelManager.GameMode.TimerAdditionalText} [/center]";
		_objectiveText.Text = $"[center] {_levelManager.GameMode.ObjectiveText} [/center]";
		_playerScoreText.Text = $"[center] Player  [indent]{_levelManager.GameMode.PlayerScore}[/indent] [/center]";
		_enemyScoreText.Text = $"[center] Enemy [indent]{_levelManager.GameMode.EnemyScore}[/indent] [/center]";
	}

	public async Task DisplayMessage(string message)
	{
		_message.Text = message;
		_message.Visible = true;

		await ToSignal(GetTree().CreateTimer(5), "timeout");

		_message.Visible = false;
	}
}
