using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class PlayerView : Node
{
    [Export]
    public PackedScene SelectionBoxScene;

    [Export]
    public PackedScene ShipDetailsScene;

    public UILayer UILayer;

    public Formation BoxFormation;
    public Formation RadialFormation;

    private Camera2D _cam;
    private LevelManager _levelManager;
    private Sprite2D _waypoint;
    private bool _isHovered;
    private bool _isSelectionHeld;
    private GroupCommand _groupCommand;
    private PauseMenu _pauseMenu;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        UILayer = GetNode<UILayer>("CanvasLayer");
        _cam = GetNode<Camera2D>("Camera2D");
        _levelManager = GetTree().Root.GetNode<LevelManager>("Level");
        _waypoint = GetNode<Sprite2D>("WaypointSprite");
        _pauseMenu = UILayer.GetNode<PauseMenu>("PauseMenu");

        BoxFormation = GetNode<Formation>("BoxFormation");
        RadialFormation = GetNode<Formation>("RadialFormation");
        
        _groupCommand = new GroupCommand(_levelManager, this);

        _waypoint.Visible = false;
        _pauseMenu.Visible = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        HandleCursor();
        HandleGroupCommand();
        HandleSelectionBox();
        HandleCameraMovement();
		HandleCameraZoom();
        PlaceWaypoint();

        if(!_pauseMenu.Visible && Input.IsActionJustPressed("ui_cancel"))
        {
            _pauseMenu.OnPauseMenuOpened();
        }
	}

    public void ShowShipDetails(Unit unit)
    {

        ShipDetails shipDetails = (ShipDetails)ShipDetailsScene.Instantiate();
        shipDetails.Unit = unit;

        GetTree().Root.AddChild(shipDetails);
    }

    public void PlaceGroupWaypoint(Vector2 waypointPos)
    {
        //if(!_levelManager.AreMultipleUnitsSelected) return;

        _waypoint.GlobalPosition = waypointPos;
        _waypoint.Visible = true;
        GD.Print(waypointPos);
    }

    public void Hovered()
	{
		_levelManager.IsUnitUIHovered = true;
	}

	public void Unhovered()
	{
		_levelManager.IsUnitUIHovered = false;
	}

    private void HandleGroupCommand()
    {
        if(!_levelManager.AreMultipleUnitsSelected) return;

        _groupCommand.GetUserInput();
    }

    private void HandleCursor()
    {
        Resource cursorImage = _levelManager.EnemyUnits.Any(x => x.IsHovered) ? ResourceLoader.Load("res://Art/Cursor/AttackCursor.png") : ResourceLoader.Load("res://Art/Cursor/NormalCursor.png");

        Input.SetCustomMouseCursor(cursorImage);
    }

    private void HandleSelectionBox()
    {
        _isSelectionHeld = Input.IsActionPressed("ui_select");
        
        //TODO: The last condition will stop the player from picking up new units quickly?
        if(_isSelectionHeld && !_levelManager.IsSelectionBoxActive && !_levelManager.AreMultipleUnitsSelected)
        {
            SelectionBox selectionBox = (SelectionBox)SelectionBoxScene.Instantiate();
            GetTree().Root.AddChild(selectionBox);
        }
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

        if(!isZoomIn && !isZoomOut)
        {
            return;
        }

        Vector2 newZoom = _cam.Zoom;

        if(isZoomIn)
        {
            newZoom.X -= 0.1f;
            newZoom.Y -= 0.1f;
        }
        else if(isZoomOut)
        {
            newZoom.X += 0.1f;
            newZoom.Y += 0.1f;
        }

        _cam.Zoom = newZoom;
        _levelManager.IsAtFurthestZoom = _cam.Zoom.X == 0.10000002f;
    }

    private void PlaceWaypoint()
    {
        if(_levelManager.AreMultipleUnitsSelected) return;
        
        bool shouldWaypointHide = !IsInstanceValid(_levelManager.SelectedShip) ||
            _levelManager.SelectedShip == null ||
            _levelManager.SelectedShip.MovementTarget == _levelManager.SelectedShip.GlobalPosition ||
            _levelManager.SelectedShip.NavigationAgent.IsTargetReached();

        if(shouldWaypointHide)
        {
            _waypoint.Visible = false;
            return;
        }

        _waypoint.GlobalPosition = _levelManager.SelectedShip.MovementTarget;
        _waypoint.Visible = true;
    }
}
