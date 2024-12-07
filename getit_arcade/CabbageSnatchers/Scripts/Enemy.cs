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
	public EnemyType enemyType;
	[Export]
	public float shot_delay = 4f;

	[Export]
	public PackedScene bulletScene;

	[Export]
	public float COLLECTIBLE_DROP_CHANCE = 1f;
	private bool isAlive = true;
	private bool canShoot = true;

	private Player player;
	private AnimatedSprite2D animatedSprite;
	private CollisionShape2D collisionShape;
	private Timer corpseTimer;
	private Timer shotTimer;

	private PackedScene collectibleScene;
	[Signal]
	public delegate void KilledEventHandler(int enemyType);

	public override void _Ready()
	{
		player = GetNode<Player>("/root/Game/Scene/Player");
		animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		collisionShape = GetNode<CollisionShape2D>("Hurtbox/CollisionShape2D");
		corpseTimer = GetNode<Timer>("CorpseTimer");
		shotTimer = new Timer();
		shotTimer.WaitTime = shot_delay;
		shotTimer.Timeout += OnShotTimerTimeout;
		AddChild(shotTimer);
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!isAlive) return;

		if (animatedSprite.Animation == "Damage" && animatedSprite.IsPlaying()) return;

		var direction = (player.Position - Position).Normalized();
		Velocity = Velocity.MoveToward(direction * MAX_SPEED, ACCELERATION * (float)delta);
		animatedSprite.Play("Run");

		if (enemyType == EnemyType.Onio && canShoot)
		{
			var bullet = bulletScene.Instantiate<Bullet>();
			bullet.Position = Position;
			bullet.direction = direction;
			bullet.rotation = GlobalPosition.AngleToPoint(player.GlobalPosition);
			GetNode("/root/Game/Scene/World").AddChild(bullet);
			canShoot = false;
			shotTimer.Start();
		}

		if (Position.X > player.Position.X)
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
			KillUnit();
		}
	}

	public void KillUnit()
	{
		PlayDeathVFX();
		isAlive = false;
		animatedSprite.Animation = "Death";
		//corpseTimer.Start();
		collisionShape.SetDeferred("disabled", true);

		// disabling collider in next frame
		GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", true);
		SetPhysicsProcess(false);

		EmitSignal(SignalName.Killed, (int)enemyType);
		Random r = new Random();

		if (r.NextDouble() <= COLLECTIBLE_DROP_CHANCE)
		{
			CallDeferred("DropCollectible");
		}
	}

	private void OnCorpseTimerTimeout()
	{
		QueueFree();
	}

	private void DropCollectible()
	{
		var collectible_scene = GD.Load<PackedScene>("res://CabbageSnatchers/Scenes/Collectible.tscn");
		var collectible = collectible_scene.Instantiate<Collectible>();

		collectible.GlobalPosition = this.GlobalPosition;
		collectible.collectibleType = CollectibleType.HEALTH;
		GetNode("/root/Game/Scene/World").AddChild(collectible);
	}

	private void PlayDeathVFX()
	{
		var vfxScene = GD.Load<PackedScene>("res://CabbageSnatchers/Scenes/VFX/EnemyDeathVFX.tscn");
		var vfx = vfxScene.Instantiate<Node2D>();
		vfx.Position = Vector2.Zero;
		AddChild(vfx);
	}

	private void OnShotTimerTimeout()
	{
		canShoot = true;
	}
}
