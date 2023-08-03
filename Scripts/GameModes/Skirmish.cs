using Godot;
using System;
using System.Threading.Tasks;
using System.Linq;

public partial class Skirmish : GameMode
{

	[Export]
	public PackedScene PostMatchScene;

	[Export]
	public CapturePoint CapturePoint;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(MissionTimer.IsStopped() && !IsGameOver)
		{
			MissionTimer.WaitTime = _gameManager.MatchOptions.MatchLength;
			MissionTimer.Start();
		}

		if(IsGameOver && PostMatchTimer.IsStopped())
		{
			PostMatchTimer.Start();
		}
	}

	public override void OnEnemyShipDestroyed(Unit unit)
	{
		_gameManager.LastMatchSummary.ShipsDestroyed += 1;
		CapturePoint.EnemyShipsOnPoint.Remove(unit);
	}

	public override void OnPlayerShipDestroyed(Unit unit)
	{
		_gameManager.LastMatchSummary.ShipsLost += 1;
		CapturePoint.PlayerShipsOnPoint.Remove(unit);
	}

	public override async Task OnWin()
    {
		IsGameOver = true;
        await _levelManager.PlayerView.UILayer.DisplayMessage("MATCH OVER. VICTORY.");
    }

    public override async Task OnLose()
    {
		IsGameOver = true;
        await _levelManager.PlayerView.UILayer.DisplayMessage("MATCH OVER. DEFEAT.");
    }

    public override void OnMissionClockExpired()
    {
		MissionTimer.Stop();

        if(PlayerScore >= EnemyScore)
		{
			OnWin();
		}
		else
		{
			OnLose();
		}
    }

	public override void OnPostMatchTimerExpired()
	{
		_gameManager.LastMatchSummary.PlayerScore = PlayerScore;
		_gameManager.LastMatchSummary.EnemyScore = EnemyScore;
		GetTree().Root.GetTree().ChangeSceneToPacked(PostMatchScene);
	}
}
