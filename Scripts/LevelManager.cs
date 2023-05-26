using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class LevelManager : Node
{
    public Unit SelectedShip;

    public List<Unit> EnemyUnits = new List<Unit>();

    public List<Unit> PlayerUnits = new List<Unit>();

    public FleetOverview FleetOverview;

    private bool _isFleetOverviewSetup;

    public override void _Ready()
    {
        PlayerUnits = GetTree().GetNodesInGroup("Player").Select(x => (Unit)x).ToList();
        EnemyUnits = GetTree().GetNodesInGroup("Enemy").Select(x => (Unit)x).ToList();
        FleetOverview = GetNode("PlayerView").GetNode("CanvasLayer").GetNode<FleetOverview>("FleetDetails");

        foreach(Unit unit in PlayerUnits)
        {
            FleetOverview.AddUnitToOverview(unit);
        }
    }

    // public override void _Process(double delta)
    // {
    //     if(!_isFleetOverviewSetup)
    //     {
    //         foreach(Unit unit in PlayerUnits)
    //         {
    //             FleetOverview.AddUnitToOverview(unit);
    //         }
    //     }
    // }
}
