using Godot;
using System;
using System.Threading.Tasks;

public partial class Rocket : Area2D
{
    [Export]
    public float Speed;

    [Export]
    public int Health;

    public Vector2 TargetLocation;

    private Sprite2D sprite;

    public TargetGroup MyTargetGroup;

    public override void _Ready()
    {
        sprite = GetNode<Sprite2D>("Sprite");
    }

    public override void _PhysicsProcess(double delta)
    {
        LookAt(TargetLocation);
		GlobalPosition = GlobalPosition.MoveToward(TargetLocation, (float)delta * Speed);

		if(GlobalPosition.IsEqualApprox(TargetLocation))
		{
			QueueFree();
		}
    }

    public virtual void UnitEntered(Node2D node)
    {
		if(node as Unit == null) return;
		
		Unit target = (Unit)node;

		// No friendly fire
		if(target.MyTargetGroup == MyTargetGroup) return;

		target.Damage(1);
		QueueFree();
		
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
