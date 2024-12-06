using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
	[Export]
	public float MAX_SPEED = 100.0f;
	[Export]
	public float ACCELERATION = 170.0f;
	[Export]
	public int HP = 3;
	private bool isAlive = true;
	private Player player;
	private AnimatedSprite2D animatedSprite;
	private CollisionShape2D collisionShape;
	private Timer corpseTimer;

	public override void _Ready()
	{
		player = GetNode<Player>("/root/Game/Scene/Player");
		animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		collisionShape = GetNode<CollisionShape2D>("Hurtbox/CollisionShape2D");
		corpseTimer = GetNode<Timer>("CorpseTimer");
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!isAlive) return;

		if (animatedSprite.Animation == "Damage" && animatedSprite.IsPlaying()) return;

		var direction = (player.Position - Position).Normalized();
		Velocity = Velocity.MoveToward(direction * MAX_SPEED, ACCELERATION * (float)delta);
		animatedSprite.Play("Run");

		if(Position.X > player.Position.X)
			animatedSprite.FlipH = true;
		else
			animatedSprite.FlipH = false;

		MoveAndSlide();
	}

	private void OnHurtboxAreaEntered(Area2D area)
	{
		if (!isAlive) return;

		animatedSprite.Play("Damage");

		HP -= 1;

		if (HP == 0)
		{
			isAlive = false;
			animatedSprite.Animation = "Death";
			//corpseTimer.Start();
			collisionShape.SetDeferred("disabled", true);

			// disabling collider in next frame
			GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", true);
			SetPhysicsProcess(false);
		}
	}

	private void OnCorpseTimerTimeout()
	{
		QueueFree();
	}
}
