using Godot;
using System;

public partial class Bullet : Area2D
{
	public Vector2 Direction;
	[Export]
	public float Speed = 300;
	[Export]
	public float Lifespan = 1f;

	private Timer _timer;
	
	private Vector2 PlayerVelocity;
	
	private AnimatedSprite2D bulletSprite;


	public override void _Ready()
	{
		Player player = GetNode<Player>("../Player");
		PlayerVelocity = player.Velocity;
		bulletSprite = GetNode<AnimatedSprite2D>("BulletSprite");
		
		BodyEntered += OnAreaEntered;
		AreaEntered += OnAreaEntered;
		
		
		_timer = new Timer();
		_timer.WaitTime = Lifespan;
		_timer.OneShot = true;
		var timerCallable = new Callable(this, nameof(_OnTimerTimeout));
		_timer.Connect("timeout", timerCallable);
		AddChild(_timer);
		_timer.Start();
		bulletSprite.Play("bullet_player");
	}

	public override void _Process(double delta)
	{
		Vector2 adaptedSpeed = (Direction * Speed + PlayerVelocity) * (float)delta;
		Vector2 defaultSpeed = Direction * Speed * (float)delta;
		Position += defaultSpeed.Length() > adaptedSpeed.Length() ? defaultSpeed : adaptedSpeed;
	}
	
	private void _OnTimerTimeout()
	{
		QueueFree();
	}

	private void OnAreaEntered(Node2D body)
	{
		if (body.Name != "Player")
		{
			QueueFree();
		}
	}
	
	private void OnAreaEntered(Area2D area)
	{
		if (area is Mob)
		{
			QueueFree();
		}
	}
}
