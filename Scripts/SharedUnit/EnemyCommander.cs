using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class EnemyCommander : Node
{
	protected bool HasEnemySpawnedUnits;

	protected bool CanSpawnUnits;

	protected LevelManager _levelManager;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_levelManager = GetTree().Root.GetNode<LevelManager>("Level");

		CanSpawnUnits = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		HandleEnemyAI();
	}

	private void HandleEnemyAI()
	{
		if(CanSpawnUnits && _levelManager.EnemyUnits.Count < 8)
		{
			CanSpawnUnits = false;
			HandleEnemyReinforce();
		}
	}

	// Determine destination for units outside of just chasing the player, combat is always the priority but if there's no targets in range, go to a capture point
	public void GetCapturePointDestination()
	{
		//TODO: implement this
	}

	public Unit GetTargetForUnit(Unit enemyUnit, List<Unit> playerShips)
	{
		if(!playerShips.Any())
		{
			return null;
		}

		// First will be furtherst away
		List<Unit> playerShipsByDistance = playerShips.OrderByDescending(x => x.GlobalPosition.DistanceTo(enemyUnit.GlobalPosition)).ToList();
		Dictionary<Unit, int> targetHeirarchy = new Dictionary<Unit, int>();

		// Ships that are closer get a higher value, ships with a more important class get a higher value
		int distanceValue = 0;
		foreach(Unit unit in playerShipsByDistance)
		{
			int classValue = 0;
			distanceValue += 3;

			if(unit.ShipClass == ShipClass.Repair)
			{
				classValue = 4; 
			}
			else if(unit.ShipClass == ShipClass.Crusier)
			{
				classValue = 3;
			}
			else if(unit.ShipClass == ShipClass.Picket)
			{
				classValue = 2;
			}
			else if(unit.ShipClass == ShipClass.DroneControl)
			{
				classValue = 1;
			}

			targetHeirarchy.Add(unit, classValue + distanceValue);
		}

		Unit newTarget = targetHeirarchy.OrderByDescending(x => x.Value).FirstOrDefault().Key;

		return newTarget;
	}

	public Unit GetHealTargetForUnit(Unit enemyUnit, List<Unit> enemyShips)
	{
		if(!enemyShips.Any())
		{
			return null;
		}
		
		return enemyShips.OrderBy(x => x.Health).ThenBy(x => x.GlobalPosition.DistanceTo(enemyUnit.GlobalPosition)).FirstOrDefault();
	}

	private void HandleEnemyReinforce()
	{
		FleetComp desiredFleetComp = GetFleetComp();
		FleetComp currentComp = GetFleetCompStatsFromList(_levelManager.EnemyUnits);

		int picketNeeded = desiredFleetComp.PicketCount - currentComp.PicketCount;
		SpawnEnemyShipsForClass(_levelManager.EnemyPicketScene, picketNeeded);

		int repairNeeded = desiredFleetComp.RepairCount - currentComp.RepairCount;
		SpawnEnemyShipsForClass(_levelManager.EnemyRepairScene, repairNeeded);

		int cruiserNeeded = desiredFleetComp.CrusierCount - currentComp.CrusierCount;
		SpawnEnemyShipsForClass(_levelManager.EnemyCruiserScene, cruiserNeeded);

		int droneNeeded = desiredFleetComp.DroneControlCount - currentComp.DroneControlCount;
		SpawnEnemyShipsForClass(_levelManager.EnemyDroneControlScene, droneNeeded);

		CanSpawnUnits = true;
	}

	private void SpawnEnemyShipsForClass(PackedScene packedScene, int countNeeded)
	{
		if(countNeeded == 0 || _levelManager.EnemyUnits.Count >= 8) return;

		for(int i = 0; i < countNeeded; i++)
		{
			if(_levelManager.EnemyUnits.Count >= 8)
			{
				return;
			}

			_levelManager.SpawnEnemyShip(packedScene);
		}
	}

	private FleetComp GetFleetComp()
	{
		EnemyFleetCompType enemyFleetCompType = GetFleetCompType();
		
		if(enemyFleetCompType == EnemyFleetCompType.Balanced)
		{
			return new FleetComp
			{
				CrusierCount = 3,
				PicketCount = 3,
				RepairCount = 1,
				DroneControlCount = 1
			};
		}
		else if(enemyFleetCompType == EnemyFleetCompType.CruiserHeavy)
		{
			return new FleetComp
			{
				CrusierCount = 5,
				PicketCount = 1,
				RepairCount = 1,
				DroneControlCount = 1
			};
		}
		else if(enemyFleetCompType == EnemyFleetCompType.PicketSwarm)
		{
			return new FleetComp
			{
				CrusierCount = 0,
				PicketCount = 8,
				RepairCount = 0,
				DroneControlCount = 0
			};
		}
		else if(enemyFleetCompType == EnemyFleetCompType.DroneHeavy)
		{
			return new FleetComp
			{
				CrusierCount = 1,
				PicketCount = 2,
				RepairCount = 1,
				DroneControlCount = 4
			};
		}
		else if(enemyFleetCompType == EnemyFleetCompType.NoDrones)
		{
			return new FleetComp
			{
				CrusierCount = 2,
				PicketCount = 4,
				RepairCount = 2,
				DroneControlCount = 0
			};
		}
		else
		{
			GD.Print("No comp selected for enemy.");
			return new FleetComp();
		}
	}

	// Analyze the player's ships and set a fleet comp that is ideal for dealing with what the player is using.
	private EnemyFleetCompType GetFleetCompType()
	{
		FleetComp playerFleetComp = GetFleetCompStatsFromList(_levelManager.PlayerUnits);

		if(playerFleetComp.PicketCount > 3)
		{
			return EnemyFleetCompType.CruiserHeavy;
		}
		else if(playerFleetComp.DroneControlCount >= 3)
		{
			return EnemyFleetCompType.PicketSwarm;
		}
		else if(playerFleetComp.DroneControlCount > 3)
		{
			return EnemyFleetCompType.DroneHeavy;
		}
		else
		{
			return EnemyFleetCompType.Balanced;
		}
	}

	private FleetComp GetFleetCompStatsFromList(List<Unit> units)
	{
		FleetComp fleetComp = new FleetComp();

		foreach(Unit unit in _levelManager.PlayerUnits)
		{
			if(unit.ShipClass == ShipClass.Picket)
			{
				fleetComp.PicketCount += 1;
			}
			else if(unit.ShipClass == ShipClass.Repair)
			{
				fleetComp.RepairCount += 1;
			}
			else if(unit.ShipClass == ShipClass.Crusier)
			{
				fleetComp.CrusierCount += 1;
			}
			else if(unit.ShipClass == ShipClass.DroneControl)
			{
				fleetComp.DroneControlCount += 1;
			}
		}

		return fleetComp;
	}

}
