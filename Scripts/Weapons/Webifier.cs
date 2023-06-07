using Godot;
using System;

public partial class Webifier : Node
{
	[Export]
    public float CoolDownTime;

	private Sprite2D _webSprite;
	private Sprite2D _webEffect;
	private Unit _target;
	private Unit _parent;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_webSprite = GetNode<Sprite2D>("WebSprite");
		_webSprite.Visible = false;

		_webEffect = GetNode<Sprite2D>("WebEffect");
		_webEffect.Visible = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(IsInstanceValid(_target))
		{
			_webSprite.LookAt(_target.GlobalPosition);
			_webSprite.Position = _parent.GlobalPosition;

			_webEffect.GlobalPosition = _target.GlobalPosition;
		}
		else
		{
			_target = null;
		}
	}

	public void ToggleWeb(bool isActive, Unit target, Unit parent)
	{
		_parent = parent;

		if(isActive)
		{
			_target = target;
			_webSprite.Visible = true;
			_webEffect.Visible = true;
			target.MaxSpeed = 0;
		}
		else
		{
			_webSprite.Visible = false;
			_webEffect.Visible = false;
			target.MaxSpeed = target.InitalMaxSpeed;
			_target = null;
		}
	}


}
