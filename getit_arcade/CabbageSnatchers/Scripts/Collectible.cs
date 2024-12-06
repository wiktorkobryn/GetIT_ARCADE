using Godot;
using System;
using System.Collections.Generic;

public partial class Collectible : Area2D
{
  [Export]
  public CollectibleType collectibleType = CollectibleType.HEALTH;
  private readonly Dictionary<CollectibleType, string> spritePath = new()
  {
	{ CollectibleType.HEALTH, "res://CabbageSnatchers/Textures/UI/Powerups/PowerupHealth.png" },
	{ CollectibleType.INVICIBILITY, "res://CabbageSnatchers/Textures/UI/Powerups/PowerupInvincibility.png" }
  };

  public override void _Ready()
  {
	GetNode<Sprite2D>("Sprite2D").Texture = (Texture2D)GD.Load(spritePath[collectibleType]);
  }

  void OnAreaEntered(Area2D area)
  {
	QueueFree();
  }

  void OnBodyEntered(Node2D node)
  {
	if (node is Player)
	{
	  var player = (Player)node;

	  switch (collectibleType)
	  {
		case CollectibleType.HEALTH:
		  player.AddHealth(1);
		  break;
		case CollectibleType.INVICIBILITY:
		  break;
		default:
		  break;
	  }
	}
  }
}
