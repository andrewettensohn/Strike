using Godot;
using System;

public partial class DroneLauncher : Node
{
	[Export]
    public PackedScene DroneScene;

    [Export]
    public float CoolDownTime;

    public void LaunchDrone(Unit parent, Vector2 spawnPosition)
    {
        Drone drone = (Drone)DroneScene.Instantiate();

        float xPos = (float)GD.RandRange(spawnPosition.X, spawnPosition.X);
        float yPos = (float)GD.RandRange(spawnPosition.Y, spawnPosition.Y);

        drone.GlobalPosition = new Vector2(xPos, yPos);
        drone.Parent = parent;

        drone.AddToGroup(parent.MyTargetGroup.ToString());

        GetTree().Root.AddChild(drone);
    }
}
