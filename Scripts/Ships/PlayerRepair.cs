using Godot;
using System;
using System.Threading.Tasks;

public partial class PlayerRepair : Unit
{

	private RepairModule _repairModule;

    public override void _Ready()
    {
        _repairModule = GetNode<RepairModule>("RepairModule");
        CombatCoolDownTime = _repairModule.CoolDownTime;

        BaseReady();
    }

    protected override async Task HandleCombat()
    {
        if(_isCombatOnCoolDown || Target == null) return;

		//repair module
        _repairModule.RepairShip(Target);

        await base.HandleCombat();
    }
}
