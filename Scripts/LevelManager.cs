using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class LevelManager : Node
{
    [Export]
    public PackedScene CruiserScene;

    [Export]
    public PackedScene PicketScene;

    public Unit SelectedShip;

    public UnitSlot SelectedUnitSlot;

    public List<Unit> EnemyUnits = new List<Unit>();

    public List<Unit> PlayerUnits = new List<Unit>();

    public FleetOverview FleetOverview;

    private bool _isFleetOverviewSetup;

    private Sprite2D _playerReinforceCorridorEnd;

    private Sprite2D _playerReinforceCorridorStart;

    public override void _Ready()
    {
        PlayerUnits = GetTree().GetNodesInGroup("Player").Select(x => (Unit)x).ToList();
        EnemyUnits = GetTree().GetNodesInGroup("Enemy").Select(x => (Unit)x).ToList();
        FleetOverview = GetNode("PlayerView").GetNode("CanvasLayer").GetNode<FleetOverview>("FleetDetails");
        _playerReinforceCorridorEnd = GetNode<Sprite2D>("PlayerReinforceCorridorEnd");
        _playerReinforceCorridorStart = GetNode<Sprite2D>("PlayerReinforceCorridorStart");

        foreach(Unit unit in PlayerUnits)
        {
            FleetOverview.AddUnitToOverview(unit);
        }
    }

    public void PlayerShipDestroyed(Unit unit)
    {
        FleetOverview.EmptyUnitSlot(unit);
    }

    public void ReinforceShip(ShipClass shipClass)
    {
        if(shipClass == ShipClass.Picket)
        {
            SpawnShip(PicketScene);
        }
        else if(shipClass == ShipClass.Crusier)
        {
            SpawnShip(CruiserScene);
        }
    }

    private void SpawnShip(PackedScene scene)
    {
        Unit unit = (Unit)scene.Instantiate();

        float xPos = GD.Randf() * 500;
        float yPos = GD.Randf() * 500;

        unit.GlobalPosition = new Vector2(_playerReinforceCorridorStart.GlobalPosition.X + xPos, _playerReinforceCorridorStart.GlobalPosition.Y + yPos);

        GetTree().Root.AddChild(unit);

        FleetOverview.AddUnitToOverview(unit);

        unit.WarpTo(new Vector2(_playerReinforceCorridorEnd.GlobalPosition.X + xPos, _playerReinforceCorridorEnd.GlobalPosition.Y + yPos));
    }
    
}
