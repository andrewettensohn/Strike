using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
    public PackedScene EnemyCapitalScene;

    [Export]
    public int PlayerReinforcePoints;

    [Export]
    public int EnemyReinforcePoints;

    [Export]
    public int DoomsdayTime;

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

    public PlayerReinforceCorrdidorEnd PlayerReinforceCorridorEnd { get; private set; }

    public Sprite2D PlayerReinforceCorridorStart { get; private set; }

    public Sprite2D EnemyReinforceCorridorEnd { get; private set; }

    public Sprite2D EnemyReinforceCorridorStart { get; private set; }

    public EnemyCommander EnemyCommander { get; private set; }

    public StrikeAudioPlayer AudioStreamPlayer;

    private bool _isReinforceDisabled;

    public override void _Ready()
    {
        EnemyCommander = GetNode<EnemyCommander>("EnemyCommander");

        PlayerUnits = GetTree().GetNodesInGroup("Player").Select(x => (Unit)x).ToList();
        EnemyUnits = GetTree().GetNodesInGroup("Enemy").Select(x => (Unit)x).ToList();

        PlayerView = GetNode<PlayerView>("PlayerView");
        FleetOverview = PlayerView.GetNode("CanvasLayer").GetNode<FleetOverview>("FleetDetails");

        PlayerReinforceCorridorEnd = GetNode<PlayerReinforceCorrdidorEnd>("PlayerReinforceCorridorEnd");
        PlayerReinforceCorridorStart = GetNode<Sprite2D>("PlayerReinforceCorridorStart");

        EnemyReinforceCorridorEnd = GetNode<Sprite2D>("EnemyReinforceCorridorEnd");
        EnemyReinforceCorridorStart = GetNode<Sprite2D>("EnemyReinforceCorridorStart");
        
        AudioStreamPlayer = GetNode<StrikeAudioPlayer>("StrikeAudioPlayer");

        SetupFleetOverviewForInitalPlayerShips();
    }

    public async Task OnWin()
    {
        await PlayerView.UILayer.DisplayMessage("Mission Status: SUCCESS");
    }

    public async Task OnLose()
    {
        await PlayerView.UILayer.DisplayMessage("Mission Status: FAILURE");
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

    public void OnDoomsdayClockExpired()
    {
        SpawnEnemyShip(EnemyCapitalScene);
        _isReinforceDisabled = true;
    }

    public void ReinforcePlayerShip(ShipClass shipClass)
    {
        if(_isReinforceDisabled) return;

        AudioStreamPlayer.PlayAudio(AudioStreamPlayer.ReinforceSoundClip);

        if(shipClass == ShipClass.Picket && PlayerReinforcePoints >= (int)ShipClass.Picket)
        {
            PlayerReinforcePoints -= (int)ShipClass.Picket;
            SpawnPlayerShip(PicketScene);
        }
        else if(shipClass == ShipClass.Crusier && PlayerReinforcePoints >= (int)ShipClass.Crusier)
        {
            PlayerReinforcePoints -= (int)ShipClass.Crusier;
            SpawnPlayerShip(CruiserScene);
        }
        else if(shipClass == ShipClass.Destroyer && PlayerReinforcePoints >= (int)ShipClass.Destroyer)
        {
            PlayerReinforcePoints -= (int)ShipClass.Destroyer;
            SpawnPlayerShip(DestroyerScene);
        }
        else if(shipClass == ShipClass.Repair && PlayerReinforcePoints >= (int)ShipClass.Repair)
        {
            PlayerReinforcePoints -= (int)ShipClass.Repair;
            SpawnPlayerShip(RepairScene);
        }
        else if(shipClass == ShipClass.Fighter && PlayerReinforcePoints >= (int)ShipClass.Fighter)
        {
            //TODO: Change this to fighter scene, hacking with destroyer scene slot
            PlayerReinforcePoints -= (int)ShipClass.Fighter;
            SpawnPlayerShip(DestroyerScene);
        }
    }

    public void OnSelectionBoxFinish()
    {
        IsSelectionBoxActive = false;

        if(HighlightedShips.Count == 0)
        {
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
                PlayerView.ShowShipDetails(x);
                x.WeaponRangeIcon.Visible = true;
            });
            SelectedShip = null;
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

    private void SpawnPlayerShip(PackedScene scene)
    {
        Unit unit = (Unit)scene.Instantiate();

        float xPos = GD.Randf() * 1000;
        float yPos = GD.Randf() * 1000;

        unit.GlobalPosition = new Vector2(PlayerReinforceCorridorStart.GlobalPosition.X + xPos, PlayerReinforceCorridorStart.GlobalPosition.Y + yPos);

        GetTree().Root.AddChild(unit);

        FleetOverview.AddUnitToOverview(unit);

        ReinforcePos pos = PlayerReinforceCorridorEnd.ReinforcePosList.FirstOrDefault(x => x.IsAvailable);
        pos.IsAvailable = false;

        unit.UnitMovement.WarpTo(pos.GlobalPosition);

        PlayerUnits.Add(unit);
    }

    private void SpawnEnemyShip(PackedScene scene)
    {
        Unit unit = (Unit)scene.Instantiate();

        float xPos = GD.Randf() * 1000;
        float yPos = GD.Randf() * 1000;

        unit.GlobalPosition = new Vector2(EnemyReinforceCorridorStart.GlobalPosition.X + xPos, EnemyReinforceCorridorStart.GlobalPosition.Y + yPos);

        GetTree().Root.AddChild(unit);

        unit.UnitMovement.WarpTo(new Vector2(EnemyReinforceCorridorEnd.GlobalPosition.X + xPos, EnemyReinforceCorridorEnd.GlobalPosition.Y + yPos));

        EnemyUnits.Add(unit);
    }
    
}
