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

        if(NavigationAgent.DistanceToTarget() < 10f)
        {
            UnitMovement.WarpOut(LevelManager.EnemyReinforceCorridorStart.GlobalPosition);
        }
    }

    protected override async Task HandleCombat()
    {

    }

    protected override async Task HandleTactical()
    {

    }

    protected override async Task HandleDeath()
    {
		if(_isDying)
		{
			return;
		}
		else
		{
			_isDying = true;
		}

		if(IsPlayerSide)
		{
			LevelManager.PlayerShipDestroyed(this);
			LevelManager.PlayerUnits.Remove(this);
		}
		else
		{
			LevelManager.EnemyUnits.Remove(this);
		}

		//_audioStreamPlayer.PlayAudio(_audioStreamPlayer.ShipDestroyedSoundClip);
		Sprite2D explosion = (Sprite2D)ExplosionScene.Instantiate();
		explosion.Scale = new Vector2(ExplosionScale, ExplosionScale);
		explosion.GlobalPosition = GlobalPosition;

        GetTree().Root.AddChild(explosion);

        await ToSignal(GetTree().CreateTimer(1), "timeout");

        //TODO: Win condition on LevelManager?
        //await LevelManager.OnWin();

        QueueFree();
    }

	public override void HandlePostRetreat()
	{
		LevelManager.EnemyUnits.Remove(this);

        //TODO: Lose condition on LevelManager?
        //Task.Run(async () => await LevelManager.OnLose());

		QueueFree();
	}


}
