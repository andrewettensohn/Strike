using Godot;
using System;
using System.Threading.Tasks;
using System.Linq;

public partial class PlayerDroneControl : Unit
{
	private MissileLauncher _missileLauncher;
    private DroneLauncher _droneLauncher;

    public override void _Ready()
    {
        _missileLauncher = GetNode<MissileLauncher>("MissileLauncher");
        CombatCoolDownTime = _missileLauncher.CoolDownTime;

        _droneLauncher = GetNode<DroneLauncher>("DroneLauncher");
        TacticalCoolDownTime = _droneLauncher.CoolDownTime;

        BaseReady();
    }

    protected override async Task HandleCombat()
    {
        if(_isCombatOnCoolDown || Target == null || !TargetsInWeaponRange.Any(x => x == Target)) return;

        _missileLauncher.FireMissile(Target, MyTargetGroup, HostileTargetGroup, GlobalPosition);

        await base.HandleCombat();
    }

    protected override async Task HandleTactical()
    {
        
        if(IsTacticalOnCoolDown || IsTacticalInUse || !IsTacticalAbilityPressed) return;

        IsTacticalInUse = true;
        IsTacticalAbilityPressed = false;

        _droneLauncher.LaunchDrone(this, this.GlobalPosition);

        TacticalDurationTimer = GetTree().CreateTimer(TacticalAbilityDuration);
        await ToSignal(TacticalDurationTimer, "timeout");

        IsTacticalInUse = false;
        
        await base.HandleTactical();
    }

    protected override void HandleEnemySpecialAbility()
    {
        //TODO: AI is able to use this outside of weapon range?
        if(IsInstanceValid(Target) && TargetsInWeaponRange.Contains(Target))
        {
            IsTacticalAbilityPressed = true;
        }
    }
}
