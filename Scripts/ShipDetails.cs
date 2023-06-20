using Godot;
using System;
using System.Linq;

public partial class ShipDetails : Control
{

	public Unit Unit;
	private RichTextLabel _health;
	private RichTextLabel _class;
	private ProgressBar _healthBar;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_health = GetNode<RichTextLabel>("Health");
		_class = GetNode<RichTextLabel>("Class");

		_healthBar = GetNode<ProgressBar>("HealthBar");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		bool isHighlighted = Unit.LevelManager.HighlightedShips != null && Unit.LevelManager.HighlightedShips.Contains(Unit);
		bool shouldShowDetails = IsInstanceValid(Unit) && (Unit.IsHovered || Unit.IsSelected || isHighlighted);

		if(!shouldShowDetails)
		{
			QueueFree();
		}
		else
		{
			GlobalPosition = Unit.GlobalPosition;
			UpdateForShipDetails();
		}
	}

	public void UpdateForShipDetails()
	{
		_health.Text = $"STRUCTURE - {Unit.Health}";
		_class.Text = $"CLASS - {Unit.ClassName}";
		_healthBar.MaxValue = Unit.MaxHealth;
		_healthBar.Value = Unit.Health;
	}
}
