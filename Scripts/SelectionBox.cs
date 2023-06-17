using Godot;
using System;

public partial class SelectionBox : Area2D
{
	private CollisionShape2D _collisionArea;

	private Line2D _line2D;

	private LevelManager _levelManager;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_levelManager = GetTree().Root.GetNode<LevelManager>("Level");
		_collisionArea = GetNode<CollisionShape2D>("CollisionShape2D");
		_line2D = GetNode<Line2D>("Line2D");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(!Input.IsActionPressed("ui_select"))
		{

		}
	}

	
	// Track the position of the mouse when _isSelectionHeld becomes true
	// Track the current position of the mouse, one corner will be the origin, the other corner is the current positon of the mouse
	// Dragging modifies the size and the position
	// The square should keep track of a list of units that enter the Area2D attached to it

	// On _isSelectionHeld becoming false, add the units from the square to the list of selected units on the LevelManager

	// LevelManager keeps track of the selection box being used, when this box is used, the only inputs allowed are movement or target selection, the ability panel should swap to only show the retreat button as well
	

	
	private void HandlePostClick()
	{
		
	}

}
