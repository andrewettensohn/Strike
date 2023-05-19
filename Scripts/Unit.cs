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
    public float TurnModifier;

	[Export]
	public bool IsPlayerSide;

	[Export]
	public TargetGroup MyTargetGroup;

	[Export]
	public TargetGroup HostileTargetGroup;

	[Export]
	public UnitBehavior UnitBehavior;

	public int CurrentSpeed;

	public Unit Target;

	public Vector2 Destination;

	public List<Vector2> Path = new List<Vector2>();

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

	protected void BaseReady()
	{
		_levelManager = GetTree().Root.GetNode<LevelManager>("Level");
		_navigationAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");

		_movementTargetPosition = GlobalTransform.Origin;

		_navigationAgent.PathDesiredDistance = 10.0f;
        _navigationAgent.TargetDesiredDistance = 10.0f;

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
			//TODO: Make a decision based on behavior.
			//Should I go on offensive or be defensive?
			//Offensive: go after player units
			//Defensive: Should I hold my position
			//Sentry: Should I chase player units after they enter my weapon range
		}

		Navigate();

		await HandleCombat();
	}

	public void WeaponRangeEntered(Node2D node)
	{
		if(node as Unit == null) return;

		Unit target = (Unit)node;
	
		if(target.MyTargetGroup == HostileTargetGroup)
		{
			_targetsInWeaponRange.Add(target);
		}
	}

	public void WeaponRangeExitted(Node2D node)
	{
		if(node as Unit == null) return;

		Unit target = (Unit)node;
	
		if(target.MyTargetGroup == HostileTargetGroup)
		{
			_targetsInWeaponRange.Remove(target);
		}
	}
	
	protected void HandleBehavior()
	{
		if(UnitBehavior == UnitBehavior.Offense)
		{
			//TODO: Find the unit in the hostile target group that's closest to me
			//Maybe the level manager keeps two lists of units, one for each side to pull from, then do a distance calculation?
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
				_levelManager.SelectedShip.Destination = GlobalPosition;
				GD.Print("Ship targeted");
			}
		}
	}

	protected void Navigate()
	{
		if (_navigationAgent.IsNavigationFinished())
        {
            return;
        }

		Vector2 currentAgentPosition = GlobalTransform.Origin;
        Vector2 nextPathPosition = _navigationAgent.GetNextPathPosition();

		LookAtNextPathPoint();

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

        Vector2 vectorToPoint = _navigationAgent.GetNextPathPosition();
        float angle = vectorToPoint.Angle();
        float rotation = GlobalRotation;

        if(Math.Abs(angle) - Math.Abs(rotation) < 0.25f)
        {
            CurrentSpeed = MaxSpeed;
        }
        else
        {
            CurrentSpeed = SpeedWhileTurning;
        }

        GlobalRotation = Mathf.LerpAngle(rotation, angle, TurnModifier);
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
        // Sprite2D largeBoom = (Sprite)LargeExplosion.Instance();
        // largeBoom.GlobalPosition = GlobalPosition;
        // GetTree().Root.AddChild(largeBoom);

        QueueFree();
    }

	public virtual void Damage(int damage)
    {
		GD.Print("Damage");
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
