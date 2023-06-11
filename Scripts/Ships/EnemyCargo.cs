using Godot;
using System;
using System.Threading.Tasks;

public partial class EnemyCargo : Unit
{

    public override void _Ready()
    {

        BaseReady();
    }

    protected override void CheckForTarget()
    {
		MovementTarget = GetTree().Root.GetNode<LevelManager>("Level").GetNode<Sprite2D>("CargoDest").GlobalPosition;
    }

    protected override async Task HandleCombat()
    {

    }

    protected override async Task HandleTactical()
    {

    }


}
