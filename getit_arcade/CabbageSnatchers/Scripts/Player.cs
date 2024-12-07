using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export]
	public float shot_delay = 0.3f;
	[Export]
	public int HP = 7;
	[Export]
	public int score = 0;

	public const int MAX_HP = 7;

	public const float MAX_SPEED = 200.0f;
	public const float ACCELERATION = 700.0f;
	public const float FRITION = 700.0f;

	private AnimatedSprite2D animatedSprite;
	private Sprite2D arm;
	private Sprite2D gun;
	private Timer shotTimer;
	private bool canShoot = true;
	private bool isAlive = true;

	[Signal]
	public delegate void ShootEventHandler();
	[Signal]
	public delegate void UpdateHealthBarHudEventHandler(int value);
	[Signal]
	public delegate void DeathEventHandler(int score);

	public override void _Ready()
	{
		animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		arm = GetNode<Sprite2D>("Arm");
		gun = GetNode<Sprite2D>("Arm/Gun");
		shotTimer = new Timer();
		shotTimer.WaitTime = shot_delay;
		shotTimer.Timeout += OnShotTimerTimeout;
		AddChild(shotTimer);
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!isAlive) return;
		if (animatedSprite.Animation == "Damage" && animatedSprite.IsPlaying()) return;

		Vector2 velocity = Velocity;

		if (Input.IsActionPressed("shoot") && canShoot)
		{
			EmitSignal(SignalName.Shoot);
			canShoot = false;
			shotTimer.Start();
		}

		Vector2 inputVector = Vector2.Zero;
		inputVector.X = Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left");
		inputVector.Y = Input.GetActionStrength("ui_down") - Input.GetActionStrength("ui_up");
		inputVector = inputVector.Normalized();

		Vector2 aimVector = Vector2.Zero;
		aimVector.X = Input.GetActionStrength("aim_right") - Input.GetActionStrength("aim_left");
		aimVector.Y = Input.GetActionStrength("aim_down") - Input.GetActionStrength("aim_up");
		aimVector = aimVector.Normalized();

		if (aimVector != Vector2.Zero)
		{
			arm.Rotation = Vector2.Zero.AngleToPoint(aimVector);
			gun.FlipV = aimVector.X < 0;
			gun.Offset = aimVector.X < 0 ? new Vector2(0, 50) : new Vector2(0, 0);
		}
		else
		{
			arm.Rotation = Vector2.Zero.AngleToPoint(inputVector);
			gun.FlipV = inputVector.X < 0;
			gun.Offset = inputVector.X < 0 ? new Vector2(0, 50) : new Vector2(0, 0);
		}


		if (inputVector != Vector2.Zero)
		{
			velocity = velocity.MoveToward(inputVector * MAX_SPEED, ACCELERATION * (float)delta);
			animatedSprite.FlipH = inputVector.X < 0;
			arm.Position = inputVector.X < 0 ? new Vector2(-25, -45) : new Vector2(13, -45);
			animatedSprite.Play("Run");
		}
		else
		{
			velocity = velocity.MoveToward(Vector2.Zero, FRITION * (float)delta);
			animatedSprite.Play("Idle");
		}

		Velocity = velocity;
		MoveAndSlide();
	}

	private void OnShotTimerTimeout()
	{
		canShoot = true;
	}

	private void OnHurtboxAreaEntered(Area2D area)
	{
		if (!isAlive) return;

		HP -= 1;
		EmitSignal(SignalName.UpdateHealthBarHud, HP);
		animatedSprite.Play("Damage");

		if (HP == 0)
		{
			isAlive = false;
			animatedSprite.Play("Death");
			EmitSignal(SignalName.Death, score);
		}
		else
		{
			Enemy enemy = area.GetParent() as Enemy;
			if (enemy != null)
				enemy.KillUnit();
		}
	}

	public void AddHealth(int value)
	{
		HP = Math.Min(HP + value, MAX_HP);
		EmitSignal(SignalName.UpdateHealthBarHud, HP);
	}

	void OnEnemySpawnerAddScore(int value)
	{
		score += value;
	}
}
