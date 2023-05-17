using Godot;
using System.Threading.Tasks;

public partial class ProtoCruiser : Unit
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
        if(_isCombatOnCoolDown || Target == null) return;

        _missileLauncher.FireSalvo(Target, MyTargetGroup, HostileTargetGroup, GlobalPosition);

        await base.HandleCombat();
    }
}