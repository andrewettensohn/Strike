using Godot;
using System.Threading.Tasks;
using System.Linq;

public partial class CruiserShip : Unit
{

    private MissileLauncher _missileLauncher;
    private FlakTurret _flakTurret;
    private Shield _shield;

    public override void _Ready()
    {
        _missileLauncher = GetNode<MissileLauncher>("MissileLauncher");
        CombatCoolDownTime = _missileLauncher.CoolDownTime;

        _flakTurret = GetNode<FlakTurret>("FlakTurret");
        DefenseCoolDownTime = _flakTurret.CoolDownTime;

        _shield = GetNode<Shield>("Shield");
        GD.Print(_shield == null);
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
        if(_isDefenseOnCoolDown || !_missilesInRange.Any()) return;

        Missile missile = _missilesInRange.FirstOrDefault(x => IsInstanceValid(x));

        if(missile != null)
        {
            _flakTurret.FireBullet(missile, MyTargetGroup, HostileTargetGroup, GlobalPosition);
        }

        await base.HandleDefense();
    }

    protected override async Task HandleTactical()
    {
        if(_isTacticalOnCoolDown) return;

        if(IsTacticalAbilityPressed)
        {
            _isTacticalOnCoolDown = true;
            IsTacticalAbilityPressed = false;
            _shield.ToggleShield(true);

            await ToSignal(GetTree().CreateTimer(TacticalCoolDownTime), "timeout");

            _isTacticalOnCoolDown = false;
            _shield.ToggleShield(false);
        }
    }
}