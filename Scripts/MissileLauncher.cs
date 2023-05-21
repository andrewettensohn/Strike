using Godot;
using System;

public partial class MissileLauncher : Node
{
    [Export]
    public PackedScene MissileScene;

    [Export]
    public float CoolDownTime;

    [Export]
    public int MissileAmmoCount;

    [Export]
    public float MissileRange;

    [Export]
    public int MissilesPerSalvo;

    public int MissilesFired { get; private set; }

    // public void FireSalvo(Unit targetShip, TargetGroup myTargetGroup, TargetGroup hostileTargetGroup, Vector2 spawnPosition, bool isRocket = false)
    // {
    //     if(MissilesFired >= MissileAmmoCount)
    //     {
    //         return;
    //     }

    //     if(spawnPosition.DistanceTo(targetShip.GlobalPosition) > MissileRange)
    //     {
    //         return;
    //     }

    //     MissilesFired += 1;

    //     int missilesSpawned = 0;
    //     while (missilesSpawned < MissilesPerSalvo)
    //     {
    //         FireMissile(targetShip, myTargetGroup, hostileTargetGroup, spawnPosition);
    //         missilesSpawned += 1;
    //     }


    //     //GetNode<AudioStreamPlayer2D>("MissileLaunchAudioPlayer").Play();
    // }

    public void FireMissile(Unit targetShip, TargetGroup myTargetGroup, TargetGroup hostileTargetGroup, Vector2 spawnPosition)
    {
        Missile missile = (Missile)MissileScene.Instantiate();

        float xPos = (float)GD.RandRange(spawnPosition.X, spawnPosition.X);
        float yPos = (float)GD.RandRange(spawnPosition.Y, spawnPosition.Y);

        missile.GlobalPosition = new Vector2(xPos, yPos);
        missile.Target = targetShip;
        missile.MyTargetGroup = myTargetGroup;

        missile.AddToGroup(myTargetGroup.ToString());

        GetTree().Root.AddChild(missile);
    }

    public Vector2 FireRocket(Unit targetShip, TargetGroup myTargetGroup, TargetGroup hostileTargetGroup, Vector2 spawnPosition)
    {
        Rocket rocket = (Rocket)MissileScene.Instantiate();

        float xPos = (float)GD.RandRange(spawnPosition.X, spawnPosition.X);
        float yPos = (float)GD.RandRange(spawnPosition.Y, spawnPosition.Y);

        rocket.GlobalPosition = new Vector2(xPos, yPos);
        rocket.TargetLocation = targetShip.GlobalPosition;
        rocket.MyTargetGroup = myTargetGroup;

        rocket.AddToGroup(myTargetGroup.ToString());

        GetTree().Root.AddChild(rocket);

        return rocket.GlobalPosition;
    }

    public void FireRocketBarrage(Unit targetShip, TargetGroup myTargetGroup, TargetGroup hostileTargetGroup, Vector2 spawnPosition)
    {
        Vector2 lastFiredRocketPos = spawnPosition;
        int rocketsSpawned = 0;
        while (rocketsSpawned < MissilesPerSalvo)
        {
            lastFiredRocketPos = new Vector2(lastFiredRocketPos.X + 25, lastFiredRocketPos.Y + 25);
            lastFiredRocketPos = FireRocket(targetShip, myTargetGroup, hostileTargetGroup, lastFiredRocketPos);
            rocketsSpawned += 1;
        }
    }
}
