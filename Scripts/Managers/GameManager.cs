using Godot;
using System;

public partial class GameManager : Node
{
	public float MusicVolumeLevel = -20f;
    public float SoundEffectsVolumeLevel = -20f;

	public PostMatchSummary LastMatchSummary = new PostMatchSummary();

	public MatchOptions MatchOptions = new MatchOptions();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
