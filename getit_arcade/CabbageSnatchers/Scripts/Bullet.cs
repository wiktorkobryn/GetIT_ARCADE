using Godot;
using System;

public partial class Bullet : Sprite2D
{
  public const float SPEED = 2500;
  public Vector2 direction;
  public float rotation;

  public override void _Process(double delta)
  {
    Position += direction * SPEED * (float)delta;
    Rotation = rotation;
  }
  private void OnDestroyTimerTimeout()
  {
    QueueFree();
  }

  private void OnHitboxAreaEntered(Area2D area)
  {
    QueueFree();
  }
}

