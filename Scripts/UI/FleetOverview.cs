using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class FleetOverview : Control
{
    [Export]
    public CompressedTexture2D PicketAbilityIcon;

    [Export]
    public CompressedTexture2D CuriserAbilityIcon;

    [Export]
    public CompressedTexture2D DroneControlAbilityIcon;

    [Export]
    public CompressedTexture2D RepairAbilityIcon;

    private List<UnitSlot> _unitSlots = new List<UnitSlot>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        List<Node> childNodes = GetChildren().ToList();
		foreach(Node node in childNodes)
		{
			if(node.Name.ToString().Contains("UnitSlot"))
			{
				UnitSlot unitSlot = (UnitSlot)node;
                unitSlot.FleetOverview = this;
				_unitSlots.Add(unitSlot);
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

    public void UpdateUnitSlot(Unit unit)
    {
        UnitSlot slot = _unitSlots.FirstOrDefault(x => x.Unit == unit);
        if(slot != null)
        {
            slot.UpdateSlotForUnit(unit);
        }
    }

    public void EmptyUnitSlot(Unit unit)
    {
        UnitSlot slot = _unitSlots.FirstOrDefault(x => x.Unit == unit);
        if(slot != null)
        {
            slot.EmptySlot();
        }  
    }

    public void AddUnitToOverview(Unit unit)
    {
        UnitSlot unitSlot = _unitSlots.FirstOrDefault(x => x.Unit == null);
        if(unitSlot != null)
        {
            unitSlot.UpdateSlotForUnit(unit);
        }
    }
}
