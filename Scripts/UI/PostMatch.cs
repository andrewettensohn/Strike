using Godot;
using System;

public partial class PostMatch : Control
{
	// [Export]
	// public PackedScene MainMenuScene;

	private RichTextLabel _matchSummary;

	private GameManager _gameManager;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_gameManager = GetNode<GameManager>("/root/GameManager");

		_matchSummary = GetNode<Panel>("Panel").GetNode<RichTextLabel>("MatchSummaryText");

		_matchSummary.Text = $"[indent] Player Score: {_gameManager.LastMatchSummary.PlayerScore} [/indent]";
		_matchSummary.Text += $"[indent] Enemy Score: {_gameManager.LastMatchSummary.EnemyScore} [/indent]";
		_matchSummary.Text += $"[indent] Ships Destroyed: {_gameManager.LastMatchSummary.ShipsDestroyed} [/indent]";
		_matchSummary.Text += $"[indent] Ships Lost: {_gameManager.LastMatchSummary.ShipsLost} [/indent]";
		// _matchSummary.Text += $"[indent] Capture Time: {_gameManager.LastMatchSummary.CaptureTime} [/indent]";
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void ReturnToMainMenu()
	{
		GetTree().ChangeSceneToFile("res://Levels/MainMenu.tscn");
	}

}
