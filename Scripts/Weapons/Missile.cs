using Godot;
using System;
using System.Threading.Tasks;

public partial class Missile : Area2D
{
    [Export]
    public float Speed;

    [Export]
    public int Health;

    [Export]
    public int DamageCaused;

    public Unit Target;

    private Sprite2D sprite;

    public TargetGroup MyTargetGroup;

    private bool _isLifetimeTimerActive;

    public override void _Ready()
    {
        sprite = GetNode<Sprite2D>("Sprite");
    }

    public override async void _PhysicsProcess(double delta)
    {
        if(IsInstanceValid(Target))
        {
            LookAt(Target.GlobalPosition);
            GlobalPosition = GlobalPosition.MoveToward(Target.GlobalPosition, (float)delta * Speed);
        }
        else
        {
            QueueFree();
            return;
        }

        if(!_isLifetimeTimerActive)
        {
            _isLifetimeTimerActive = true;
            await ToSignal(GetTree().CreateTimer(45.0f), "timeout");

            QueueFree();
        }
    }

    public virtual void UnitEntered(Node2D node)
    {
        if(IsInstanceValid(Target) && node.Name == Target.Name)
        {
            if(node as Unit != null)
            {
                Unit target = (Unit)node;
                target.Damage(DamageCaused);
            }

            QueueFree();
            Target = null;
        }
    }

    public virtual void BulletEntered(Area2D area)
    {
        if(area as Bullet == null) return;

        Bullet bullet = (Bullet)area;
        
        if(bullet.HostileTargetGroup != MyTargetGroup)
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
}
