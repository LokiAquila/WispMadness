using Godot;
using System;


public partial class Player : CharacterBody2D
{
	// Vitesse de déplacement du joueur.
	[Export]
	public float Speed = 250.0f;
	
	[Export]
	public PackedScene ProjectileScene;
	
	[Export]
	public double shootingCooldown = 0.3; // Délai en secondes entre chaque tir
	
	// Champ de vision actuel du joueur.
	private float vitality;

	// Point de départ de la lumière du joueur.
	private PointLight2D playerLight;

	// Animation du joueur.
	private AnimatedSprite2D playerSprite;

	// Timer pour gérer la diminution de la lumière.
	private Timer lightTimer;
	
	public Camera2D camera;

	// Déplacement du joueur.
	
	private Vector2 _screenSize; // Size of the game window.
	
	private double shootingTimer = 0; // Compteur de temps pour suivre le délai entre les tirs
	
	[Signal]
	public delegate void PlayerDeathEventHandler(Player player);

	public override void _Ready()
	{
		_screenSize = GetViewportRect().Size;
		// Récupérer les nœuds enfants.
		playerLight = GetNode<PointLight2D>("PlayerLight");
		lightTimer = GetNode<Timer>("LightTimer");
		
		
		playerSprite = GetNode<AnimatedSprite2D>("PlayerSprite");
		
		// Initialiser le champ de vision du joueur.
		vitality = 1; // 1 pour 100% de l'échelle initiale
		playerLight.TextureScale = vitality;
	
		camera = GetNode<Camera2D>("Camera2D");
		camera.Enabled = true;
		camera.MakeCurrent();
		camera.Align();
		camera.PositionSmoothingEnabled = true;
		camera.PositionSmoothingSpeed = 10;

		// Connecter le signal "timeout" du timer à la méthode "OnLightTimerTimeout"
		lightTimer.Timeout += OnLightTimerTimeout;
		
		playerSprite.AnimationLooped += _on_player_sprite_animation_looped;
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 direction =  Input.GetVector("Gauche", "Droite", "Haut", "Bas");
		Velocity = direction * Speed;
		MoveAndSlide();
	}

	public override void _Process(double delta)
	{
		// Vérifiez si le joueur peut tirer en fonction du délai entre les tirs
		if (Input.IsActionJustPressed("Attaque") && shootingTimer >= shootingCooldown)
		{
			Shoot();
			shootingTimer = 0;	 // Réinitialisez le compteur de temps après avoir tiré
		}
		
		if (shootingTimer < shootingCooldown)
		{
			shootingTimer += delta;
		}
	}
	
	private void Shoot()
	{
		var projectile = ProjectileScene.Instantiate<Bullet>();
		GetParent().AddChild(projectile); // Ajoute le projectile à la scène
		projectile.Position = Position; // Démarre le projectile à la position du joueur

		// Calcule la direction de la souris par rapport au joueur
		var mousePosition = GetGlobalMousePosition();
		var directionToMouse = (mousePosition - Position).Normalized();

		projectile.Direction = directionToMouse; // Direction basée sur la position de la souris
		playerSprite.Play("attack_player");
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
	
	public void _on_player_sprite_animation_looped()
	{
		if (playerSprite.Animation == "attack_player")
		{
			playerSprite.Play("walk_player");
			Console.WriteLine("Changement d'animation");
		}
		
	}
	
	public void OnOrbPickedUp(Orb orb, float orbVitality)
	{
		// Augmenter la vitalité du joueur sans dépasser la vitalité maximale
		GD.Print("Gob mon orb : " + orbVitality);
		vitality = Mathf.Min(vitality + orbVitality, 1);
		playerLight.TextureScale = vitality;
	}
	
	public void OnMobContact(Mob mob)
	{
		// Augmenter la vitalité du joueur sans dépasser la vitalité maximale
		GD.Print("MARCHE STP");
		vitality = (float) Mathf.Max(vitality - 0.5, 0);
		playerLight.TextureScale = vitality;
	}

	public void Kill()
	{
		playerSprite.Play("death_player");
		EmitSignal(nameof(PlayerDeathEventHandler), this); // Émettre le signal
	}
}
