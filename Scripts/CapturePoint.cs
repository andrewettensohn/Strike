using Godot;
using System;
using System.Collections.Generic;

public partial class CapturePoint : Area2D
{

	public List<Unit> PlayerShipsOnPoint = new List<Unit>();

	public List<Unit> EnemyShipsOnPoint = new List<Unit>();

	public bool DoesPlayerOwnPoint => PlayerShipsOnPoint.Count > EnemyShipsOnPoint.Count;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnShipEntered(Node2D node)
	{
		if (node as Unit == null) return;
		
		Unit unit = (Unit)node;

		if (unit.IsPlayerSide)
		{
			PlayerShipsOnPoint.Add(unit);
		}
		else
		{
			EnemyShipsOnPoint.Add(unit);
		}
	}

	public void OnShipExitted(Node2D node)
	{
		if (node as Unit == null) return;
		
		Unit unit = (Unit)node;

		if (unit.IsPlayerSide)
		{
			PlayerShipsOnPoint.Remove(unit);
		}
		else
		{
			EnemyShipsOnPoint.Remove(unit);
		}
	}
}
