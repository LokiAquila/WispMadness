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
	
	private Camera2D camera;

	// Déplacement du joueur.
	
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
	
		camera = GetNode<Camera2D>("Camera2D");
		camera.Enabled = true;
		camera.MakeCurrent();
		camera.Align();
		camera.PositionSmoothingEnabled = true;
		camera.PositionSmoothingSpeed = 3;

		// Connecter le signal "timeout" du timer à la méthode "OnLightTimerTimeout".
		lightTimer = GetNode<Timer>("LightTimer");
		var lightTimerCallable = new Callable(this, nameof(OnLightTimerTimeout));
		lightTimer.Connect("timeout", lightTimerCallable);
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 direction =  Input.GetVector("Gauche", "Droite", "Haut", "Bas");
		Velocity = direction * Speed;
		MoveAndSlide();
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
