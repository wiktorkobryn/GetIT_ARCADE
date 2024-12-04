using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
	[Export]
	public float MAX_SPEED = 120.0f;
	[Export]
	public float ACCELERATION = 200.0f;
	[Export]
	public int HP = 3;
	private bool isAlive = true;
	private Player player;
	private AnimatedSprite2D animatedSprite;
	private Timer corpseTimer;

	public override void _Ready()
	{
		player = GetNode<Player>("/root/Game/Player");
		animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		corpseTimer = GetNode<Timer>("CorpseTimer");
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!isAlive) return;

		if (animatedSprite.Animation == "Damage" && animatedSprite.IsPlaying()) return;

		var direction = (player.Position - Position).Normalized();
		Velocity = Velocity.MoveToward(direction * MAX_SPEED, ACCELERATION * (float)delta);
		animatedSprite.Play("Run");

		MoveAndSlide();
	}

	private void OnHurtboxAreaEntered(Area2D area)
	{
		animatedSprite.Play("Damage");

		HP -= 1;

		if (HP == 0)
		{
			isAlive = false;
			//animatedSprite.Animation = "Death";
			corpseTimer.Start();
		}
	}

	private void OnCorpseTimerTimeout()
	{
		QueueFree();
	}
}
