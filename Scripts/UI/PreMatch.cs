using Godot;
using System;

public partial class PreMatch : Control
{

	[Export]
	public CompressedTexture2D AstroAlleyCapturePreview;

	[Export]
	public CompressedTexture2D AstroAlleyDeathmatchPreview;

	[Export]
	public CompressedTexture2D BelowAstroCapturePreview;

	[Export]
	public CompressedTexture2D BelowAstroDeathmatchPreview;

	[Export]
	public CompressedTexture2D VoidDeathmatchPreview;

	public HSlider MatchLengthSlider;
	public HSlider PlayerReinforcePointsSlider;
	public HSlider EnemyReinforcePointsSlider;

	public RichTextLabel MatchLengthDescText;
	public RichTextLabel PlayerReinforcePointsDescText;
	public RichTextLabel EnemyReinforcePointsDescText;
	public RichTextLabel LevelName;

	private GameManager _gameManager;

	private string _selectedLevel = "res://Levels/AstroAlleyDeathmatch.tscn";

	private Sprite2D _levelPreviewSprite;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		MatchLengthSlider = GetNode<Panel>("Panel").GetNode<HSlider>("MatchLengthSlider");
		PlayerReinforcePointsSlider = GetNode<Panel>("Panel").GetNode<HSlider>("PlayerReinforcePointsSlider");
		EnemyReinforcePointsSlider = GetNode<Panel>("Panel").GetNode<HSlider>("EnemyReinforcePointsSlider");

		MatchLengthDescText = GetNode<Panel>("Panel").GetNode<RichTextLabel>("MatchLengthDescText");
		PlayerReinforcePointsDescText = GetNode<Panel>("Panel").GetNode<RichTextLabel>("PlayerReinforcePointsDescText");
		EnemyReinforcePointsDescText = GetNode<Panel>("Panel").GetNode<RichTextLabel>("EnemyReinforcePointsDescText");
		LevelName = GetNode<Panel>("Panel").GetNode<RichTextLabel>("LevelName");
		LevelName = GetNode<Panel>("Panel").GetNode<RichTextLabel>("LevelName");

		_levelPreviewSprite = GetNode<Panel>("Panel").GetNode<Sprite2D>("LevelPreview");
		_levelPreviewSprite.Texture = AstroAlleyDeathmatchPreview;

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
		_gameManager.GotoScene(_selectedLevel);
	}

	public void AsteroidAlleyDeathmatch()
	{
		_levelPreviewSprite.Texture = AstroAlleyDeathmatchPreview;
		LevelName.Text = "Asteroid Alley (Deathmatch)";
		_selectedLevel = "res://Levels/AstroAlleyDeathmatch.tscn";
	}

	public void AsteroidAlleyCapture()
	{
		_levelPreviewSprite.Texture = AstroAlleyCapturePreview;
		LevelName.Text = "Asteroid Alley (King of the Hill)";
		_selectedLevel = "res://Levels/AstroAlleyCapture.tscn";
	}

	public void VoidDeathmatch()
	{
		_levelPreviewSprite.Texture = VoidDeathmatchPreview;
		LevelName.Text = "Void (Deathmatch)";
		_selectedLevel = "res://Levels/VoidDeathmatch.tscn";
	}

	public void BelowAstroCapture()
	{
		_levelPreviewSprite.Texture = BelowAstroCapturePreview;
		LevelName.Text = "Below the Asteroids (King of the Hill)";
		_selectedLevel = "res://Levels/BelowAstroCapture.tscn";
	}

	public void BelowAstroDeatchmatch()
	{
		_levelPreviewSprite.Texture = BelowAstroDeathmatchPreview;
		LevelName.Text = "Below the Asteroids (Deathmatch)";
		_selectedLevel = "res://Levels/BelowAstroDeathmatch.tscn";
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

	public void OnEasyModeToggled(bool button_pressed)
	{
		_gameManager.MatchOptions.IsEasyMode = button_pressed;
		GD.Print(_gameManager.MatchOptions.IsEasyMode);
	}
}
