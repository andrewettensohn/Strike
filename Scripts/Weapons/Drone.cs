using Godot;
using System;
using System.Threading.Tasks;

public partial class Drone : Area2D
{
	[Export]
    public float Speed;

    [Export]
    public int Health;

    [Export]
    public int DamageCaused;

	[Export]
	public double CombatCoolDownTime;

    public Unit Parent;

	private bool _isAttachedToTarget;

	private bool _isCombatOnCoolDown;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

 	public override async void _PhysicsProcess(double delta)
    {
		if(!IsInstanceValid(Parent))
		{
			QueueFree();
			return;
		}

        if(IsInstanceValid(Parent.Target))
        {
            LookAt(Parent.Target.GlobalPosition);
            GlobalPosition = GlobalPosition.MoveToward(Parent.Target.GlobalPosition, (float)delta * Speed);
        }
		else
		{
			//Drone might start doing damage without being attached
			QueueFree();
		}

		if(_isAttachedToTarget && !_isCombatOnCoolDown)
		{
			await HandleCombat();
		}
    }

	public void UnitEntered(Node2D node)
    {
        if(IsInstanceValid(Parent.Target) && node.Name == Parent.Target.Name)
        {
			_isAttachedToTarget = true;
        }
    }

	public void UnitExitted(Node2D node)
    {
        if(IsInstanceValid(Parent.Target) && node.Name == Parent.Target.Name)
        {
			_isAttachedToTarget = false;
        }
    }

    public void BulletEntered(Area2D area)
    {
        if(area as Bullet == null) return;

        Bullet bullet = (Bullet)area;
        
        if(bullet.HostileTargetGroup != Parent?.MyTargetGroup)
        {
            return;
        }
        
        bullet.QueueFree();

        Health -= 1;

        if(Health <= 0)
        {
            QueueFree();
        }
    }

	private async Task HandleCombat()
    {
        _isCombatOnCoolDown = true;

		await ToSignal(GetTree().CreateTimer(1), "timeout");

		await Parent.Target.Damage(DamageCaused);

        await ToSignal(GetTree().CreateTimer(CombatCoolDownTime), "timeout");

        _isCombatOnCoolDown = false;
    }
}
