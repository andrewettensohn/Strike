using Godot;
using System;
using System.Threading.Tasks;
using System.Linq;

public partial class DestroyerShip : Unit
{
    private MissileLauncher _missileLauncher;

    public override void _Ready()
    {
        _missileLauncher = GetNode<MissileLauncher>("MissileLauncher");
        CombatCoolDownTime = _missileLauncher.CoolDownTime;

        BaseReady();
    }

    protected override async Task HandleCombat()
    {
        if(_isCombatOnCoolDown || Target == null || !TargetsInWeaponRange.Any(x => x == Target)) return;

        _missileLauncher.FireRocketBarrage(Target, MyTargetGroup, HostileTargetGroup, GlobalPosition);

        await base.HandleCombat();
    }
}
