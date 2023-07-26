using Godot;
using System;
using System.Threading.Tasks;
using System.Linq;

public partial class Skirmish : GameMode
{

	[Export]
	public CapturePoint CapturePoint;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(MissionTimer.IsStopped() && !IsGameOver)
		{
			MissionTimer.WaitTime = 300f;
			MissionTimer.Start();
		}

		HandleEnemyAI();
	}

	public override void OnEnemyShipDestroyed(Unit unit)
	{
		CapturePoint.EnemyShipsOnPoint.Remove(unit);
	}

	public override void OnPlayerShipDestroyed(Unit unit)
	{
		CapturePoint.PlayerShipsOnPoint.Remove(unit);
	}

	public override async Task OnWin()
    {
		IsGameOver = true;
        await _levelManager.PlayerView.UILayer.DisplayMessage("Mission Status: SUCCESS");
    }

    public override async Task OnLose()
    {
		IsGameOver = true;
        await _levelManager.PlayerView.UILayer.DisplayMessage("Mission Status: FAILURE");
    }

    public override void OnMissionClockExpired()
    {
        if(CapturePoint.DoesPlayerOwnPoint)
		{
			OnWin();
		}
		else
		{
			OnLose();
		}
    }

	private void HandleEnemyAI()
	{
		//Race conditions galore! I'll probably need to keep track of spawned ships seperately
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
		//GD.Print(_levelManager.EnemyUnits.Count(x => x.ShipClass == ShipClass.Crusier));
	}

	private void SpawnEnemyShipsForClass(PackedScene packedScene, int countNeeded)
	{
		if(countNeeded == 0) return;

		for(int i = 0; i < countNeeded; i++)
		{
			//GD.Print("Spawned");
			_levelManager.SpawnEnemyShip(packedScene);
		}
	}
}