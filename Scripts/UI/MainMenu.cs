using Godot;
using System;

public partial class MainMenu : Node2D
{
	[Export]
	public PackedScene ProtoSkirmishPreMatch;


	private Panel _levelSelectPanel;

	private Panel _optionsMenu;

	GameManager _gameManager;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_levelSelectPanel = GetNode<Panel>("LevelSelectPanel");
		_optionsMenu = GetNode<Panel>("OptionsMenu");
		_gameManager = GetNode<GameManager>("/root/GameManager");

		_levelSelectPanel.Visible = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}


	public void ExitGame()
    {
        GetTree().Quit();
    }

	public void PlayProtoLevel()
	{
		_gameManager.GotoScene("res://Levels/ProtoSkirmishPreMatch.tscn");	
	}

	public void ToggleLevelSelectPanel()
	{
		_optionsMenu.Visible = false;
		_levelSelectPanel.Visible = !_levelSelectPanel.Visible;
	}
}