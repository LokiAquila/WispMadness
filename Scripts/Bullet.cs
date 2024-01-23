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
	
	private Vector2 PlayerVelocity; // Ajoutez cette variable
	
	

	public override void _Ready()
	{
		// Détruire le projectile après un certain temps
		Player player = GetNode<Player>("../Player");
		PlayerVelocity = player.Velocity;
		
		_timer = new Timer();
		_timer.WaitTime = Lifespan;
		_timer.OneShot = true;
		var timerCallable = new Callable(this, nameof(_OnTimerTimeout));
		_timer.Connect("timeout", timerCallable);
		AddChild(_timer);
		_timer.Start();
	}

	public override void _Process(double delta)
	{
		Position += (Direction * Speed + PlayerVelocity) * (float)delta;
	}
	
	private void _OnTimerTimeout()
	{
		QueueFree();
	}
}
