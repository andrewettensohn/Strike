using Godot;
using System;
using System.Collections.Generic;

public partial class PlayerView : Node
{
    private Camera2D _cam;
    private LevelManager _levelManager;
    private Sprite2D _waypoint;
    private ShipDetails _shipDetails;
    private ShipDetails _enemyShipDetails;
    private bool _isHovered;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        _cam = GetNode<Camera2D>("Camera2D");
        _levelManager = GetTree().Root.GetNode<LevelManager>("Level");
        _waypoint = GetNode<Sprite2D>("WaypointSprite");
        _shipDetails = GetNode<ShipDetails>("ShipDetails");
        _enemyShipDetails = GetNode<ShipDetails>("EnemyShipDetails");

        _waypoint.Visible = false;
        _shipDetails.Visible = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        HandleCameraMovement();
		HandleCameraZoom();
        PlaceWaypoint();
        HandleShipDetails();
        HandleEnemyShipDetails();
	}

    private void HandleCameraMovement()
    {
        float newXPos = _cam.Position.X;
        float newYPos = _cam.Position.Y;

        if(Input.IsActionPressed("ui_left"))
        {
            newXPos = _cam.Position.X - 8f;
        }
        else if(Input.IsActionPressed("ui_right"))
        {
            newXPos = _cam.Position.X + 8f;
        }

        if(Input.IsActionPressed("ui_down"))
        {
            newYPos = _cam.Position.Y + 8f;
        }
        else if(Input.IsActionPressed("ui_up"))
        {
            newYPos = _cam.Position.Y - 8f;
        }
    
        _cam.Position = new Vector2(newXPos, newYPos);
    }

	private void HandleCameraZoom()
    {
        bool isZoomOut = Input.IsActionJustReleased("zoom_out");
        bool isZoomIn = Input.IsActionJustReleased("zoom_in");

        Vector2 newZoom = _cam.Zoom;

        if(isZoomIn && _cam.Zoom.X >= -1.0f)
        {
            newZoom.X -= 0.5f;
            newZoom.Y -= 0.5f;
        }
        else if(isZoomOut && _cam.Zoom.X <= 100f)
        {
            newZoom.X += 0.5f;
            newZoom.Y += 0.5f;
        }

        _cam.Zoom = newZoom;
    }

    private void PlaceWaypoint()
    {
        if(_levelManager.SelectedShip == null || _levelManager.SelectedShip.MovementTarget == _levelManager.SelectedShip.GlobalPosition || _levelManager.SelectedShip.NavigationAgent.IsTargetReached())
        {
            _waypoint.Visible = false;
            return;
        }

        _waypoint.GlobalPosition = _levelManager.SelectedShip.MovementTarget;
        _waypoint.Visible = true;
    }

    public void HandleShipDetails()
    {
        if(!IsInstanceValid(_levelManager.SelectedShip) || _levelManager.SelectedShip == null || !_levelManager.SelectedShip.IsSelected)
        {
            _shipDetails.Visible = false;
            return;
        }

        _shipDetails.Visible = true;
        _shipDetails.UpdateForShipDetails(_levelManager.SelectedShip);
        _shipDetails.GlobalPosition = _levelManager.SelectedShip.GlobalPosition;
    }

    public void HandleEnemyShipDetails()
    {
        if(!IsInstanceValid(_levelManager.HoveredEnemy) || _levelManager.HoveredEnemy == null)
        {
            _enemyShipDetails.Visible = false;
            return;
        }

        _enemyShipDetails.Visible = true;
        _enemyShipDetails.UpdateForShipDetails(_levelManager.HoveredEnemy);
        _enemyShipDetails.GlobalPosition = _levelManager.HoveredEnemy.GlobalPosition;
    }

    public void Hovered()
	{
		_levelManager.IsUnitUIHovered = true;
	}

	public void Unhovered()
	{
		_levelManager.IsUnitUIHovered = false;
	}
}
