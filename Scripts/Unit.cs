using Godot;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class Unit : CharacterBody2D
{
    [Export]
    public int Health;

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

	public int CurrentSpeed;

	public Unit Target;

	public Vector2 MovementTarget
    {
        get { return _navigationAgent.TargetPosition; }
        set { _navigationAgent.TargetPosition = value; }
    }

	private NavigationAgent2D _navigationAgent;

	private LevelManager _levelManager;

	private bool _isSelected;

	private bool _isHovered;

	private Vector2 _movementTargetPosition;

	protected float DefenseCoolDownTime;

    protected float CombatCoolDownTime;

    protected float TacticalCoolDownTime;

	protected bool _isCombatOnCoolDown;

	protected bool _isTacticalOnCoolDown;

	protected bool _isDefenseOnCoolDown;

	protected List<Unit> _targetsInWeaponRange = new List<Unit>();

	protected List<Missile> _missilesInRange = new List<Missile>();

	protected void BaseReady()
	{
		_levelManager = GetTree().Root.GetNode<LevelManager>("Level");
		_navigationAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");

		_movementTargetPosition = GlobalTransform.Origin;

		_navigationAgent.PathDesiredDistance = PathDesiredDistance;
        _navigationAgent.TargetDesiredDistance = TargetDesiredDistance;

		Callable.From(ActorSetup).CallDeferred();
	}

	public override async void _PhysicsProcess(double delta)
	{
		CheckForTarget();

		if(IsPlayerSide)
		{
			GetUserInput();
		}
		else
		{
			HandleOffenseBehavior();

			//Offensive: go after player units
			//Defensive: Should I hold my position
			//Sentry: Should I chase player units after they enter my weapon range
		}

		Navigate();

		await HandleCombat();
		await HandleDefense();

		//TODO: Handle Defense, all units have point defense.
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
			_targetsInWeaponRange.Add(target);
		}
		else
		{
			_targetsInWeaponRange.Remove(target);
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
			return;
		}

		Unit newTargetDestination = null;

		//TODO: Make distance calculation to determine best target to go after
		if(!IsPlayerSide)
		{
			newTargetDestination = _levelManager.PlayerUnits.FirstOrDefault();
		}
		else
		{
			newTargetDestination = _levelManager.EnemyUnits.FirstOrDefault();
		}

		if(newTargetDestination != null)
		{
			MovementTarget = newTargetDestination.GlobalPosition;
		}
	}

	protected void CheckForTarget()
	{
		if(!IsInstanceValid(Target))
		{
			Target = _targetsInWeaponRange.FirstOrDefault();
		}
	}

	protected void GetUserInput()
	{

		if (Input.IsActionJustPressed("ui_select") && _isHovered && IsPlayerSide)
		{
			GD.Print("Unit Selected");
			_isSelected = true;
			_levelManager.SelectedShip = this;
		}
		else if (Input.IsActionJustPressed("ui_select") && !_isHovered && IsPlayerSide)
		{
			GD.Print("Unit Unselected");
			_isSelected = false;
		}

		if(Input.IsActionJustPressed("ui_action") && _isSelected)
		{
			MovementTarget = GetGlobalMousePosition();
		}
		else if(Input.IsActionJustPressed("ui_action") && _isHovered && !IsPlayerSide)
		{
			if(_levelManager.SelectedShip != null)
			{
				_levelManager.SelectedShip.Target = this;
				_levelManager.SelectedShip.MovementTarget = GlobalPosition;
				GD.Print("Ship targeted");
			}
		}
	}

	protected void Navigate()
	{
		float distanceToTarget = MovementTarget.DistanceTo(GlobalPosition);

		if (_navigationAgent.IsNavigationFinished() || distanceToTarget <= TargetDesiredDistance)
        {
            return;
        }

		LookAtNextPathPoint();

		Vector2 currentAgentPosition = GlobalTransform.Origin;
        Vector2 nextPathPosition = _navigationAgent.GetNextPathPosition();

		Vector2 newVelocity = (nextPathPosition - currentAgentPosition).Normalized();
        newVelocity *= CurrentSpeed;

        Velocity = newVelocity;

		MoveAndSlide();
	}

	protected void Hovered()
	{
		_isHovered = true;
	}

	protected void Unhovered()
	{
		_isHovered = false;
	}

	protected virtual void LookAtNextPathPoint()
    {
        if(!IsLookingAtTarget())
        {
			CurrentSpeed = SpeedWhileTurning;
        }
        else
        {
			CurrentSpeed = MaxSpeed;
        }

		float angleTo = GlobalPosition.AngleToPoint(_navigationAgent.GetNextPathPosition());
        float currentAngle = GlobalRotation;

		GlobalRotation = Mathf.LerpAngle(currentAngle, angleTo, Weight);
    }

	public bool IsLookingAtTarget()
    {
        // Get the direction the CharacterBody is facing
        Vector2 forwardDirection = new Vector2(Mathf.Cos(Rotation), Mathf.Sin(Rotation));

        // Calculate the direction to the target vector
        Vector2 directionToTarget = (_navigationAgent.GetNextPathPosition() - GlobalPosition).Normalized();

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
        _isTacticalOnCoolDown = true;

        await ToSignal(GetTree().CreateTimer(TacticalCoolDownTime), "timeout");

        _isTacticalOnCoolDown = false;
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
			_levelManager.PlayerUnits.Remove(this);
		}
		else
		{
			_levelManager.EnemyUnits.Remove(this);
		}
        // Sprite2D largeBoom = (Sprite)LargeExplosion.Instance();
        // largeBoom.GlobalPosition = GlobalPosition;
        // GetTree().Root.AddChild(largeBoom);

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
