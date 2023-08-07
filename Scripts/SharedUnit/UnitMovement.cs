using System;
using Godot;


public class UnitMovement
{
    private Unit _unit;

    public UnitMovement(Unit unit)
    {
        _unit = unit;
    }

    public void WarpTo(Vector2 location)
	{
		_unit.IsWarping = true;
		_unit.WarpTarget = location;
		_unit.MovementTarget = location;
		_unit.MovementTargetPosition = location;
	}

	public void WarpOut(Vector2 location)
	{
		_unit.TurningAngleThreshold = 0.99f;
		_unit.SpeedWhileTurning = 1;
		_unit.IsWarping = true;
		_unit.WarpTarget = location;
		_unit.MovementTarget = location;

		_unit.IsRetreating = true;
		_unit.AudioStreamPlayer.PlayAudio(_unit.AudioStreamPlayer.RetreatClickedSoundClip);
	}

	public void Navigate()
	{
		float distanceToTarget = _unit.MovementTarget.DistanceTo(_unit.GlobalPosition);

		if (_unit.NavigationAgent.IsNavigationFinished() || distanceToTarget <= _unit.TargetDesiredDistance)
        {
			if(_unit.IsWarping)
			{
				_unit.AudioStreamPlayer.PlayAudio(_unit.AudioStreamPlayer.WarpInSoundClip);

				_unit.IsWarping = false;
				_unit.Collision.Disabled = false;
			}

			if(_unit.IsRetreating)
			{
				_unit.HandlePostRetreat();
			}

            return;
        }

		LookAtNextPathPoint();

		Vector2 currentAgentPosition = _unit.GlobalTransform.Origin;
        Vector2 nextPathPosition = _unit.NavigationAgent.GetNextPathPosition();

		Vector2 newVelocity = (nextPathPosition - currentAgentPosition).Normalized();
        newVelocity *= _unit.CurrentSpeed;

		_unit.NavigationAgent.SetVelocity(newVelocity);
	}

    protected virtual void LookAtNextPathPoint()
    {
        if(!IsLookingAtTarget())
        {
			_unit.CurrentSpeed = _unit.SpeedWhileTurning;
        }
        else
        {
			if(_unit.IsWarping)
			{
				_unit.Collision.Disabled = true;
				_unit.CurrentSpeed = 3000;
			}
			else
			{
				_unit.CurrentSpeed = _unit.MaxSpeed;
			}
        }

		float angleTo = _unit.GlobalPosition.AngleToPoint(_unit.NavigationAgent.GetNextPathPosition());
        float currentAngle = _unit.GlobalRotation;

		_unit.GlobalRotation = Mathf.LerpAngle(currentAngle, angleTo, _unit.Weight);
    }

	public bool IsLookingAtTarget()
    {
        // Get the direction the CharacterBody is facing
        Vector2 forwardDirection = new Vector2(Mathf.Cos(_unit.Rotation), Mathf.Sin(_unit.Rotation));

        // Calculate the direction to the target vector
        Vector2 directionToTarget = (_unit.NavigationAgent.GetNextPathPosition() - _unit.GlobalPosition).Normalized();

        // Calculate the dot product between the forward direction and the direction to the target
        float dotProduct = forwardDirection.Dot(directionToTarget);

        // Check if the dot product is close to 1.0 (within a tolerance) to determine if the CharacterBody is looking at the target
        float tolerance = _unit.TurningAngleThreshold;
        return dotProduct >= tolerance;
    }

	public async void ActorSetup()
    {
        // Wait for the first physics frame so the NavigationServer can sync.
        await _unit.ToSignal(_unit.GetTree(), SceneTree.SignalName.PhysicsFrame);

        // Now that the navigation map is no longer empty, set the movement target.
        _unit.MovementTarget = _unit.MovementTargetPosition;
    }

}