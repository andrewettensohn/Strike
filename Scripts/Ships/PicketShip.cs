using Godot;
using System;
using System.Threading.Tasks;
using System.Linq;

public partial class PicketShip : Unit
{
    private MissileLauncher _missileLauncher;
    private FlakTurret _flakTurretOne;
	private FlakTurret _flakTurretTwo;
    private Shield _shield;

    public override void _Ready()
    {
        _missileLauncher = GetNode<MissileLauncher>("MissileLauncher");
        CombatCoolDownTime = _missileLauncher.CoolDownTime;

        _flakTurretOne = GetNode<FlakTurret>("FlakTurret");
		_flakTurretTwo = GetNode<FlakTurret>("FlakTurret2");
        DefenseCoolDownTime = _flakTurretOne.CoolDownTime;
     
        _shield = GetNode<Shield>("Shield");
        TacticalCoolDownTime = _shield.CoolDownTime;

        BaseReady();
    }

    protected override async Task HandleCombat()
    {
        if(_isCombatOnCoolDown || Target == null || !TargetsInWeaponRange.Any(x => x == Target)) return;

        _missileLauncher.FireMissile(Target, MyTargetGroup, HostileTargetGroup, GlobalPosition);

        await base.HandleCombat();
    }


    protected override async Task HandleDefense()
    {
        if(_isDefenseOnCoolDown || (!_missilesInRange.Any() && !_dronesInRange.Any())) return;

        Drone drone = _dronesInRange.FirstOrDefault(x => IsInstanceValid(x));
        Missile missile = _missilesInRange.FirstOrDefault(x => IsInstanceValid(x));

        if(drone != null)
        {
            _flakTurretOne.FireBullet(drone, MyTargetGroup, HostileTargetGroup, GlobalPosition);
            _flakTurretTwo.FireBullet(drone, MyTargetGroup, HostileTargetGroup, GlobalPosition);
        }
        else if(missile != null)
        {
            _flakTurretOne.FireBullet(missile, MyTargetGroup, HostileTargetGroup, GlobalPosition);
			_flakTurretTwo.FireBullet(missile, MyTargetGroup, HostileTargetGroup, GlobalPosition);
        }

        await base.HandleDefense();
    }

    protected override async Task HandleTactical()
    {
        
        if(IsTacticalOnCoolDown || IsTacticalInUse || !IsTacticalAbilityPressed) return;

        IsTacticalInUse = true;
        IsTacticalAbilityPressed = false;

        _shield.ToggleShield(true, MyTargetGroup);

        TacticalDurationTimer = GetTree().CreateTimer(TacticalAbilityDuration);
        await ToSignal(TacticalDurationTimer, "timeout");

        _shield.ToggleShield(false, MyTargetGroup);

        IsTacticalInUse = false;
        
        await base.HandleTactical();
    }

    protected override void HandleEnemySpecialAbility()
    {
        if(_missilesInRange.Any() && Health < MaxHealth)
        {
            IsTacticalAbilityPressed = true;
        }
    }
}
