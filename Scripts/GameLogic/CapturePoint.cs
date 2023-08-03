using Godot;
using System;
using System.Collections.Generic;

public partial class CapturePoint : Area2D
{
	[Export]
	public Texture2D NeutralTexture;

	[Export]
	public Texture2D PlayerTexture;

	[Export]
	public Texture2D EnemyTexture;

	public List<Unit> PlayerShipsOnPoint = new List<Unit>();

	public List<Unit> EnemyShipsOnPoint = new List<Unit>();

	private Sprite2D _sprite;

	private int _enemyScoreTick;

	private int _playerScoreTick;

	private LevelManager _levelManager;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_levelManager = GetTree().Root.GetNode<LevelManager>("Level");
		_sprite = GetNode<Sprite2D>("Sprite");
	}

	public override void _PhysicsProcess(double delta)
	{
		if(PlayerShipsOnPoint.Count > EnemyShipsOnPoint.Count)
		{
			_sprite.Texture = PlayerTexture;
			_enemyScoreTick= 0;
			_playerScoreTick += 1;
		}
		else if(EnemyShipsOnPoint.Count > PlayerShipsOnPoint.Count)
		{
			_sprite.Texture = EnemyTexture;
			_playerScoreTick = 0;
			_enemyScoreTick += 1;
		}
		else
		{
			_sprite.Texture = NeutralTexture;
		}

		if(_playerScoreTick >= 60)
		{
			_playerScoreTick = 0;
			_levelManager.GameMode.PlayerScore += 1;
		}
		else if(_enemyScoreTick >= 60)
		{
			_enemyScoreTick = 0;
			_levelManager.GameMode.EnemyScore += 1;
		}
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
