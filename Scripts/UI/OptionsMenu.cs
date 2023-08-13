using Godot;
using System;

public partial class OptionsMenu : Panel
{
	private GameManager _gameManager;
	private HSlider _musicSlider;
	private HSlider _sfxSlider;
	private StrikeAudioPlayer _strikeAudioPlayer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_gameManager = GetNode<GameManager>("/root/GameManager");
		_musicSlider = GetNode<HSlider>("MusicSlider");
        _sfxSlider = GetNode<HSlider>("SFXSlider");
		_strikeAudioPlayer = GetNode<StrikeAudioPlayer>("StrikeAudioPlayer");

		_musicSlider.Value = _gameManager.MusicVolumeLevel;
		_sfxSlider.Value = _gameManager.SoundEffectsVolumeLevel;

		Visible = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	public void OnMusicVolumeSlideEnded(bool value_changed)
    {
        if(!value_changed) return;

        _gameManager.MusicVolumeLevel = (float)_musicSlider.Value;
    }

    public void OnEffectsSlideEnded(bool value_changed)
    {
        if(!value_changed) return;

        _gameManager.SoundEffectsVolumeLevel = (float)_sfxSlider.Value;

		_strikeAudioPlayer.PlayAudio(_strikeAudioPlayer.ReinforceSoundClip);
    }
}
