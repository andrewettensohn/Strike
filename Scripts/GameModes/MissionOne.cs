using Godot;
using System;
using System.Threading.Tasks;

public partial class MissionOne : Skirmish
{

	[Export]
	public EnemyCargo CargoShip;

	public bool IsGameOver;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override async void _Process(double delta)
	{
		if(IsGameOver) return;
		
		if(!IsInstanceValid(CargoShip))
		{
			await OnWin();
		}
		else if(CargoShip.IsRetreating)
		{
			await OnLose();
		}
	}

	public override void OnObjectiveComplete(object objectiveObj = null)
    {
		
    }

	public override async Task OnWin()
    {
		IsGameOver = true;
        await _levelManager.PlayerView.UILayer.DisplayMessage("Mission Status: SUCCESS");
    }

    public override async Task OnLose()
    {
		IsGameOver = true;
        await _levelManager.PlayerView.UILayer.DisplayMessage("Mission Status: FAILURE");
    }

	public override void OnDoomsdayClockExpired()
    {
        _levelManager.SpawnEnemyShip(_levelManager.EnemyCapitalScene);
        _levelManager.IsReinforceDisabled = true;
    }
}
