using Godot;
using System;

public partial class ReinforceSlot : Panel
{
	[Export]
	public ShipClass ShipClass;

	private LevelManager _levelManager;

	private bool _isHovered;

	private ColorRect _hoverHighlightRect;

	private UILayer _uiLayer;

	private ToolTipInfo _toolTipInfo;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_levelManager = GetTree().Root.GetNode<LevelManager>("Level");
		_hoverHighlightRect = GetNode<ColorRect>("HoverHighlightRect");
		_uiLayer = GetParent().GetParent<UILayer>();

		_toolTipInfo = GetNode<ToolTipInfo>("ToolTipInfo");

		_hoverHighlightRect.Visible = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("ui_select") && _isHovered)
		{
			_levelManager.ReinforcePlayerShip(ShipClass);
		}
	}

	public void Hovered()
	{
		_uiLayer.StartToolTipTimer(_toolTipInfo);
		_hoverHighlightRect.Visible = true;
		_isHovered = true;
	}

	public void Unhovered()
	{
		_uiLayer.HideToolTip();
		_hoverHighlightRect.Visible = false;
		_isHovered = false;
	}
}
