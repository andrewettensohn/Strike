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

    [Export]
    public PackedScene DestroyerScene;

    [Export]
    public PackedScene RepairScene;

    [Export]
    public int PlayerReinforcePoints;

    [Export]
    public int EnemyReinforcePoints;

    public Unit SelectedShip;

    public UnitSlot SelectedUnitSlot;

    public Unit HoveredEnemy;

    public List<Unit> EnemyUnits = new List<Unit>();

    public List<Unit> PlayerUnits = new List<Unit>();

    public bool IsUnitUIHovered;

    public FleetOverview FleetOverview;

    private bool _isFleetOverviewSetup;

    public Sprite2D PlayerReinforceCorridorEnd { get; private set; }

    public Sprite2D PlayerReinforceCorridorStart { get; private set; }

    public EnemyCommander EnemyCommander { get; private set; }

    public override void _Ready()
    {
        EnemyCommander = GetNode<EnemyCommander>("EnemyCommander");
        PlayerUnits = GetTree().GetNodesInGroup("Player").Select(x => (Unit)x).ToList();
        EnemyUnits = GetTree().GetNodesInGroup("Enemy").Select(x => (Unit)x).ToList();
        FleetOverview = GetNode("PlayerView").GetNode("CanvasLayer").GetNode<FleetOverview>("FleetDetails");
        PlayerReinforceCorridorEnd = GetNode<Sprite2D>("PlayerReinforceCorridorEnd");
        PlayerReinforceCorridorStart = GetNode<Sprite2D>("PlayerReinforceCorridorStart");

        foreach(Unit unit in PlayerUnits)
        {
            FleetOverview.AddUnitToOverview(unit);
        }
    }

    public void PlayerShipDestroyed(Unit unit)
    {
        FleetOverview.EmptyUnitSlot(unit);
        PlayerUnits.Remove(unit);

        if(unit.ShipClass == ShipClass.Picket)
        {
            PlayerReinforcePoints += (int)ShipClass.Picket;
        }
        else if(unit.ShipClass == ShipClass.Crusier)
        {
            PlayerReinforcePoints += (int)ShipClass.Crusier;
        }
    }

    public void ReinforceShip(ShipClass shipClass)
    {
        if(shipClass == ShipClass.Picket && PlayerReinforcePoints >= (int)ShipClass.Picket)
        {
            PlayerReinforcePoints -= (int)ShipClass.Picket;
            SpawnShip(PicketScene);
        }
        else if(shipClass == ShipClass.Crusier && PlayerReinforcePoints >= (int)ShipClass.Crusier)
        {
            PlayerReinforcePoints -= (int)ShipClass.Crusier;
            SpawnShip(CruiserScene);
        }
        else if(shipClass == ShipClass.Destroyer && PlayerReinforcePoints >= (int)ShipClass.Destroyer)
        {
            PlayerReinforcePoints -= (int)ShipClass.Destroyer;
            SpawnShip(DestroyerScene);
        }
        else if(shipClass == ShipClass.Repair && PlayerReinforcePoints >= (int)ShipClass.Repair)
        {
            PlayerReinforcePoints -= (int)ShipClass.Repair;
            SpawnShip(RepairScene);
        }
    }

    private void SpawnShip(PackedScene scene)
    {
        Unit unit = (Unit)scene.Instantiate();

        float xPos = GD.Randf() * 500;
        float yPos = GD.Randf() * 500;

        unit.GlobalPosition = new Vector2(PlayerReinforceCorridorStart.GlobalPosition.X + xPos, PlayerReinforceCorridorStart.GlobalPosition.Y + yPos);

        GetTree().Root.AddChild(unit);

        FleetOverview.AddUnitToOverview(unit);

        unit.WarpTo(new Vector2(PlayerReinforceCorridorEnd.GlobalPosition.X + xPos, PlayerReinforceCorridorEnd.GlobalPosition.Y + yPos));

        PlayerUnits.Add(unit);
    }
    
}
