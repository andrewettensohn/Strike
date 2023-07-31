using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public partial class StrikeDialougePlayer : AudioStreamPlayer
{

	[Export]
	public AudioStream[] UnitSelectedSoundClips;

	[Export]
	public AudioStream[] UnitMoveOrderSoundClips;

	[Export]
	public AudioStream[] UnitDamagedSoundClips;

	[Export]
	public AudioStream[] UnitAttackingSoundClips;

	[Export]
	public AudioStream[] UnitDestroyedSoundClips;

	[Export]
	public AudioStream[] ReinforceSoundClips;

	private GameManager _gameManager;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_gameManager = GetNode<GameManager>("/root/GameManager");

		VolumeDb = _gameManager.SoundEffectsVolumeLevel;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		VolumeDb = _gameManager.SoundEffectsVolumeLevel - 10;
	}

	public void PlayUnitSelectedSound()
	{
		AudioStream audio = UnitSelectedSoundClips[GD.Randi() % UnitSelectedSoundClips.Length];
		PlayAudio(audio);
	}

	public void PlayUnitMoveOrderSound()
	{
		AudioStream audio = UnitMoveOrderSoundClips[GD.Randi() % UnitMoveOrderSoundClips.Length];
		PlayAudio(audio);
	}

	public void PlayUnitDestroyedSound()
	{
		AudioStream audio = UnitDestroyedSoundClips[GD.Randi() % UnitDestroyedSoundClips.Length];
		PlayAudio(audio);
	}

	public void PlayUnitDamagedSoundClip()
	{
		// Does not always play, would get annoying
		uint chanceToPlay = GD.Randi() % 10;

		if(chanceToPlay >= 6)
		{
			AudioStream audio = UnitDamagedSoundClips[GD.Randi() % UnitDamagedSoundClips.Length];
			PlayAudio(audio);
		}
	}

	public void PlayUnitAttackingSoundClip()
	{
		// Does not always play, would get annoying
		uint chanceToPlay = GD.Randi() % 10;

		if(chanceToPlay >= 8)
		{
			AudioStream audio = UnitAttackingSoundClips[GD.Randi() % UnitAttackingSoundClips.Length];
			PlayAudio(audio);
		}
	}

	public void PlayUnitReinforceSoundClip()
	{
		// Does not always play, would get annoying
		uint chanceToPlay = GD.Randi() % 10;

		if(chanceToPlay >= 1)
		{
			AudioStream audio = ReinforceSoundClips[GD.Randi() % ReinforceSoundClips.Length];
			PlayAudio(audio);
		}
	}

	private void PlayAudio(AudioStream audioStream)
	{
		if(Playing) return;

		Playing = false;
		Stream = audioStream;
		Playing = true;
	}
}
