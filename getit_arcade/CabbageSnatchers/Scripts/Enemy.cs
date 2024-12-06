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
	[Export]
	public int ENEMY_TYPE = 1;
	[Export]
	public float COLLECTIBLE_DROP_CHANCE = 1f;
	private bool isAlive = true;
	private Player player;
	private AnimatedSprite2D animatedSprite;
	private CollisionShape2D collisionShape;
	private Timer corpseTimer;

	private PackedScene collectibleScene;
	[Signal]
	public delegate void KilledEventHandler(int enemyType);

	public override void _Ready()
	{
		player = GetNode<Player>("/root/Game/Player");
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

		MoveAndSlide();
	}

	private void OnHurtboxAreaEntered(Area2D area)
	{
		if (!isAlive) return;

		animatedSprite.Play("Damage");
		HP -= 1;

		if (HP == 0)
		{
			HandleDeath();
		}
	}

	private void OnCorpseTimerTimeout()
	{
		QueueFree();
	}

	private void HandleDeath()
	{
		isAlive = false;
		animatedSprite.Animation = "Death";
		//corpseTimer.Start();
		collisionShape.SetDeferred("disabled", true);
		EmitSignal(SignalName.Killed, ENEMY_TYPE);
		Random r = new Random();

		if (r.NextDouble() <= COLLECTIBLE_DROP_CHANCE)
		{
			CallDeferred("DropCollectible");
		}
	}

	private void DropCollectible()
	{
		var collectible_scene = GD.Load<PackedScene>("res://CabbageSnatchers/Scenes/Collectible.tscn");
		var collectible = collectible_scene.Instantiate<Collectible>();

		collectible.GlobalPosition = this.GlobalPosition;
		collectible.collectibleType = CollectibleType.HEALTH;
		GetNode("/root/Game/World").AddChild(collectible);
	}
}
