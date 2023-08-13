using Godot;
using System;

public partial class HowToPlayPanel : Panel
{

	private TextureRect _howToPlay1;

	private TextureRect _howToPlay2;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_howToPlay1 = GetNode<TextureRect>("HowToPlay1");
		_howToPlay2 = GetNode<TextureRect>("HowToPlay2");
		_howToPlay2.Visible = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	public void OnPageOneButtonPressed()
	{
		_howToPlay1.Visible = true;
		_howToPlay2.Visible = false;
	}

	public void OnPageTwoButtonPressed()
	{
		_howToPlay1.Visible = false;
		_howToPlay2.Visible = true;
	}
}
