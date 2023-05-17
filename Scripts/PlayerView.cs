using Godot;
using System;
using System.Collections.Generic;

public partial class PlayerView : Node
{
    private Camera2D _cam;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        _cam = GetNode<Camera2D>("Camera2D");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        HandleCameraMovement();
		HandleCameraZoom();
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
}
