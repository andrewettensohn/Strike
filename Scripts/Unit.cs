using Godot;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class Unit : CharacterBody2D
{
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

	public bool IsSelected { get; private set;}

	public bool IsTacticalAbilityPressed;

	private bool _isHovered;

	private Sprite2D _weaponRangeIcon;

	private Vector2 _movementTargetPosition;

	protected float DefenseCoolDownTime;

    protected float CombatCoolDownTime;

    protected float TacticalCoolDownTime;

	protected bool _isCombatOnCoolDown;

	public bool IsTacticalOnCoolDown { get; protected set; }

	protected bool _isDefenseOnCoolDown;

	public bool IsTacticalInUse { get; protected set; }

	public SceneTreeTimer TacticalDurationTimer { get; protected set; }

	public SceneTreeTimer TacticalCoolDownTimer { get; protected set; }

	protected List<Unit> TargetsInWeaponRange { get; private set; } = new List<Unit>();

	protected List<Missile> _missilesInRange = new List<Missile>();

	private bool _isWarping;

	private Vector2 _warpTarget;

	private CollisionShape2D _collision;

	private bool _isRetreating;

	protected void BaseReady()
	{
		MaxHealth = Health;
		InitalMaxSpeed = MaxSpeed;
		
		AddToGroup(MyTargetGroup.ToString());

		Sprite = GetNode<Sprite2D>("Sprite2D");

		LevelManager = GetTree().Root.GetNode<LevelManager>("Level");
		NavigationAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");
		_weaponRangeIcon = GetNode<Sprite2D>("WeaponRangeIcon");
		_collision = GetNode<CollisionShape2D>("CollisionShape2D");

		_movementTargetPosition = GlobalTransform.Origin;

		NavigationAgent.PathDesiredDistance = PathDesiredDistance;
        NavigationAgent.TargetDesiredDistance = TargetDesiredDistance;

		_weaponRangeIcon.Visible = false;

		Callable.From(ActorSetup).CallDeferred();
	}

	public override async void _PhysicsProcess(double delta)
	{
		CheckForTarget();

		GetUserInput();

		HandleOffenseBehavior();

		Navigate();

		if(!_isWarping)
		{
			await HandleCombat();
			await HandleDefense();
			await HandleTactical();
		}
	}

	public void WeaponRangeEntered(Node2D node)
	{
		CheckUnitInRange(node, true);
	}

	public void DefenseRangeEntered(Area2D area)
	{
		CheckMissileInRange(area, true);
	} 

	public void WeaponRangeExitted(Node2D node)
	{
		CheckUnitInRange(node, false);
	}

	public void DefenseRangeExitted(Area2D area)
	{
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
	
	protected void HandleOffenseBehavior()
	{
		//TODO: Maybe have buttons to change the player units behavior, then this logic can be used.
		if(UnitBehavior != UnitBehavior.Offense || IsPlayerSide) return;

		if(IsInstanceValid(Target))
		{
			MovementTarget = Target.GlobalPosition;
			TargetDesiredDistance = 200;
			return;
		}

		Unit newTargetDestination = null;

		if(!IsPlayerSide)
		{
			newTargetDestination = LevelManager.PlayerUnits.FirstOrDefault();
		}
		else
		{
			newTargetDestination = LevelManager.EnemyUnits.FirstOrDefault();
		}

		if(newTargetDestination != null)
		{
			MovementTarget = newTargetDestination.GlobalPosition;
		}
	}

	protected void CheckForTarget()
	{
		if(IsInstanceValid(Target) && IsPlayerSide) return;

		if(IsPlayerSide)
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

	protected void GetUserInput()
	{

		if (Input.IsActionJustPressed("ui_select") && _isHovered && IsPlayerSide)
		{
			OnSelected();
		}
		else if (Input.IsActionJustPressed("ui_select") && !_isHovered && IsPlayerSide && !LevelManager.IsUnitUIHovered && !(LevelManager.SelectedUnitSlot?.Unit == this && LevelManager.SelectedUnitSlot.IsHovered)) //And UnitSlot is not hovered?
		{
			OnUnselected();
		}

		if(Input.IsActionJustPressed("ui_action") && _isHovered && !IsPlayerSide)
		{
			// The unit is hovered, the action button is pressed, set the hostile target for selected ship
			if(LevelManager.SelectedShip != null && LevelManager.SelectedShip.TargetsInWeaponRange.Any(x => x == this))
			{
				LevelManager.SelectedShip.Target = this;
				LevelManager.SelectedShip.MovementTarget = LevelManager.SelectedShip.GlobalPosition;
			}
		}
		else if(Input.IsActionJustPressed("ui_action") && _isHovered && IsPlayerSide)
		{
			// The unit is hovered, the action button is pressed, set the target for a selected repair ship
			if(LevelManager.SelectedShip != null && LevelManager.SelectedShip.ShipClass == ShipClass.Repair && LevelManager.SelectedShip.TargetsInWeaponRange.Any(x => x == this))
			{
				LevelManager.SelectedShip.Target = this;
				LevelManager.SelectedShip.MovementTarget = LevelManager.SelectedShip.GlobalPosition;
			}
		}
		else if(Input.IsActionJustPressed("ui_action") && IsSelected)
		{
			// The unit is selected, the action button is pressed, do a movement command
			Vector2 mouseClickPos = GetGlobalMousePosition();
			if(Target == null || mouseClickPos.DistanceTo(Target.MovementTarget) > 200)
			{
				MovementTarget = GetGlobalMousePosition();
			}
			
		}
	}

	public void OnSelected()
	{
		IsSelected = true;
		LevelManager.SelectedShip = this;
		_weaponRangeIcon.Visible = true;	
	}

	public void OnUnselected()
	{
		IsSelected = false;
		_weaponRangeIcon.Visible = false;
	}

	public void WarpTo(Vector2 location)
	{
		_isWarping = true;
		_warpTarget = location;
		MovementTarget = location;
		_movementTargetPosition = location;
	}

	public void WarpOut(Vector2 location)
	{
		TurningAngleThreshold = 0.99f;
		SpeedWhileTurning = 1;
		_isWarping = true;
		_warpTarget = location;
		MovementTarget = location;

		_isRetreating = true;
	}

	protected void Navigate()
	{
		float distanceToTarget = MovementTarget.DistanceTo(GlobalPosition);

		if (NavigationAgent.IsNavigationFinished() || distanceToTarget <= TargetDesiredDistance)
        {
			if(_isWarping)
			{
				_isWarping = false;
				_collision.Disabled = false;
			}

			if(_isRetreating)
			{
				HandlePostRetreat();
			}

            return;
        }

		LookAtNextPathPoint();

		Vector2 currentAgentPosition = GlobalTransform.Origin;
        Vector2 nextPathPosition = NavigationAgent.GetNextPathPosition();

		Vector2 newVelocity = (nextPathPosition - currentAgentPosition).Normalized();
        newVelocity *= CurrentSpeed;

        Velocity = newVelocity;

		MoveAndSlide();
	}

	protected void Hovered()
	{
		_isHovered = true;

		if(!IsPlayerSide)
		{
			_weaponRangeIcon.Visible = true;
			LevelManager.HoveredEnemy = this;
		}
	}

	protected void Unhovered()
	{
		_isHovered = false;

		if(!IsPlayerSide)
		{
			_weaponRangeIcon.Visible = false;

			if(LevelManager.HoveredEnemy == this)
			{
				LevelManager.HoveredEnemy = null;
			}
		}
	}

	protected virtual void LookAtNextPathPoint()
    {
        if(!IsLookingAtTarget())
        {
			CurrentSpeed = SpeedWhileTurning;
        }
        else
        {
			if(_isWarping)
			{
				_collision.Disabled = true;
				CurrentSpeed = 2000;
			}
			else
			{
				CurrentSpeed = MaxSpeed;
			}
        }

		float angleTo = GlobalPosition.AngleToPoint(NavigationAgent.GetNextPathPosition());
        float currentAngle = GlobalRotation;

		GlobalRotation = Mathf.LerpAngle(currentAngle, angleTo, Weight);
    }

	public bool IsLookingAtTarget()
    {
        // Get the direction the CharacterBody is facing
        Vector2 forwardDirection = new Vector2(Mathf.Cos(Rotation), Mathf.Sin(Rotation));

        // Calculate the direction to the target vector
        Vector2 directionToTarget = (NavigationAgent.GetNextPathPosition() - GlobalPosition).Normalized();

        // Calculate the dot product between the forward direction and the direction to the target
        float dotProduct = forwardDirection.Dot(directionToTarget);

        // Check if the dot product is close to 1.0 (within a tolerance) to determine if the CharacterBody is looking at the target
        float tolerance = TurningAngleThreshold;
        return dotProduct >= tolerance;
    }

	private async void ActorSetup()
    {
        // Wait for the first physics frame so the NavigationServer can sync.
        await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);

        // Now that the navigation map is no longer empty, set the movement target.
        MovementTarget = _movementTargetPosition;
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

	protected void HandleDeath()
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
        // Sprite2D largeBoom = (Sprite)LargeExplosion.Instance();
        // largeBoom.GlobalPosition = GlobalPosition;
        // GetTree().Root.AddChild(largeBoom);

        QueueFree();
    }

	protected void HandlePostRetreat()
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
        Health -= damage;

        // Sprite2D smallBoom = (Sprite2D)SmallExplosion.Instance();
        // smallBoom.GlobalPosition = GlobalPosition;
        // GetTree().Root.AddChild(smallBoom);
        

        // GetNode<AudioStreamPlayer2D>("ImpactAudioPlayer").Stream = ImpactSounds[(int)GD.Randi() % (ImpactSounds.Count - 1)];
        // GetNode<AudioStreamPlayer2D>("ImpactAudioPlayer").Play();
        
        if(Health <= 0)
        {
            HandleDeath();
        }
    }
}
