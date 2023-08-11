using Godot;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class Unit : CharacterBody2D
{
	//Exports

	[Export]
	public string ClassName;

	[Export]
	public string AbilityDescription;

    [Export]
    public float Health;

    [Export]
    public int MaxSpeed;

    [Export]
    public float Weight;

	[Export]
    public int SpeedWhileTurning = 50;

	[Export]
	public bool IsPlayerSide;

	[Export]
	public TargetGroup MyTargetGroup;

	[Export]
	public TargetGroup HostileTargetGroup;

	[Export]
	public UnitBehavior UnitBehavior;

	[Export]
	public float PathDesiredDistance;

	[Export]
	public float TargetDesiredDistance;

	[Export]
	public float TurningAngleThreshold;

	[Export]
	public ShipClass ShipClass;

	[Export]
	public int TacticalAbilityDuration;

	[Export]
	public PackedScene ExplosionScene;

	[Export]
	public float ExplosionScale;

	// Publics

	public int InitalMaxSpeed { get; private set; }

	public float MaxHealth { get; private set; }

	public Sprite2D Sprite;

	public int CurrentSpeed;

	public Unit Target;

	public Vector2 MovementTarget
    {
        get { return NavigationAgent.TargetPosition; }
        set { NavigationAgent.TargetPosition = value; }
    }

	public NavigationAgent2D NavigationAgent;

	public LevelManager LevelManager { get; private set; }

	public bool IsSelected { get; set;}

	public bool IsTacticalAbilityPressed;

	public bool IsHovered;

	public Sprite2D WeaponRangeIcon;

	public Vector2 MovementTargetPosition;

	public bool IsTacticalOnCoolDown { get; protected set; }

	public bool IsTacticalInUse { get; protected set; }

	public SceneTreeTimer TacticalDurationTimer { get; protected set; }

	public SceneTreeTimer TacticalCoolDownTimer { get; protected set; }

	public List<Unit> TargetsInWeaponRange { get; set; } = new List<Unit>();

	public bool IsWarping;

	public Vector2 WarpTarget;

	public CollisionShape2D Collision;

	public bool IsRetreating;

	public StrikeAudioPlayer AudioStreamPlayer;

	public UnitCommand UnitCommand;

	public UnitMovement UnitMovement;

	public bool WillCapture;

	public Vector2 SafeVelocity;

	public ToolTipInfo ToolTipInfo;


	// Protected and Private

	protected float DefenseCoolDownTime;

    protected float CombatCoolDownTime;

    protected float TacticalCoolDownTime;

	protected bool _isCombatOnCoolDown;

	protected bool _isDefenseOnCoolDown;

	protected List<Missile> _missilesInRange = new List<Missile>();

	protected List<Drone> _dronesInRange = new List<Drone>();

	protected bool _isDying;

	private Sprite2D _unitIcon;

	private GameManager _gameManager;

	protected void BaseReady()
	{
		UnitCommand = new UnitCommand(this);
		UnitMovement = new UnitMovement(this);

		MaxHealth = Health;
		InitalMaxSpeed = MaxSpeed;
		
		AddToGroup(MyTargetGroup.ToString());

		Sprite = GetNode<Sprite2D>("Sprite2D");

		LevelManager = GetTree().Root.GetNode<LevelManager>("Level");
		NavigationAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");
		WeaponRangeIcon = GetNode<Sprite2D>("WeaponRangeIcon");
		Collision = GetNode<CollisionShape2D>("CollisionShape2D");
		AudioStreamPlayer = GetNode<StrikeAudioPlayer>("StrikeAudioPlayer");

		_gameManager = GetNode<GameManager>("/root/GameManager");

		_unitIcon = GetNode<Sprite2D>("UnitIcon");
		_unitIcon.Visible = false;

		MovementTargetPosition = GlobalTransform.Origin;

		NavigationAgent.PathDesiredDistance = PathDesiredDistance;
        NavigationAgent.TargetDesiredDistance = TargetDesiredDistance;

		WeaponRangeIcon.Visible = false;

		NavigationAgent.PathDesiredDistance = 100f;

		if(IsPlayerSide)
		{
			ToolTipInfo = GetNode<ToolTipInfo>("ToolTipInfo");
			
			if(_gameManager.MatchOptions.IsEasyMode)
			{
				Health *= 2;
			}
		}

		Callable.From(UnitMovement.ActorSetup).CallDeferred();
	}

	public override async void _PhysicsProcess(double delta)
	{
		try
		{
			UnitMovement.Navigate();

			if(IsWarping) return;

			CheckForTarget();

			UnitCommand.GetUserInput();

			HandleOffenseBehavior();

			await HandleCombat();
			await HandleDefense();
			await HandleTactical();
			
			_unitIcon.Visible = LevelManager.IsAtFurthestZoom;
		}
		catch(Exception ex)
		{
			GD.Print($"Unit Error. {ex.Message}");
		}
	}

	public void OnNavAgentVelocityComputed(Vector2 safeVelocity)
	{
		NavigationAgent.MaxSpeed = CurrentSpeed;
		Velocity = safeVelocity;
		MoveAndSlide();
	}

	public void WeaponRangeEntered(Node2D node)
	{
		CheckUnitInRange(node, true);
	}

	public void DefenseRangeEntered(Area2D area)
	{
		CheckDroneInRange(area, true);
		CheckMissileInRange(area, true);
	} 

	public void WeaponRangeExitted(Node2D node)
	{
		CheckUnitInRange(node, false);
	}

	public void DefenseRangeExitted(Area2D area)
	{
		CheckDroneInRange(area, false);
		CheckMissileInRange(area, false);
	}

	protected void CheckUnitInRange(Node2D node, bool shouldAdd)
	{
		if(node as Unit == null) return;

		Unit target = (Unit)node;
	
		if(target.MyTargetGroup != HostileTargetGroup) return;
		
		if(shouldAdd)
		{
			TargetsInWeaponRange.Add(target);

			if(IsPlayerSide)
			{
				LevelManager.DialougeStreamPlayer.PlayUnitAttackingSoundClip();
			}
		}
		else
		{
			TargetsInWeaponRange.Remove(target);
		}
	}

	protected void CheckMissileInRange(Node2D node, bool shouldAdd)
	{
		if(node as Missile == null) return;

		Missile missile = (Missile)node;
	
		if(missile.MyTargetGroup != HostileTargetGroup) return;
		
		if(shouldAdd)
		{
			_missilesInRange.Add(missile);
		}
		else
		{
			_missilesInRange.Remove(missile);
		}
	}

	protected void CheckDroneInRange(Node2D node, bool shouldAdd)
	{
		if(node as Drone == null) return;

		Drone drone = (Drone)node;
	
		if(drone.Parent.MyTargetGroup != HostileTargetGroup) return;
		
		if(shouldAdd)
		{
			_dronesInRange.Add(drone);
		}
		else
		{
			_dronesInRange.Remove(drone);
		}
	}
	
	protected void HandleOffenseBehavior()
	{
		if(UnitBehavior != UnitBehavior.Offense) return;

		//bool isTargetValid = IsInstanceValid(Target);

		//if((isTargetValid && !IsPlayerSide) || (isTargetValid && MovementTargetPosition == Target.GlobalPosition && IsPlayerSide))
		if(IsPlayerSide && IsInstanceValid(Target) && MovementTargetPosition == Target.GlobalPosition)
		{
			MovementTarget = Target.GlobalPosition;
		}

		if(!IsPlayerSide)
		{
			MovementTarget = LevelManager.EnemyCommander.GetMovementTarget(this);
		}

		if(IsInstanceValid(Target) && MovementTarget == Target.GlobalPosition && TargetsInWeaponRange.Contains(Target))
		{
			MaxSpeed = 0;
		}
		else
		{
			MaxSpeed = InitalMaxSpeed;
		}

		if(!IsPlayerSide)
		{
			HandleEnemySpecialAbility();
		}
	}

	protected virtual void HandleEnemySpecialAbility()
	{
		//Implement in override
	}

	protected virtual void CheckForTarget()
	{
		if(IsInstanceValid(Target) && IsPlayerSide) return;

		if(IsPlayerSide && ShipClass == ShipClass.Repair)
		{
			Target = LevelManager.PlayerUnits.OrderBy(x => x.Health).FirstOrDefault(x => x != this); //Let's see how this works
			return;
		}
		else if(IsPlayerSide)
		{
			Target = TargetsInWeaponRange.FirstOrDefault();
			return;
		}

		if(ShipClass == ShipClass.Repair && LevelManager.EnemyUnits?.Any() == true)
		{
			Target = LevelManager.EnemyCommander.GetHealTargetForUnit(this, LevelManager.EnemyUnits);	
		}
		else if(LevelManager.PlayerUnits?.Any() == true)
		{
			Target = LevelManager.EnemyCommander.GetTargetForUnit(this, LevelManager.PlayerUnits);
		}
	}

	protected void Hovered()
	{
		IsHovered = true;
		WeaponRangeIcon.Visible = true;
		LevelManager.PlayerView.ShowShipDetails(this);

		if(!IsPlayerSide)
		{
			LevelManager.HoveredEnemy = this;
		}
	}

	protected void Unhovered()
	{
		IsHovered = false;

		if(!IsSelected && !LevelManager.HighlightedShips.Any(x => x == this))
		{
			WeaponRangeIcon.Visible = false;
		}

		if(!IsPlayerSide)
		{
			if(LevelManager.HoveredEnemy == this)
			{
				LevelManager.HoveredEnemy = null;
			}
		}
	}

	protected virtual async Task HandleCombat()
    {
        _isCombatOnCoolDown = true;

        await ToSignal(GetTree().CreateTimer(CombatCoolDownTime), "timeout");

        _isCombatOnCoolDown = false;
    }

    protected virtual async Task HandleTactical()
    {
        IsTacticalOnCoolDown = true;

		TacticalCoolDownTimer = GetTree().CreateTimer(TacticalCoolDownTime);
        await ToSignal(TacticalCoolDownTimer, "timeout");

        IsTacticalOnCoolDown = false;
    }

    protected virtual async Task HandleDefense()
    {
        _isDefenseOnCoolDown = true;

        await ToSignal(GetTree().CreateTimer(DefenseCoolDownTime), "timeout");

        _isDefenseOnCoolDown = false;
    }

	protected virtual void HandleDeath()
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
		}
		else
		{
			LevelManager.EnemyShipDestroyed(this);
		}

		Sprite2D explosion = (Sprite2D)ExplosionScene.Instantiate();
		explosion.Scale = new Vector2(ExplosionScale, ExplosionScale);
		explosion.GlobalPosition = GlobalPosition;

        GetTree().Root.AddChild(explosion);

        QueueFree();
    }

	public virtual void HandlePostRetreat()
	{
		if(IsPlayerSide)
		{
			LevelManager.PlayerShipDestroyed(this);
			LevelManager.PlayerUnits.Remove(this);
		}
		else
		{
			LevelManager.EnemyUnits.Remove(this);
		}

		QueueFree();
	}

	public virtual void Damage(int damage)
    {
		if(!IsPlayerSide && _gameManager.MatchOptions.IsEasyMode)
		{
			damage *= 4;
		}

        Health -= damage;
        
        if(Health <= 0)
        {
            HandleDeath();
        }
		else if(IsPlayerSide)
		{
			LevelManager.DialougeStreamPlayer.PlayUnitDamagedSoundClip();
		}
    }
}
