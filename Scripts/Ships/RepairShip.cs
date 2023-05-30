using Godot;
using System;
using System.Threading.Tasks;
using System.Linq;

public partial class RepairShip : Unit
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
        if(_isCombatOnCoolDown || Target == null || !TargetsInWeaponRange.Any(x => x == Target)) return;

		//repair module
        _repairModule.RepairShip(Target);

        await base.HandleCombat();
    }
}
