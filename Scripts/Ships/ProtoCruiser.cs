using Godot;
using System.Threading.Tasks;
using System.Linq;

public partial class ProtoCruiser : Unit
{
    private MissileLauncher _missileLauncher;
    private FlakTurret _flakTurret;

    public override void _Ready()
    {
        _missileLauncher = GetNode<MissileLauncher>("MissileLauncher");
        CombatCoolDownTime = _missileLauncher.CoolDownTime;

        _flakTurret = GetNode<FlakTurret>("FlakTurret");
        DefenseCoolDownTime = _flakTurret.CoolDownTime;

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
        if(_isDefenseOnCoolDown || !_missilesInRange.Any()) return;

        Missile missile = _missilesInRange.FirstOrDefault(x => IsInstanceValid(x));

        if(missile != null)
        {
            _flakTurret.FireBullet(missile, MyTargetGroup, HostileTargetGroup, GlobalPosition);
        }

        await base.HandleDefense();
    }
}