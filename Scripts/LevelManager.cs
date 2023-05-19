using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class LevelManager : Node
{
    public Unit SelectedShip;

    public List<Unit> EnemyUnits = new List<Unit>();

    public List<Unit> PlayerUnits = new List<Unit>();

    public override void _Ready()
    {
        PlayerUnits = GetTree().GetNodesInGroup("PlayerUnits").Select(x => (Unit)x).ToList();
        EnemyUnits = GetTree().GetNodesInGroup("EnemyUnits").Select(x => (Unit)x).ToList();
    }
}
