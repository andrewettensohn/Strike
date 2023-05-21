using Godot;
using System;
using System.Threading.Tasks;
using System.Linq;

public partial class PlayerPicket : Unit
{
    private MissileLauncher _missileLauncher;
    private FlakTurret _flakTurretOne;
	private FlakTurret _flakTurretTwo;

    public override void _Ready()
    {
        _missileLauncher = GetNode<MissileLauncher>("MissileLauncher");
        CombatCoolDownTime = _missileLauncher.CoolDownTime;

        _flakTurretOne = GetNode<FlakTurret>("FlakTurret");
		_flakTurretTwo = GetNode<FlakTurret>("FlakTurret2");
        DefenseCoolDownTime = _flakTurretOne.CoolDownTime;

        BaseReady();
    }

    protected override async Task HandleCombat()
    {
        if(_isCombatOnCoolDown || Target == null) return;

        _missileLauncher.FireMissile(Target, MyTargetGroup, HostileTargetGroup, GlobalPosition);

        await base.HandleCombat();
    }


    protected override async Task HandleDefense()
    {
        if(_isDefenseOnCoolDown || !_missilesInRange.Any()) return;

        Missile missile = _missilesInRange.FirstOrDefault(x => IsInstanceValid(x));

        if(missile != null)
        {
            _flakTurretOne.FireBullet(missile, MyTargetGroup, HostileTargetGroup, GlobalPosition);
			_flakTurretTwo.FireBullet(missile, MyTargetGroup, HostileTargetGroup, GlobalPosition);
        }

        await base.HandleDefense();
    }
}
