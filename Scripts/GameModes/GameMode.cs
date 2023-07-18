using Godot;
using System;
using System.Threading.Tasks;

public partial class GameMode : Node
{
	public int Score;

	public Node[] ObjectiveTargets;

	protected LevelManager _levelManager;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_levelManager = GetTree().Root.GetNode<LevelManager>("Level");
	}

	public virtual void OnObjectiveComplete(object objectiveObj = null)
	{

	}

	public virtual async Task OnWin()
    {
		
    }

    public virtual async Task OnLose()
    {

    }

	public virtual void OnDoomsdayClockExpired()
    {

    }
}
