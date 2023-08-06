using Godot;
using System;

public partial class PreMatch : Control
{
	[Export]
	public PackedScene Level;

	public HSlider MatchLengthSlider;
	public HSlider PlayerReinforcePointsSlider;
	public HSlider EnemyReinforcePointsSlider;

	public RichTextLabel MatchLengthDescText;
	public RichTextLabel PlayerReinforcePointsDescText;
	public RichTextLabel EnemyReinforcePointsDescText;

	private GameManager _gameManager;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		MatchLengthSlider = GetNode<Panel>("Panel").GetNode<HSlider>("MatchLengthSlider");
		PlayerReinforcePointsSlider = GetNode<Panel>("Panel").GetNode<HSlider>("PlayerReinforcePointsSlider");
		EnemyReinforcePointsSlider = GetNode<Panel>("Panel").GetNode<HSlider>("EnemyReinforcePointsSlider");

		MatchLengthDescText = GetNode<Panel>("Panel").GetNode<RichTextLabel>("MatchLengthDescText");
		PlayerReinforcePointsDescText = GetNode<Panel>("Panel").GetNode<RichTextLabel>("PlayerReinforcePointsDescText");
		EnemyReinforcePointsDescText = GetNode<Panel>("Panel").GetNode<RichTextLabel>("EnemyReinforcePointsDescText");

		_gameManager = GetNode<GameManager>("/root/GameManager");

		_gameManager.MatchOptions = new MatchOptions
		{
			MatchLength = (int)MatchLengthSlider.Value,
			PlayerReinforcePoints = (int)PlayerReinforcePointsSlider.Value,
			EnemyReinforcePoints = (int)EnemyReinforcePointsSlider.Value
		};
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void StartMatchButtonPressed()
	{
		_gameManager.GotoScene("res://Levels/ProtoSkirmish.tscn");
	}

	public void OnMatchLengthSlideEnded(bool value_changed)
    {
        if(!value_changed) return;

        _gameManager.MatchOptions.MatchLength = (int)MatchLengthSlider.Value;

		MatchLengthDescText.Text = $"Match Length: {MatchLengthSlider.Value}";
    }

	public void OnPlayerReinforcePointsSlideEnded(bool value_changed)
    {
        if(!value_changed) return;

        _gameManager.MatchOptions.PlayerReinforcePoints = (int)PlayerReinforcePointsSlider.Value;

		MatchLengthDescText.Text = $"Player Reinforce Points: {PlayerReinforcePointsSlider.Value}";
    }

	public void OnEnemyReinforcePointsSlideEnded(bool value_changed)
    {
        if(!value_changed) return;

        _gameManager.MatchOptions.EnemyReinforcePoints = (int)EnemyReinforcePointsSlider.Value;

		MatchLengthDescText.Text = $"Enemy Reinforce Points: {EnemyReinforcePointsSlider.Value}";
    }
}
