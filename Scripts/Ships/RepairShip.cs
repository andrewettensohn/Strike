using Godot;
using System;
using System.Threading.Tasks;
using System.Linq;

public partial class RepairShip : Unit
{
    [Export]
    public new int TacticalCoolDownTime;

	private RepairModule _repairModule;
    private int _initalMaxSpeed;

    public override void _Ready()
    {
        _initalMaxSpeed = MaxSpeed;
        
        _repairModule = GetNode<RepairModule>("RepairModule");
        CombatCoolDownTime = _repairModule.CoolDownTime;

        BaseReady();
    }

    protected override async Task HandleCombat()
    {
        if(_isCombatOnCoolDown || Target == null || !TargetsInWeaponRange.Any(x => x == Target)) return;

        _repairModule.RepairShip(Target);

        await base.HandleCombat();
    }

    protected override async Task HandleTactical()
    {
        if(IsTacticalOnCoolDown || IsTacticalInUse || !IsTacticalAbilityPressed) return;

        IsTacticalInUse = true;
        IsTacticalAbilityPressed = false;

        MaxSpeed *= 5;

        TacticalDurationTimer = GetTree().CreateTimer(TacticalAbilityDuration);
        await ToSignal(TacticalDurationTimer, "timeout");

        MaxSpeed = _initalMaxSpeed;

        IsTacticalInUse = false;
        
        await base.HandleTactical();
    }
}
