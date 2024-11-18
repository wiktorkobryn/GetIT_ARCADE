using Godot;
using System;

public partial class Gun : Sprite2D
{
  [Export]
  public PackedScene bullet_scene;
  private Marker2D bulletSpawnPoint;

  public override void _Ready()
  {
    bulletSpawnPoint = GetNode<Marker2D>("BulletSpawnPoint");
  }

  private void OnPlayerShoot()
  {
    var bullet = bullet_scene.Instantiate<Bullet>();
    bullet.GlobalPosition = bulletSpawnPoint.GlobalPosition;
    bullet.direction = GlobalPosition.DirectionTo(bulletSpawnPoint.GlobalPosition);
    bullet.rotation = GlobalPosition.AngleToPoint(bulletSpawnPoint.GlobalPosition);
    GetNode("/root").AddChild(bullet);
  }
}
