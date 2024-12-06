using Godot;
using System;
using System.Collections.Generic;

public partial class EnemySpawner : Node2D
{
  [Export]
  public PackedScene enemyScene;
  [Export]
  public int maxEnemiesSpawned = 10;
  private int numOfEnemies = 0;

  [Signal]
  public delegate void AddScoreEventHandler(int score);

  private void OnSpawnTimerTimeout()
  {
    var enemy = enemyScene.Instantiate<Enemy>();

    if (numOfEnemies < maxEnemiesSpawned)
    {
      SpawnEnemy(enemy);
    }
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
    enemy.AddToGroup("enemies");
    enemy.Killed += OnEnemyKilled;
    AddChild(enemy);
    numOfEnemies += 1;
  }

  void OnPlayerDeath(int score)
  {
    GetNode<Timer>("SpawnTimer").Stop();
  }

  void OnEnemyKilled(int enemyType)
  {
    numOfEnemies -= 1;
    EmitSignal(SignalName.AddScore, 10 * (enemyType + 1));
  }
}
