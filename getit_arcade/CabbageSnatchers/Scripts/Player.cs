using Godot;
using System;

public partial class Player : CharacterBody2D
{
    [Export] public float shot_delay = 0.3f;
    [Export] public int HP = 7;
    [Export] public int score = 0;
    [Export] public Label scoreLbl = null;
    [Export] public float dashSpeed = 400.0f;
    [Export] public float dashDuration = 0.2f;
    [Export] public float dashCooldown = 1.0f;
	[Export] public Sprite2D gunSprite, katanaSprite;

    private const int MAX_HP = 7;
    private const float MAX_SPEED = 200.0f;
    private const float ACCELERATION = 700.0f;
    private const float FRICTION = 700.0f;
	private const float DASH_OMNIVAMP_CHANCE = 0.7f;
    
    private AnimatedSprite2D animatedSprite;
    private Sprite2D arm;
    private Sprite2D gun;
    private Timer shotTimer;
    private Timer dashTimer;
    private Timer dashCooldownTimer;
    private bool canShoot = true;
    private bool isAlive = true;
    private bool isDashing = false;

    [Signal] public delegate void ShootEventHandler();
    [Signal] public delegate void UpdateHealthBarHudEventHandler(int value);
    [Signal] public delegate void DeathEventHandler(int score);

    public override void _Ready()
    {
        animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        arm = GetNode<Sprite2D>("Arm");
        gun = GetNode<Sprite2D>("Arm/Gun");

        shotTimer = new Timer { WaitTime = shot_delay };
        shotTimer.Timeout += OnShotTimerTimeout;
        AddChild(shotTimer);
        
        dashTimer = new Timer { WaitTime = dashDuration, OneShot = true };
        dashTimer.Timeout += OnDashEnd;
        AddChild(dashTimer);
        
        dashCooldownTimer = new Timer { WaitTime = dashCooldown, OneShot = true };
        AddChild(dashCooldownTimer);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!isAlive) return;
        if (animatedSprite.Animation == "Damage" && animatedSprite.IsPlaying()) return;

        Vector2 velocity = Velocity;
        Vector2 inputVector = new Vector2(
            Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left"),
            Input.GetActionStrength("ui_down") - Input.GetActionStrength("ui_up")
        ).Normalized();

        Vector2 aimVector = new Vector2(
            Input.GetActionStrength("aim_right") - Input.GetActionStrength("aim_left"),
            Input.GetActionStrength("aim_down") - Input.GetActionStrength("aim_up")
        ).Normalized();

        if (aimVector != Vector2.Zero)
        {
            arm.Rotation = Vector2.Zero.AngleToPoint(aimVector);
            gun.FlipV = aimVector.X < 0;
            gun.Offset = aimVector.X < 0 ? new Vector2(0, 50) : new Vector2(0, 0);
        }
        else if (inputVector != Vector2.Zero)
        {
            arm.Rotation = Vector2.Zero.AngleToPoint(inputVector);
            gun.FlipV = inputVector.X < 0;
            gun.Offset = inputVector.X < 0 ? new Vector2(0, 50) : new Vector2(0, 0);
        }

        if (Input.IsActionPressed("ui_square") && canShoot)
        {
            EmitSignal(SignalName.Shoot);
            canShoot = false;
            shotTimer.Start();
        }
        
        if (Input.IsActionJustPressed("ui_triangle") && !isDashing && dashCooldownTimer.IsStopped())
        {
            isDashing = true;
            dashTimer.Start();
            dashCooldownTimer.Start();
        }
        
        if (isDashing && inputVector != Vector2.Zero)
        {
			gunSprite.Visible = false;
			katanaSprite.Visible = true;

            velocity = inputVector * dashSpeed;
			animatedSprite.Play("Dash");
        }
        else
        {
			gunSprite.Visible = true;
			katanaSprite.Visible = false;

			if (inputVector != Vector2.Zero && Input.IsActionPressed("ui_R1"))
			{
				velocity = velocity.MoveToward(inputVector * MAX_SPEED, ACCELERATION * (float)delta);
				animatedSprite.FlipH = inputVector.X < 0;
				arm.Position = inputVector.X < 0 ? new Vector2(-25, -45) : new Vector2(13, -45);
				animatedSprite.Play("Run");
			}
			else
			{
				velocity = velocity.MoveToward(Vector2.Zero, FRICTION * (float)delta);
				animatedSprite.Play("Idle");
			}
        }

        Velocity = velocity;
        MoveAndSlide();
    }

    private void OnDashEnd()
    {
        isDashing = false;
    }

    private void OnShotTimerTimeout()
    {
        canShoot = true;
    }

    private void OnHurtboxAreaEntered(Area2D area)
    {
        if (!isAlive) return;

		if (isDashing)
		{
			if (area.GetParent() is Enemy enemy)
			{
				enemy.DealDamage(2);

				Random r = new Random();
				if (r.NextDouble() <= DASH_OMNIVAMP_CHANCE)
				{
					HP += 1;
					EmitSignal(SignalName.UpdateHealthBarHud, HP);
				}
			}
		}
		else
		{
			HP -= 1;
			animatedSprite.Play("Damage");
			EmitSignal(SignalName.UpdateHealthBarHud, HP);
			
			if (HP == 0)
			{
				isAlive = false;
				animatedSprite.Play("Death");
				EmitSignal(SignalName.Death, score);
			}
			else
			{
				if (area.GetParent() is Enemy enemy)
					enemy.KillUnit();
			}
		}
    }

    public void AddHealth(int value)
    {
        HP = Math.Min(HP + value, MAX_HP);
        EmitSignal(SignalName.UpdateHealthBarHud, HP);
    }

    private void OnEnemySpawnerAddScore(int value)
    {
        score += value;
        if (scoreLbl != null)
            scoreLbl.Text = "SCORE:  " + score;
    }
}