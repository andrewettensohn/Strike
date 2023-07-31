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

		HandleEnemyAI();
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
		GetTree().ChangeSceneToPacked(PostMatchScene);
	}

	private void HandleEnemyAI()
	{
		if(CanSpawnUnits && _levelManager.EnemyUnits.Count < 8)
		{
			CanSpawnUnits = false;
			HandleEnemyReinforce();
		}
	}

	private void HandleEnemyReinforce()
	{
		//Ideal comp: 4 picket, 2 repair, 2 cruisers
		int picketNeeded = 4 - _levelManager.EnemyUnits.Count(x => x.ShipClass == ShipClass.Picket);
		SpawnEnemyShipsForClass(_levelManager.EnemyPicketScene, picketNeeded);

		int repairNeeded = 2 - _levelManager.EnemyUnits.Count(x => x.ShipClass == ShipClass.Repair);
		SpawnEnemyShipsForClass(_levelManager.EnemyRepairScene, repairNeeded);

		int cruiserNeeded = 2 - _levelManager.EnemyUnits.Count(x => x.ShipClass == ShipClass.Crusier);
		SpawnEnemyShipsForClass(_levelManager.EnemyCruiserScene, cruiserNeeded);

		CanSpawnUnits = true;
	}

	private void SpawnEnemyShipsForClass(PackedScene packedScene, int countNeeded)
	{
		if(countNeeded == 0) return;

		for(int i = 0; i < countNeeded; i++)
		{
			_levelManager.SpawnEnemyShip(packedScene);
		}
	}
}
