using Godot;
using System;

public partial class UnitSlot : Control
{
	private Sprite2D _portrait;

	private RichTextLabel _health;

	private Unit _unit;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_portrait = GetNode<Sprite2D>("Portrait");
		_health = GetNode<RichTextLabel>("Health");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void UpdateSlotForUnit(Unit unit)
	{
		_unit = unit;
		_portrait.Texture = _unit.Sprite.Texture;
		//_health.Text = $"${_unit.Sprite.Structur}";
	}

	public void EmptySlot()
	{
		_unit = null;
		_portrait.Texture = null;
	}
}
