using Godot;
using System;

public partial class Player : CharacterBody2D
{
	// Vitesse de déplacement du joueur.
	[Export]
	public float Speed = 300.0f;
	
	// Champ de vision actuel du joueur.
	private float vitality;

	// Point de départ de la lumière du joueur.
	private PointLight2D playerLight;

	// Animation du joueur.
	private AnimatedSprite2D playerSprite;

	// Timer pour gérer la diminution de la lumière.
	private Timer lightTimer;

	// Déplacement du joueur.
	private Vector2 velocity = Vector2.Zero;
	
	private Vector2 _screenSize; // Size of the game window.

	public override void _Ready()
	{
		_screenSize = GetViewportRect().Size;
		// Récupérer les nœuds enfants.
		playerLight = GetNode<PointLight2D>("PlayerLight");
		
		playerSprite = GetNode<AnimatedSprite2D>("PlayerSprite");

		// Initialiser le champ de vision du joueur.
		vitality = 1; // 1 pour 100% de l'échelle initiale
		playerLight.TextureScale = vitality;

		// Connecter le signal "timeout" du timer à la méthode "OnLightTimerTimeout".
		lightTimer = GetNode<Timer>("LightTimer");
		var lightTimerCallable = new Callable(this, nameof(OnLightTimerTimeout));
		lightTimer.Connect("timeout", lightTimerCallable);
	}

	public override void _Process(double delta)
	{
		// Gérer les mouvements du joueur.
		velocity = Vector2.Zero;

		if (Input.IsActionPressed("Haut"))
			velocity.Y -= 1;
		if (Input.IsActionPressed("Bas"))
			velocity.Y += 1;
		if (Input.IsActionPressed("Gauche"))
			velocity.X -= 1;
		if (Input.IsActionPressed("Droite"))
			velocity.X += 1;

		// Gérer les animations du joueur en fonction de la direction.
		var animatedSprite2D = GetNode<AnimatedSprite2D>("PlayerSprite");

		if (velocity.Length() > 0)
		{
			velocity = velocity.Normalized() * Speed * (float)delta;
			animatedSprite2D.Play();

			// Mise à jour de la position avec la vélocité
			Position += velocity;

			// Clamp après avoir mis à jour la position
			Position = new Vector2(
				x: Mathf.Clamp(Position.X, 0, _screenSize.X),
				y: Mathf.Clamp(Position.Y, 0, _screenSize.Y)
			);
		}
		else
		{
			animatedSprite2D.Stop();
		}
		
		if (velocity.X != 0 || velocity.Y != 0)
		{
			animatedSprite2D.Animation = "walk";
			animatedSprite2D.FlipV = false;
			// See the note below about boolean assignment.
			animatedSprite2D.FlipH = velocity.X < 0;
		}
		else if (velocity.Y != 0)
		{
			animatedSprite2D.FlipV = velocity.Y > 0;
		}
	}

	public void OnLightTimerTimeout()
	{
		// Diminuer progressivement l'échelle de la lumière du joueur lorsque le timer expire.
		vitality -= 0.01f; // Réduire l'échelle de 10%

		// Arrêter le timer lorsque la lumière est épuisée.
		if (vitality <= 0)
		{
			vitality = 0;
			playerLight.TextureScale = 0; 
			lightTimer.Stop();
		}
		else
		{
			playerLight.TextureScale = vitality;
		}
	}
}
