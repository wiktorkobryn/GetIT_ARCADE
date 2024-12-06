using Godot;
using System;

public partial class VfxController : Node2D
{
  public override void _Process(double delta)
  {
    var animation = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

    if (!animation.IsPlaying())
    {
      QueueFree();
    }
  }
}
