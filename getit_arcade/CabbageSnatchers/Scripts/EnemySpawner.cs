using Godot;
using System;
using System.Collections.Generic;

public partial class EnemySpawner : Node2D
{
  [Export]
  public PackedScene enemy_scene;

  private void OnSpawnTimerTimeout()
  {
    var enemy = enemy_scene.Instantiate<Enemy>();
    SpawnEnemy(enemy);
  }

  private void SpawnEnemy(Enemy enemy)
  {
    Random r = new Random();
    // var screen = GetViewport().GetVisibleRect().Size;
    var spawnPoints = this.GetNode("SpawnPoints");
    var numOfSpawnPoints = spawnPoints.GetChildCount();
    var position = spawnPoints.GetChild<Marker2D>(r.Next(0, numOfSpawnPoints)).GlobalPosition;
    // var position = new Vector2((int)(screen.X / 0.3), r.Next(-(int)(screen.Y / 0.3), (int)(screen.Y / 0.3)));
    enemy.GlobalPosition = position;
    AddChild(enemy);
  }
}
