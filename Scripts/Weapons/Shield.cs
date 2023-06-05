using Godot;
using System;

public partial class Shield : Area2D
{
	[Export]
    public float CoolDownTime;

	private Sprite2D _sprite;
	private CollisionShape2D _collision; 

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_sprite = GetNode<Sprite2D>("Sprite2D");
		_collision = GetNode<CollisionShape2D>("CollisionShape2D");

		_sprite.Visible = false;
		_collision.Disabled = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void ToggleShield(bool isActive)
	{
		if(isActive)
		{
			_sprite.Visible = true;
			_collision.Disabled = false;
		}
		else
		{
			_sprite.Visible = false;
			_collision.Disabled = true;
		}
	}

	public void MissileEntered(Area2D area)
    {
        if(area as Missile == null) return;

        Missile missile = (Missile)area;
        
        // if(missile.HostileTargetGroup != MyTargetGroup)
        // {
        //     return;
        // }
        
        missile.QueueFree();
    }
}
