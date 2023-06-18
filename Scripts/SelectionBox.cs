using Godot;
using System;
using System.Collections.Generic;

public partial class SelectionBox : Area2D
{
	private CollisionShape2D _collisionArea;

	private LevelManager _levelManager;

	private Vector2 _initalMousePosition;

	private Rect2 _box;

	private bool _isCollisionShapeUpdated;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_levelManager = GetTree().Root.GetNode<LevelManager>("Level");
		_collisionArea = GetNode<CollisionShape2D>("CollisionShape2D");
		
		_levelManager.OnSelectionBoxSpawned();

		_initalMousePosition = GetGlobalMousePosition();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(!Input.IsActionPressed("ui_select"))
		{
			HandlePostClick();
		}

		SetCollisionShape();

		QueueRedraw();
	}

	public override void _Draw()
	{
		Rect2 rect = new (_initalMousePosition, GetGlobalMousePosition() - _initalMousePosition);
		DrawRect(rect, Colors.Green, false);
		
		if(rect.Size.X != _box.Size.X || rect.Size.Y != _box.Size.Y)
		{
			_box = rect;
			_isCollisionShapeUpdated = false;
		}
	}

	
	// Track the position of the mouse when _isSelectionHeld becomes true
	// Track the current position of the mouse, one corner will be the origin, the other corner is the current positon of the mouse
	// Dragging modifies the size and the position
	// The square should keep track of a list of units that enter the Area2D attached to it

	// On _isSelectionHeld becoming false, add the units from the square to the list of selected units on the LevelManager

	// LevelManager keeps track of the selection box being used, when this box is used, the only inputs allowed are movement or target selection, the ability panel should swap to only show the retreat button as well
	
	public void OnUnitEntered(Node2D node)
	{
		if(node as Unit == null) return;

		Unit unit = (Unit)node;

		if(!unit.IsPlayerSide || _levelManager.HighlightedShips.Contains(unit)) return;

		_levelManager.HighlightedShips.Add(unit);
	}

	public void OnAreaEntered(Area2D area)
	{
		GD.Print($"Yo! {area.Name}");
	}
	
	private void HandlePostClick()
	{
		_levelManager.OnSelectionBoxFinish();

		QueueFree();
	}

	// If you constantly redraw the collision shape then it won't detect collisions properly, hence all this bullshit
	private void SetCollisionShape()
	{
		if(_isCollisionShapeUpdated) return;

		_isCollisionShapeUpdated = true;
		RectangleShape2D rectShape = new RectangleShape2D();
		rectShape.Size = new Vector2(Math.Abs(_box.Size.X), Math.Abs(_box.Size.Y));
		_collisionArea.Shape = rectShape;
		_collisionArea.GlobalPosition = new Vector2(_box.Position.X + _box.Size.X / 2, _box.Position.Y + _box.Size.Y / 2);
	}

}
