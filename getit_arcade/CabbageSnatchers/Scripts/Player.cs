using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export]
	public float shot_delay = 0.5f;
	public const float MAX_SPEED = 1000.0f;
	public const float ACCELERATION = 1500.0f;
	public const float FRITION = 2000.0f;

	private AnimatedSprite2D animatedSprite;
	private Sprite2D arm;
	private Sprite2D gun;
	private Timer shotTimer;
	private bool canShoot = true;

	[Signal]
	public delegate void ShootEventHandler();

	public override void _Ready()
	{
		animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		arm = GetNode<Sprite2D>("Arm");
		gun = GetNode<Sprite2D>("Arm/Gun");
		// shotTimer = GetNode<Timer>("ShotTimer");
		shotTimer = new Timer();
		shotTimer.WaitTime = shot_delay;
		shotTimer.Timeout += OnShotTimerTimeout;
		AddChild(shotTimer);
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		if (Input.IsMouseButtonPressed(MouseButton.Left) && canShoot)
		{
			EmitSignal(SignalName.Shoot);
			canShoot = false;
			shotTimer.Start();
		}

		Vector2 inputVector = Vector2.Zero;
		inputVector.X = Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left");
		inputVector.Y = Input.GetActionStrength("ui_down") - Input.GetActionStrength("ui_up");
		inputVector = inputVector.Normalized();

		if (inputVector != Vector2.Zero)
		{
			velocity = velocity.MoveToward(inputVector * MAX_SPEED, ACCELERATION * (float)delta);
			animatedSprite.FlipH = inputVector.X < 0;
			arm.Rotation = Vector2.Zero.AngleToPoint(inputVector);
			gun.FlipV = inputVector.X < 0;
			gun.Offset = inputVector.X < 0 ? new Vector2(0, 50) : new Vector2(0, 0);
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
}