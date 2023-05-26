using Godot;
using System;

public partial class UnitSlot : Control
{
	private Sprite2D _portrait;

	private RichTextLabel _health;

	public Unit Unit { get; private set; }

    private bool _isHovered;

    private bool _isSelected;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_portrait = GetNode<Sprite2D>("Portrait");
		_health = GetNode<RichTextLabel>("Health");

        Visible = false;
	}

    // Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        //Check if ship selected already?
        GetUserInput();
	}

	protected void GetUserInput()
	{

		if (Input.IsActionJustPressed("ui_select") && _isHovered)
		{
			_isSelected = true;
            Unit.OnSelected();
		}
		else if (Input.IsActionJustPressed("ui_select") && !_isHovered)
		{
			_isSelected = false;
		}
	}

	public void UpdateSlotForUnit(Unit unit)
	{
		Unit = unit;
		_portrait.Texture = Unit.Sprite.Texture;
        _portrait.Rotation = Unit.Sprite.Rotation;
        Visible = true;
		//_health.Text = $"${_unit.Sprite.Structur}";
	}

	public void EmptySlot()
	{
		Unit = null;
		_portrait.Texture = null;
        Visible = false;
	}

    public void Hovered()
	{
		_isHovered = true;
	}

	public void Unhovered()
	{
		_isHovered = false;
	}
}
