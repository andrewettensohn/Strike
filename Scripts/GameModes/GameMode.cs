using Godot;
using System;
using System.Threading.Tasks;

public partial class GameMode : Node
{
	[Export]
	public string ObjectiveText;

	[Export]
	public string TimerAdditionalText;

	public int PlayerScore;

	public int EnemyScore;

	public Node[] ObjectiveTargets;

	public Timer MissionTimer;

	protected LevelManager _levelManager;

	public bool IsGameOver;

	protected bool HasEnemySpawnedUnits;

	protected bool CanSpawnUnits;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_levelManager = GetTree().Root.GetNode<LevelManager>("Level");
		MissionTimer = GetNode<Timer>("MissionTimer");

		CanSpawnUnits = true;
	}

	// I wish this could be an abstract class so I don't look so stupid to the people that I imagine will read this code, but really no one is ever going to read this
	// I even did a two line comment here instead of a multi-line, I hate multi-line comments, sue me! Go ahead!
	// Anyway, I can't do an abstract class because I need the process and ready functions that are inherited from node
	// Maybe I can do abstract partial but that just feels odd and the thing is I use all these keywords to make myself feel better.
	// No one else is going to ever contribute to this code base and the computer sure as hell doesn't care whether this is abstract or not!
	// Don't quote me on that though, but I'm pretty sure this class having the abstract keyword would do nothing to performance.
	// One thing I should note is that clean code is important for yourself too. I've stepped away from this project a couple times and I can pick it back up kind easy
	// Cause everything isn't too poorly written. I wonder if I'll ever be a real game dev.

	public virtual void OnObjectiveComplete(object objectiveObj = null)
	{
		//Implement in override
	}

	public virtual async Task OnWin()
    {
		//Implement in override
    }

    public virtual async Task OnLose()
    {
		//Implement in override
    }

	public virtual void OnMissionClockExpired()
    {
		//Implement in override
    }

	public virtual void OnEnemyShipDestroyed(Unit unit)
	{
		//Implement in override
	}

	public virtual void OnPlayerShipDestroyed(Unit unit)
	{
		//Implement in override
	}
}
