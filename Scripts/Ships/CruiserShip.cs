using Godot;
using System.Threading.Tasks;
using System.Linq;

public partial class CruiserShip : Unit
{

    private MissileLauncher _missileLauncher;
    private Laser _laser;

    public override void _Ready()
    {
        _missileLauncher = GetNode<MissileLauncher>("MissileLauncher");
        CombatCoolDownTime = _missileLauncher.CoolDownTime;

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

    protected override async Task HandleTactical()
    {
        
        if(IsTacticalOnCoolDown || IsTacticalInUse || !IsTacticalAbilityPressed || !IsInstanceValid(Target)) return;

        IsTacticalInUse = true;
        IsTacticalAbilityPressed = false;

        await _laser.FireLaser(Target, this);

        TacticalDurationTimer = GetTree().CreateTimer(TacticalAbilityDuration);
        await ToSignal(TacticalDurationTimer, "timeout");

        IsTacticalInUse = false;
        
        await base.HandleTactical();
    }

    protected override void HandleEnemySpecialAbility()
    {
        //TODO: AI is able to use this outside of weapon range?
        double chanceToUse = GD.RandRange(0, 1);
        if(IsInstanceValid(Target) && TargetsInWeaponRange.Contains(Target) && chanceToUse == 1)
        {
            IsTacticalAbilityPressed = true;
        }
        else
        {
            IsTacticalAbilityPressed = false;
        }
    }
}