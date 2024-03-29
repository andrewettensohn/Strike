using Godot;
using System.Threading.Tasks;
using System.Linq;

public partial class EnemyCapital : Unit
{
	private Laser _laser;
    private FlakTurret _flakTurret;
    private Webifier _webifier;

    public override void _Ready()
    {
        _laser = GetNode<Laser>("Laser");
        CombatCoolDownTime = _laser.CoolDownTime;

        _flakTurret = GetNode<FlakTurret>("FlakTurret");
        DefenseCoolDownTime = _flakTurret.CoolDownTime;

        _webifier = GetNode<Webifier>("Webifier");
        TacticalCoolDownTime = _webifier.CoolDownTime;

        BaseReady();
    }

    protected override async Task HandleCombat()
    {
        if(_isCombatOnCoolDown || Target == null || !TargetsInWeaponRange.Any(x => x == Target)) return;

        await _laser.FireLaser(Target, this);

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
        
        if(IsTacticalOnCoolDown || IsTacticalInUse || !IsTacticalAbilityPressed) return;

        IsTacticalInUse = true;
        IsTacticalAbilityPressed = false;

        _webifier.ToggleWeb(true, Target, this);

        TacticalDurationTimer = GetTree().CreateTimer(TacticalAbilityDuration);
        await ToSignal(TacticalDurationTimer, "timeout");

        _webifier.ToggleWeb(false, Target, this);

        IsTacticalInUse = false;
        
        await base.HandleTactical();
    }
}
