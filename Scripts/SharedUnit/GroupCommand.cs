using System;
using Godot;
using System.Linq;
using System.Collections.Generic;

public class GroupCommand
{
    private LevelManager _levelManager;
    private PlayerView _playerView;

    public GroupCommand(LevelManager levelManager, PlayerView playerView)
    {
        _levelManager = levelManager;
        _playerView = playerView;
    }

    public void GetUserInput()
	{
		HandleClicked();
		HandleAction();
		HandleKeyPress();
	}

    private void HandleClicked()
	{
		if(!Input.IsActionJustPressed("ui_select") || _levelManager.IsUnitUIHovered || Input.IsActionPressed("key_shift")) return;

		_levelManager.HighlightedShips.ForEach(x => x.UnitCommand.OnUnselected());
        _levelManager.HighlightedShips = new List<Unit>();
	}

	private void HandleAction()
	{
		if(!Input.IsActionJustPressed("ui_action")) return;

		Unit hoveredEnemyUnit = _levelManager.EnemyUnits.FirstOrDefault(x => x.IsHovered);

		if(hoveredEnemyUnit != null)
		{
			OnHostileShipActionPressed(hoveredEnemyUnit);
		}
		else
		{
			OnMovementCommandPressed();
		}

		// else if(_unit.IsHovered && _unit.IsPlayerSide && _unit.LevelManager.SelectedShip != null && _unit.LevelManager.SelectedShip.ShipClass == ShipClass.Repair && _unit.LevelManager.SelectedShip.TargetsInWeaponRange.Any(x => x == _unit))
		// {
		// 	OnRepairTargetActionPressed();
		// }
		
	}

	private void HandleKeyPress()
	{
		if(Input.IsActionJustPressed("ability"))
		{
			_levelManager.HighlightedShips.ForEach(x => x.IsTacticalAbilityPressed = true);
		}
	}

	// The unit is hovered, the action button is pressed, set the hostile target for selected ship
	private void OnHostileShipActionPressed(Unit hoveredEnemyUnit)
	{
		_playerView.PlaceGroupWaypoint();

		_levelManager.AudioStreamPlayer.PlayAudio(_levelManager.AudioStreamPlayer.SetTargetSoundClip);

		_levelManager.HighlightedShips.ForEach(x => {
			x.Target = hoveredEnemyUnit;
			
			if(x.ShipClass == ShipClass.Fighter)
			{
				x.TargetDesiredDistance = 0;
			}
			else
			{
				x.TargetDesiredDistance = 200;
			}
		});
	}

	// The unit is selected, the action button is pressed, do a movement command
	private void OnMovementCommandPressed()
	{
        Vector2 mousePos = _playerView.PlaceGroupWaypoint();

		_playerView.RadialFormation.GlobalPosition = mousePos;

		for(int i = 0; i < _levelManager.HighlightedShips.Count; i++)
		{
			_levelManager.HighlightedShips[i].MovementTarget = _playerView.RadialFormation.FormationPoints[i].GlobalPosition;
		}

		_levelManager.DialougeStreamPlayer.PlayUnitMoveOrderSound();
	}
}