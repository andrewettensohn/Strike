using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class LevelManager : Node2D
{
    [Export]
    public PackedScene CruiserScene;

    [Export]
    public PackedScene PicketScene;

    [Export]
    public PackedScene DroneControlScene;

    [Export]
    public PackedScene RepairScene;

    [Export]
    public PackedScene EnemyCapitalScene;

    [Export]
    public PackedScene EnemyPicketScene;

    [Export]
    public PackedScene EnemyCruiserScene;

    [Export]
    public PackedScene EnemyRepairScene;

    [Export]
    public PackedScene EnemyDroneControlScene;

    [Export]
    public GameMode GameMode;

    public Unit SelectedShip;

    public List<Unit> HighlightedShips;

    public UnitSlot SelectedUnitSlot;

    public Unit HoveredEnemy;

    public List<Unit> EnemyUnits = new List<Unit>();

    public List<Unit> PlayerUnits = new List<Unit>();

    public bool IsUnitUIHovered;

    public bool IsSelectionBoxActive;

    public bool AreMultipleUnitsSelected => HighlightedShips?.Count > 0;

    public FleetOverview FleetOverview;

    public PlayerView PlayerView;

    public bool IsAtFurthestZoom;

    private bool _isFleetOverviewSetup;

    public ReinforceCorrdidorEnd PlayerReinforceCorridorEnd { get; private set; }

    public Sprite2D PlayerReinforceCorridorStart { get; private set; }

    public ReinforceCorrdidorEnd EnemyReinforceCorridorEnd { get; private set; }

    public Sprite2D EnemyReinforceCorridorStart { get; private set; }

    public EnemyCommander EnemyCommander { get; private set; }

    public StrikeAudioPlayer AudioStreamPlayer;

    public StrikeDialougePlayer DialougeStreamPlayer;

    public bool IsReinforceDisabled;

    private GameManager _gameManager;

    public override void _Ready()
    {
        _gameManager = GetNode<GameManager>("/root/GameManager");

        EnemyCommander = GetNode<EnemyCommander>("EnemyCommander");

        PlayerUnits = GetTree().GetNodesInGroup("Player").Select(x => (Unit)x).ToList();
        EnemyUnits = GetTree().GetNodesInGroup("Enemy").Select(x => (Unit)x).ToList();

        PlayerView = GetNode<PlayerView>("PlayerView");
        FleetOverview = PlayerView.GetNode("CanvasLayer").GetNode<FleetOverview>("FleetDetails");

        PlayerReinforceCorridorEnd = GetNode<ReinforceCorrdidorEnd>("PlayerReinforceCorridorEnd");
        PlayerReinforceCorridorStart = GetNode<Sprite2D>("PlayerReinforceCorridorStart");

        EnemyReinforceCorridorEnd = GetNode<ReinforceCorrdidorEnd>("EnemyReinforceCorridorEnd");
        EnemyReinforceCorridorStart = GetNode<Sprite2D>("EnemyReinforceCorridorStart");
        
        AudioStreamPlayer = GetNode<StrikeAudioPlayer>("StrikeAudioPlayer");
        DialougeStreamPlayer = GetNode<StrikeDialougePlayer>("StrikeDialougePlayer");

        SetupFleetOverviewForInitalPlayerShips();
    }

    public void PlayerShipDestroyed(Unit unit)
    {
        FleetOverview.EmptyUnitSlot(unit);
        PlayerUnits.Remove(unit);
        HighlightedShips.Remove(unit);

        DialougeStreamPlayer.PlayUnitDestroyedSound();

        if(unit.ShipClass == ShipClass.Picket)
        {
            GameMode.PlayerReinforcePoints += (int)ShipClass.Picket;
        }
        else if(unit.ShipClass == ShipClass.Crusier)
        {
            GameMode.PlayerReinforcePoints += (int)ShipClass.Crusier;
        }

        GameMode.OnPlayerShipDestroyed(unit);
    }

    public void EnemyShipDestroyed(Unit unit)
    {
        EnemyUnits.Remove(unit);
        GameMode.OnEnemyShipDestroyed(unit);
    }

    public void ReinforcePlayerShip(ShipClass shipClass)
    {
        if(IsReinforceDisabled || PlayerUnits.Count >= 8) return;

        AudioStreamPlayer.PlayAudio(AudioStreamPlayer.ReinforceSoundClip);
        DialougeStreamPlayer.PlayUnitReinforceSoundClip();

        if(shipClass == ShipClass.Picket && GameMode.PlayerReinforcePoints >= (int)ShipClass.Picket)
        {
            GameMode.PlayerReinforcePoints -= (int)ShipClass.Picket;
            SpawnPlayerShip(PicketScene);
        }
        else if(shipClass == ShipClass.Crusier && GameMode.PlayerReinforcePoints >= (int)ShipClass.Crusier)
        {
            GameMode.PlayerReinforcePoints -= (int)ShipClass.Crusier;
            SpawnPlayerShip(CruiserScene);
        }
        else if(shipClass == ShipClass.Repair && GameMode.PlayerReinforcePoints >= (int)ShipClass.Repair)
        {
            GameMode.PlayerReinforcePoints -= (int)ShipClass.Repair;
            SpawnPlayerShip(RepairScene);
        }
        else if(shipClass == ShipClass.DroneControl && GameMode.PlayerReinforcePoints >= (int)ShipClass.DroneControl)
        {
            GameMode.PlayerReinforcePoints -= (int)ShipClass.DroneControl;
            SpawnPlayerShip(DroneControlScene);
        }
    }

    public void OnSelectionBoxFinish()
    {
        IsSelectionBoxActive = false;
        
        GD.Print("finished with box");

        if(HighlightedShips.Count == 0)
        {
            GD.Print("No ships");
            return;
        }
        else if(HighlightedShips.Count == 1)
        {
            HighlightedShips.FirstOrDefault().UnitCommand.OnSelected();
            HighlightedShips = new List<Unit>();
        }
        else
        {
            HighlightedShips.ForEach(x => {
                if(IsInstanceValid(x)) //ObjectDisposedException?
                {
                    PlayerView.ShowShipDetails(x);
                    x.WeaponRangeIcon.Visible = true;
                }
            });

            SelectedShip = null;

            DialougeStreamPlayer.PlayUnitSelectedSound();
        }
    }

    public void OnSelectionBoxSpawned()
    {
        IsSelectionBoxActive = true;
		HighlightedShips = new List<Unit>();
    }

    private void SetupFleetOverviewForInitalPlayerShips()
    {
        foreach(Unit unit in PlayerUnits)
        {
            FleetOverview.AddUnitToOverview(unit);
        }
    }

    public void SpawnPlayerShip(PackedScene scene)
    {
        Unit unit = (Unit)scene.Instantiate();

        float xPos = GD.Randf() * 1000;
        float yPos = GD.Randf() * 1000;

        unit.GlobalPosition = new Vector2(PlayerReinforceCorridorStart.GlobalPosition.X + xPos, PlayerReinforceCorridorStart.GlobalPosition.Y + yPos);

        AddChild(unit);

        FleetOverview.AddUnitToOverview(unit);

        ReinforcePos pos = PlayerReinforceCorridorEnd.ReinforcePosList.FirstOrDefault(x => x.IsAvailable);
        pos.IsAvailable = false;

        unit.UnitMovement.WarpTo(pos.GlobalPosition);

        PlayerUnits.Add(unit);
    }

    public void SpawnEnemyShip(PackedScene scene)
    {
        Unit unit = (Unit)scene.Instantiate();

        float xPos = GD.Randf() * 1000;
        float yPos = GD.Randf() * 1000;

        unit.GlobalPosition = new Vector2(EnemyReinforceCorridorStart.GlobalPosition.X + xPos, EnemyReinforceCorridorStart.GlobalPosition.Y + yPos);

        AddChild(unit);

        ReinforcePos pos = EnemyReinforceCorridorEnd.ReinforcePosList.FirstOrDefault(x => x.IsAvailable);
        pos.IsAvailable = false;

        unit.UnitMovement.WarpTo(pos.GlobalPosition);

        EnemyUnits.Add(unit);
    }

    public void EndLevel()
    {
        _gameManager.GotoScene("res://Levels/PostMatch.tscn");
    }

    public Vector2 GetMousePos()
    {
        return GetGlobalMousePosition();
    }
    
}
