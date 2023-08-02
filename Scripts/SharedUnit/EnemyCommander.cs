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
		EnemyFleetComp fleetComp = GetFleetComp();

		int picketNeeded = fleetComp.PicketCount - _levelManager.EnemyUnits.Count(x => x.ShipClass == ShipClass.Picket);
		SpawnEnemyShipsForClass(_levelManager.EnemyPicketScene, picketNeeded);

		int repairNeeded = fleetComp.RepairCount - _levelManager.EnemyUnits.Count(x => x.ShipClass == ShipClass.Repair);
		SpawnEnemyShipsForClass(_levelManager.EnemyRepairScene, repairNeeded);

		int cruiserNeeded = fleetComp.CrusierCount - _levelManager.EnemyUnits.Count(x => x.ShipClass == ShipClass.Crusier);
		SpawnEnemyShipsForClass(_levelManager.EnemyCruiserScene, cruiserNeeded);

		int droneNeeded = fleetComp.DroneControlCount - _levelManager.EnemyUnits.Count(x => x.ShipClass == ShipClass.DroneControl);
		SpawnEnemyShipsForClass(_levelManager.EnemyDroneControlScene, droneNeeded);

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

	private EnemyFleetComp GetFleetComp()
	{
		EnemyFleetCompType enemyFleetCompType = GetFleetCompType();
		
		if(enemyFleetCompType == EnemyFleetCompType.Balanced)
		{
			return new EnemyFleetComp
			{
				CrusierCount = 3,
				PicketCount = 3,
				RepairCount = 1,
				DroneControlCount = 1
			};
		}
		else if(enemyFleetCompType == EnemyFleetCompType.CruiserHeavy)
		{
			return new EnemyFleetComp
			{
				CrusierCount = 5,
				PicketCount = 1,
				RepairCount = 1,
				DroneControlCount = 1
			};
		}
		else if(enemyFleetCompType == EnemyFleetCompType.PicketSwarm)
		{
			return new EnemyFleetComp
			{
				CrusierCount = 0,
				PicketCount = 8,
				RepairCount = 0,
				DroneControlCount = 0
			};
		}
		else if(enemyFleetCompType == EnemyFleetCompType.DroneHeavy)
		{
			return new EnemyFleetComp
			{
				CrusierCount = 1,
				PicketCount = 2,
				RepairCount = 1,
				DroneControlCount = 4
			};
		}
		else if(enemyFleetCompType == EnemyFleetCompType.NoDrones)
		{
			return new EnemyFleetComp
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
			return new EnemyFleetComp();
		}
	}

	// Analyze the player's ships and set a fleet comp that is ideal for dealing with what the player is using.
	private EnemyFleetCompType GetFleetCompType()
	{
		int cruiserCount = 0;
		int repairCount = 0;
		int picketCount = 0;
		int droneControlCount = 0;

		foreach(Unit unit in _levelManager.PlayerUnits)
		{
			if(unit.ShipClass == ShipClass.Picket)
			{
				picketCount += 1;
			}
			else if(unit.ShipClass == ShipClass.Repair)
			{
				repairCount += 1;
			}
			else if(unit.ShipClass == ShipClass.Crusier)
			{
				cruiserCount += 1;
			}
			else if(unit.ShipClass == ShipClass.DroneControl)
			{
				droneControlCount += 1;
			}
		}

		if(picketCount > 3)
		{
			return EnemyFleetCompType.CruiserHeavy;
		}
		else if(droneControlCount >= 3)
		{
			return EnemyFleetCompType.PicketSwarm;
		}
		else if(cruiserCount > 3)
		{
			return EnemyFleetCompType.DroneHeavy;
		}
		else
		{
			return EnemyFleetCompType.Balanced;
		}
	}

}
