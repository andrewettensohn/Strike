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

        float xPos = (float)GD.RandRange(spawnPosition.X - 300, spawnPosition.X + 300);
        float yPos = (float)GD.RandRange(spawnPosition.Y - 300, spawnPosition.Y + 300);

        missile.GlobalPosition = new Vector2(xPos, yPos);
        missile.Target = targetShip;

        missile.AddToGroup(myTargetGroup.ToString());
        //missile.HostileBulletTargetGroup = hostileTargetGroup;

        GetTree().Root.AddChild(missile);
    }
}
