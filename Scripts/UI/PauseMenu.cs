using Godot;
using System;

public partial class PauseMenu : Panel
{
	private LevelManager _levelManager;
	private GameManager _gameManager;
	private HSlider _musicSlider;
	private HSlider _sfxSlider;
	private Timer _escDelayTimer;
	private bool _isEscBlocked;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_levelManager = GetTree().Root.GetNode<LevelManager>("Level");
		_gameManager = GetNode<GameManager>("/root/GameManager");
		_musicSlider = GetNode<HSlider>("MusicSlider");
        _sfxSlider = GetNode<HSlider>("SFXSlider");
		_escDelayTimer = GetNode<Timer>("EscDelayTimer");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if(!_isEscBlocked && Input.IsActionJustPressed("ui_cancel"))
        {
            OnResumeButtonPressed();
        }
	}

	public void OnPauseMenuOpened()
	{
		_isEscBlocked = true;
		_escDelayTimer.Start();

		Visible = true;
		GetTree().Paused = true;
	}

	public void OnResumeButtonPressed()
	{
		_isEscBlocked = false;
		Visible = false;
		GetTree().Paused = false;
	}

	public void OnMusicVolumeSlideEnded(bool value_changed)
    {
        if(!value_changed) return;

        _gameManager.MusicVolumeLevel = _musicSlider.Value;
    }

    public void OnEffectsSlideEnded(bool value_changed)
    {
        if(!value_changed) return;

        _gameManager.SoundEffectsVolumeLevel = _sfxSlider.Value;
    }

	public void ExitGame()
    {
        GetTree().Quit();
    }

	public void OnEscDelayTimerExpired()
	{
		_isEscBlocked = false;
		_escDelayTimer.Stop();
	}
}
