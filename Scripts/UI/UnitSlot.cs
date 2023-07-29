using Godot;
using System;

public partial class UnitSlot : Control
{
	private Sprite2D _portrait;

	private RichTextLabel _health;

	private ProgressBar _healthBar;

	private ColorRect _hoverHighlightRect;

	public Unit Unit { get; private set; }

    public  bool IsHovered { get; private set; }

    private bool _isSelected;

	private ColorRect _abilityInUseRect;

	private ColorRect _abilityCoolDownRect;

	private Sprite2D _abilityIcon;

	private RichTextLabel _abilityCoolDownTime;

	public FleetOverview FleetOverview;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_portrait = GetNode<Sprite2D>("Portrait");
		_health = GetNode<RichTextLabel>("Health");
		_healthBar = GetNode<ProgressBar>("HealthBar");
		_hoverHighlightRect = GetNode<ColorRect>("HoverHighlightRect");
		_abilityIcon = GetNode<Sprite2D>("AbilityIcon");
		_abilityInUseRect = GetNode<ColorRect>("AbilityInUseRect");
		_abilityCoolDownRect = GetNode<ColorRect>("AbilityCoolDownRect");
		_abilityCoolDownTime = GetNode<RichTextLabel>("AbilityCoolDownTime");

        Visible = false;
	}

    // Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        GetUserInput();

		if(Unit != null)
		{
			_health.Text = $"STRUCTURE {Unit.Health}";
			_healthBar.MaxValue = Unit.MaxHealth;
			_healthBar.Value = Unit.Health;

			HandleAbilityTimerText();

			HandleAbilityIconColorRect();

			HandleHoverHighlightRect();
		}
		else
		{
			_hoverHighlightRect.Visible = false;
		}
	}

	protected void GetUserInput()
	{

		if (Input.IsActionJustPressed("ui_select") && IsHovered)
		{
			//_isSelected = true;
            Unit.LevelManager.SelectedUnitSlot = this;
            Unit.UnitCommand.OnSelected();
		}
		else if (Input.IsActionJustPressed("ui_select") && !IsHovered)
		{
			_isSelected = false;
		}
	}

	public void AbilityButtonPressed()
	{
		Unit.IsTacticalAbilityPressed = true;
		Unit.UnitCommand.OnSelected();
	}

	public void UpdateSlotForUnit(Unit unit)
	{
		Unit = unit;
		_portrait.Texture = Unit.Sprite.Texture;
        _portrait.Rotation = Unit.Sprite.Rotation;
		DisplayAbilityIconForShipClass(unit.ShipClass);
        Visible = true;
	}

	public void EmptySlot()
	{
		Unit = null;
		_portrait.Texture = null;
        Visible = false;
	}

    public void Hovered()
	{
		IsHovered = true;
	}

	public void Unhovered()
	{
		IsHovered = false;
	}

	private void HandleAbilityTimerText()
	{
		if(IsInstanceValid(Unit.TacticalCoolDownTimer) && Unit.TacticalCoolDownTimer.TimeLeft > 0)
		{
			_abilityCoolDownTime.Visible = true;
			_abilityCoolDownTime.Text = $"{Math.Round(Unit.TacticalCoolDownTimer.TimeLeft)}";
		}
		else
		{
			_abilityCoolDownTime.Visible = false;
		}
	}

	private void HandleAbilityIconColorRect()
	{
		if(Unit.IsTacticalInUse)
		{
			_abilityInUseRect.Visible = true;
			_abilityCoolDownRect.Visible = false;
		}
		else if(Unit.IsTacticalOnCoolDown)
		{
			_abilityInUseRect.Visible = false;
			_abilityCoolDownRect.Visible = true;
		}
		else
		{
			_abilityInUseRect.Visible = false;
			_abilityCoolDownRect.Visible = false;
		}
	}

	private void HandleHoverHighlightRect()
	{
		if(Unit.IsSelected || Unit.LevelManager.HighlightedShips.Contains(Unit))
		{
			_hoverHighlightRect.Visible = true;
		}
		else
		{
			_hoverHighlightRect.Visible = false;
		}
	}

	private void DisplayAbilityIconForShipClass(ShipClass shipClass)
	{
		if(shipClass == ShipClass.Picket)
		{
			_abilityIcon.Texture = FleetOverview.PicketAbilityIcon;
		}
		if(shipClass == ShipClass.Crusier)
		{
			_abilityIcon.Texture = FleetOverview.CuriserAbilityIcon;
		}
		if(shipClass == ShipClass.DroneControl)
		{
			_abilityIcon.Texture = FleetOverview.DroneControlAbilityIcon;
		}
		if(shipClass == ShipClass.Repair)
		{
			_abilityIcon.Texture = FleetOverview.RepairAbilityIcon;
		}
	}
}
