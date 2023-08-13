using Godot;
using System;

public partial class MainMenu : Node2D
{
	[Export]
	public PackedScene ProtoSkirmishPreMatch;


	// private Panel _levelSelectPanel;

	private Panel _optionsMenu;

	private Panel _creditsPanel;

	private Panel _howToPlayPanel;

	private GameManager _gameManager;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//_levelSelectPanel = GetNode<Panel>("LevelSelectPanel");
		_optionsMenu = GetNode<CanvasLayer>("CanvasLayer").GetNode<Panel>("OptionsMenu");
		_creditsPanel = GetNode<CanvasLayer>("CanvasLayer").GetNode<Panel>("CreditsPanel");
		_howToPlayPanel = GetNode<CanvasLayer>("CanvasLayer").GetNode<Panel>("HowToPlayPanel");
		_gameManager = GetNode<GameManager>("/root/GameManager");

		//_levelSelectPanel.Visible = false;
		_creditsPanel.Visible = false;
		_howToPlayPanel.Visible = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}


	public void ExitGame()
    {
        GetTree().Quit();
    }

	public void Play()
	{
		_gameManager.GotoScene("res://Levels/PreMatch.tscn");
	}

	public void ToggleCreditsScreen()
	{
		if (_creditsPanel.Visible)
		{
			_creditsPanel.Visible = false;
		}
		else
		{
			_creditsPanel.Visible = true;
			_optionsMenu.Visible = false;
			_howToPlayPanel.Visible = false;
		}
	}

	public void ToggleOptionsScreen()
	{
		if (_optionsMenu.Visible)
		{
			_optionsMenu.Visible = false;
		}
		else
		{
			_creditsPanel.Visible = false;
			_optionsMenu.Visible = true;
			_howToPlayPanel.Visible = false;
		}
	}
	
	public void ToggleHowToPlayScreen()
	{
		if (_howToPlayPanel.Visible)
		{
			_howToPlayPanel.Visible = false;
		}
		else
		{
			_creditsPanel.Visible = false;
			_optionsMenu.Visible = false;
			_howToPlayPanel.Visible = true;
		}
	}
}
