using Godot;
using System;
using System.Threading.Tasks;

public partial class Laser : Node
{

	[Export]
	public int LaserDuration;

	[Export]
	public int CoolDownTime;

	[Export]
	public int Damage;

	private Line2D _line;

	private Vector2 _initalTargetPos;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_line = GetNode<Line2D>("Line2D");
		_line.Visible = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	public async Task FireLaser(Unit target, Unit parent)
	{
		_line.Visible = true;

		_line.AddPoint(parent.GlobalPosition);
		_line.AddPoint(target.GlobalPosition);

		await ToSignal(GetTree().CreateTimer(LaserDuration), "timeout");

		await target.Damage(Damage);

		_line.ClearPoints();
		_line.Visible = false;
	}
}
