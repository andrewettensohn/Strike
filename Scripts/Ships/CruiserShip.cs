using Godot;
using System.Threading.Tasks;
using System.Linq;

public partial class CruiserShip : Unit
{

    private MissileLauncher _missileLauncher;
    //private FlakTurret _flakTurret;
    //private Webifier _webifier;
    private Laser _laser;

    public override void _Ready()
    {
        _missileLauncher = GetNode<MissileLauncher>("MissileLauncher");
        CombatCoolDownTime = _missileLauncher.CoolDownTime;

        // _flakTurret = GetNode<FlakTurret>("FlakTurret");
        // DefenseCoolDownTime = _flakTurret.CoolDownTime;

        // _webifier = GetNode<Webifier>("Webifier");
        // TacticalCoolDownTime = _webifier.CoolDownTime;

        _laser = GetNode<Laser>("Laser");
        TacticalCoolDownTime = _laser.CoolDownTime;

        BaseReady();
    }

    protected override async Task HandleCombat()
    {
        if(_isCombatOnCoolDown || Target == null || !TargetsInWeaponRange.Any(x => x == Target)) return;

        _missileLauncher.FireMissile(Target, MyTargetGroup, HostileTargetGroup, GlobalPosition);

        await base.HandleCombat();
    }

    // protected override async Task HandleDefense()
    // {
    //     if(_isDefenseOnCoolDown || !_missilesInRange.Any()) return;

    //     Missile missile = _missilesInRange.FirstOrDefault(x => IsInstanceValid(x));

    //     if(missile != null)
    //     {
    //         _flakTurret.FireBullet(missile, MyTargetGroup, HostileTargetGroup, GlobalPosition);
    //     }

    //     await base.HandleDefense();
    // }

    protected override async Task HandleTactical()
    {
        
        if(IsTacticalOnCoolDown || IsTacticalInUse || !IsTacticalAbilityPressed) return;

        IsTacticalInUse = true;
        IsTacticalAbilityPressed = false;

        await _laser.FireLaser(Target, this);

        // _webifier.ToggleWeb(true, Target, this);

        TacticalDurationTimer = GetTree().CreateTimer(TacticalAbilityDuration);
        await ToSignal(TacticalDurationTimer, "timeout");

        //_webifier.ToggleWeb(false, Target, this);

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