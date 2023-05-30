using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class EnemyCommander : Node
{
	

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	//Ships that are closer get a higher value, ships with a more important class get a higher value

	public Unit GetTargetForUnit(Unit enemyUnit, List<Unit> playerShips)
	{
		if(!playerShips.Any())
		{
			return null;
		}

		// First will be furtherst away
		List<Unit> playerShipsByDistance = playerShips.OrderByDescending(x => x.GlobalPosition.DistanceTo(enemyUnit.GlobalPosition)).ToList();
		Dictionary<Unit, int> targetHeirarchy = new Dictionary<Unit, int>();

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
			else if(unit.ShipClass == ShipClass.Destroyer)
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
		
		return enemyShips.OrderByDescending(x => x.GlobalPosition.DistanceTo(enemyUnit.GlobalPosition))
		.ThenBy(x => x.Health)
		.FirstOrDefault();
	}

}
