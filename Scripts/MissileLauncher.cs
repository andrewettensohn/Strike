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

    public void FireSalvo(Unit targetShip, TargetGroup myTargetGroup, TargetGroup hostileTargetGroup, Vector2 spawnPosition)
    {
        if(MissilesFired >= MissileAmmoCount)
        {
            return;
        }

        if(spawnPosition.DistanceTo(targetShip.GlobalPosition) > MissileRange)
        {
            return;
        }

        MissilesFired += 1;

        int missilesSpawned = 0;
        while (missilesSpawned < MissilesPerSalvo)
        {
            FireMissile(targetShip, myTargetGroup, hostileTargetGroup, spawnPosition);
            missilesSpawned += 1;
        }


        //GetNode<AudioStreamPlayer2D>("MissileLaunchAudioPlayer").Play();
    }

    private void FireMissile(Unit targetShip, TargetGroup myTargetGroup, TargetGroup hostileTargetGroup, Vector2 spawnPosition)
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
}
