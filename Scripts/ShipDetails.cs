using Godot;
using System;

public partial class ShipDetails : Control
{
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
	}

	public void UpdateForShipDetails(Unit unit)
	{
		_health.Text = $"STRUCTURE - {unit.Health}";
		_class.Text = $"CLASS - {unit.ClassName}";
		_healthBar.MaxValue = unit.MaxHealth;
		_healthBar.Value = unit.Health;
	}
}
