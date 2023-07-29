using Godot;
using System;

public partial class StrikeAudioPlayer : AudioStreamPlayer
{
	[Export]
	public AudioStream SetTargetSoundClip;

	[Export]
	public AudioStream WarpInSoundClip;

	[Export]
	public AudioStream ReinforceSoundClip;

	[Export]
	public AudioStream RetreatClickedSoundClip;

	[Export]
	public AudioStream ShipDestroyedSoundClip;

	private GameManager _gameManager;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_gameManager = GetNode<GameManager>("/root/GameManager");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		VolumeDb = _gameManager.SoundEffectsVolumeLevel;
	}

	public void PlayAudio(AudioStream audio)
	{
		Playing = false;
		Stream = audio;
		Playing = true;
	}
}
