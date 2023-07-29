using System;
using System.Linq;
using Godot;

public class UnitCommand
{

    private Unit _unit;

    public UnitCommand(Unit unit)
    {
        _unit = unit;
    }

    public void GetUserInput()
	{
		HandleClicked();
		HandleAction();
	}

    public void OnSelected()
	{
		if(Input.IsActionPressed("key_shift"))
		{
			OnShiftSelected();
			return;
		}

		_unit.LevelManager.PlayerView.ShowShipDetails(_unit);
		_unit.IsSelected = true;
		_unit.LevelManager.SelectedShip = _unit;
		_unit.WeaponRangeIcon.Visible = true;

		if(!_unit.LevelManager.AreMultipleUnitsSelected)
		{
			_unit.LevelManager.DialougeStreamPlayer.PlayUnitSelectedSound();
		}
	}

	public void OnUnselected()
	{
		_unit.IsSelected = false;
		_unit.WeaponRangeIcon.Visible = false;
	}

	public void OnShiftSelected()
	{
		if(!_unit.LevelManager.HighlightedShips.Any(x => x == _unit))
		{
			_unit.LevelManager.HighlightedShips.Add(_unit);
		}
		else
		{
			_unit.LevelManager.HighlightedShips.Remove(_unit);
			OnUnselected();
		}

		if(_unit.LevelManager.SelectedShip != null && !_unit.LevelManager.HighlightedShips.Any(x => x == _unit.LevelManager.SelectedShip))
		{
			_unit.LevelManager.HighlightedShips.Add(_unit.LevelManager.SelectedShip);
			_unit.LevelManager.SelectedShip = null;
		}

		_unit.LevelManager.PlayerView.ShowShipDetails(_unit);
		_unit.WeaponRangeIcon.Visible = true;
	}

	private void HandleClicked()
	{
		if(!Input.IsActionJustPressed("ui_select")) return;

		bool hasUnitSlotNotBeenSelected = !(_unit.LevelManager.SelectedUnitSlot?.Unit == _unit && _unit.LevelManager.SelectedUnitSlot.IsHovered);

		if (_unit.IsHovered && _unit.IsPlayerSide)
		{
			OnSelected();
		}
		else if (!_unit.IsHovered && _unit.IsPlayerSide && !_unit.LevelManager.IsUnitUIHovered && !Input.IsActionPressed("key_shift") && hasUnitSlotNotBeenSelected)
		{
			OnUnselected();
		}
	}

	private void HandleAction()
	{
		if(!Input.IsActionJustPressed("ui_action") || _unit.LevelManager.AreMultipleUnitsSelected) return;

		if(_unit.IsHovered && !_unit.IsPlayerSide && _unit.LevelManager.SelectedShip != null && _unit.LevelManager.SelectedShip.TargetsInWeaponRange.Any(x => x == _unit))
		{
			OnHostileShipActionPressed();
		}
		else if(_unit.IsHovered && _unit.IsPlayerSide && _unit.LevelManager.SelectedShip != null && _unit.LevelManager.SelectedShip.ShipClass == ShipClass.Repair && _unit.LevelManager.SelectedShip.TargetsInWeaponRange.Any(x => x == _unit))
		{
			OnRepairTargetActionPressed();
		}
		else if(_unit.IsSelected)
		{
			OnMovementCommandPressed();
		}
	}

	// The unit is hovered, the action button is pressed, set the hostile target for selected ship
	private void OnHostileShipActionPressed()
	{
		_unit.AudioStreamPlayer.PlayAudio(_unit.AudioStreamPlayer.SetTargetSoundClip);

		_unit.LevelManager.SelectedShip.Target = _unit;
		_unit.LevelManager.SelectedShip.MovementTarget = _unit.LevelManager.SelectedShip.GlobalPosition;

		_unit.LevelManager.SelectedShip.TargetDesiredDistance = 200;
	}

	// The unit is hovered, the action button is pressed, set the target for a selected repair ship
	private void OnRepairTargetActionPressed()
	{
		_unit.LevelManager.SelectedShip.Target = _unit;
		_unit.LevelManager.SelectedShip.MovementTarget = _unit.LevelManager.SelectedShip.GlobalPosition;
	}

	// The unit is selected, the action button is pressed, do a movement command
	private void OnMovementCommandPressed()
	{
		Vector2 mouseClickPos = _unit.GetGlobalMousePosition();
		if(_unit.Target == null || mouseClickPos.DistanceTo(_unit.Target.MovementTarget) > 200)
		{
			_unit.MovementTarget = _unit.GetGlobalMousePosition();
		}

		_unit.LevelManager.DialougeStreamPlayer.PlayUnitMoveOrderSound();
	}
}