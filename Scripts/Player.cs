using Godot;
using System;
using System.Collections.Generic;
using CGJ24.Classes;


public partial class Player : CharacterBody2D
{

	private Timer deathTimer;
	// Vitesse de déplacement du joueur.
	// Vitesse de déplacement du joueur.
	[Export]
	public float Speed = 250.0f;
	
	[Export]
	public PackedScene ProjectileScene;
	
	[Export]
	public double shootingCooldown = 0.3; // Délai en secondes entre chaque tir
	private double shootingTimer = 0; // Compteur de temps pour suivre le délai entre les tirs
	
	// Champ de vision actuel du joueur.
	public float vitality;
	
	private PointLight2D playerLight;
	private AnimatedSprite2D playerSprite;
	public Timer lightTimer;
	public Camera2D camera;
	
	
	//Liste des éléments de la carte interactibles par le joueur
	private readonly List<IPlayerInteractable> interactables = new();
	//Le conteneur du texte d'interaction
	private HBoxContainer labelContainer;
	//Le texte d'interaction
	private Label interactLabel;

	// Déplacement du joueur.
	private Vector2 _screenSize; // Size of the game window.

	public int Orbs = 0; 

	public Upgrade piercingUpgrade;
	
	public Upgrade speedUpgrade;
	
	public Upgrade fireRateUpgrade;
	
	public Upgrade enduranceUpgrade;

	private double initialLightWaitTime;

	private AudioStreamPlayer2D bulletSound;
	
	[Export]
	public bool in_menu { get; set; } = false;
	
	[Signal]
	public delegate void PlayerDeathEventHandler(Player player);
	
	[Signal]
	public delegate void NombreObresChangedEventHandler(int nombreOrbes);
	
	[Signal]
	public delegate void FireRateUpgradedEventHandler(int fireRateLevel, int fireRateLevelMax);
	
	[Signal]
	public delegate void SpeedUpgradedEventHandler(int speedLevel, int speedLevelMax);
	
	[Signal]
	public delegate void PiercingUpgradedEventHandler(int piercingLevel, int piercingLevelMax);
	
	[Signal]
	public delegate void EnduranceUpgradedEventHandler(int enduranceLevel, int enduranceLevelMax);
	

	public override void _Ready()
	{
		bulletSound = GetNode<AudioStreamPlayer2D>("BulletSound");
		piercingUpgrade = new Upgrade(5, 0, 3);
		speedUpgrade = new Upgrade(1, 0, 5);
		fireRateUpgrade = new Upgrade(2, 0, 5);
		enduranceUpgrade = new Upgrade(1, 0, 10);
		
		// Récupérer les éléments de la scene
		_screenSize = GetViewportRect().Size;
		playerLight = GetNode<PointLight2D>("PlayerLight");
		lightTimer = GetNode<Timer>("LightTimer");
		initialLightWaitTime = lightTimer.WaitTime;
		playerSprite = GetNode<AnimatedSprite2D>("PlayerSprite");
		labelContainer = GetNode<HBoxContainer>("MarginContainer/Interact");
		interactLabel = labelContainer.GetNode<Label>("Label");

		
		// Initialiser le champ de vision du joueur.
		vitality = 1; // 1 pour 100% 
		playerLight.TextureScale = vitality;
	
		// Paramétrage de la camera
		camera = GetNode<Camera2D>("Camera2D");
		camera.Enabled = true;
		camera.MakeCurrent();
		camera.Align();
		camera.PositionSmoothingEnabled = true;
		camera.PositionSmoothingSpeed = 10;

		// Connecter le signal "timeout" du timer à la méthode "OnLightTimerTimeout"
		lightTimer.Timeout += OnLightTimerTimeout;
		
		playerSprite.AnimationLooped += OnPlayerSpriteAnimationLooped;
		
		deathTimer = new Timer();
		deathTimer.OneShot = true;
		deathTimer.Autostart = false;
		AddChild(deathTimer);
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!in_menu)
		{
			Vector2 direction =  Input.GetVector("Gauche", "Droite", "Haut", "Bas");
			Velocity = direction * (Speed + speedUpgrade.GetLevel() * 10);
			MoveAndSlide();
		}
	}

	public override void _Process(double delta)
	{
		var reelShootingCooldown = shootingCooldown - fireRateUpgrade.GetLevel() * 0.03;
		// Vérifiez si le joueur peut tirer en fonction du délai entre les tirs
		if (Input.IsActionJustPressed("Attaque") && shootingTimer >= reelShootingCooldown && !in_menu)
		{
			Shoot();
			shootingTimer = 0;	 // Réinitialisez le compteur de temps après avoir tiré
		}

		if(Input.IsActionJustPressed("Interaction") && interactables.Count != 0)
		{
			IPlayerInteractable interactable = interactables[0];
			interactable.Interact(this);
		}
		
		if (shootingTimer < shootingCooldown)
		{
			shootingTimer += delta;
		}
	}
	
	private void Shoot()
	{
		bulletSound.Play();
		
		var projectile = ProjectileScene.Instantiate<Bullet>();
		GetParent().AddChild(projectile); // Ajoute le projectile à la scène
		projectile.Position = Position; // Démarre le projectile à la position du joueur

		// Calcule la direction de la souris par rapport au joueur
		var mousePosition = GetGlobalMousePosition();
		var directionToMouse = (mousePosition - Position).Normalized();

		projectile.Direction = directionToMouse; // Direction basée sur la position de la souris
		playerSprite.Play("attack_player");
		
	}
	
	public void Die()
	{
		lightTimer.Autostart = false;
		lightTimer.Stop();
		vitality = 0;
		playerLight.TextureScale = 1;
		piercingUpgrade.Reset();
		speedUpgrade.Reset();
		fireRateUpgrade.Reset();
		enduranceUpgrade.Reset();
		
		EmitSignal(nameof(PlayerDeath), this); // Émettre le signal
		StartEndGameAnimation();
	}

	public void UpgradePiercing()
	{
		if (!piercingUpgrade.CanUpgrade(Orbs)) return;
		piercingUpgrade.Up();
		Orbs -= piercingUpgrade.GetPrice();
		EmitSignal(nameof(NombreObresChanged), Orbs);
		EmitSignal(nameof(PiercingUpgraded), piercingUpgrade.GetLevel(), piercingUpgrade.GetMaxLevel());
	}
	
	public void UpgradeSpeed()
	{
		if (!speedUpgrade.CanUpgrade(Orbs)) return;
		speedUpgrade.Up();
		Orbs -= speedUpgrade.GetPrice();
		EmitSignal(nameof(NombreObresChanged), Orbs);
		EmitSignal(nameof(SpeedUpgraded), speedUpgrade.GetLevel(), speedUpgrade.GetMaxLevel());
	}
	public void UpgradeFireRate()
	{
		if (!fireRateUpgrade.CanUpgrade(Orbs)) return;
		fireRateUpgrade.Up();
		Orbs -= fireRateUpgrade.GetPrice();
		EmitSignal(nameof(NombreObresChanged), Orbs);
		EmitSignal(nameof(FireRateUpgraded), fireRateUpgrade.GetLevel(), fireRateUpgrade.GetMaxLevel());
	}
	
	public void UpgradeEndurance()
	{
		if (!enduranceUpgrade.CanUpgrade(Orbs)) return;
		enduranceUpgrade.Up();
		lightTimer.WaitTime = initialLightWaitTime + (initialLightWaitTime / 2) * enduranceUpgrade.GetLevel();
		Orbs -= enduranceUpgrade.GetPrice();
		EmitSignal(nameof(NombreObresChanged), Orbs);
		EmitSignal(nameof(EnduranceUpgraded), enduranceUpgrade.GetLevel(), enduranceUpgrade.GetMaxLevel());
	}
    
	
	public void AddInteractable(IPlayerInteractable interactable)
	{
		if(interactables.Contains(interactable))
		{
			return;
		}

		if(interactables.Count == 0)
		{
			labelContainer.Visible = true;
			interactLabel.Text = interactable.InteractionName;
		}

		interactables.Add(interactable);
	}
	
	public void RemoveInteractable(IPlayerInteractable interactable)
	{
		if(!interactables.Contains(interactable))
		{
			return;
		}

		if(interactables.Count == 1)
		{
			labelContainer.Visible = false;
			interactLabel.Text = "";
		}

		interactables.Remove(interactable);
	}

	public void OnLightTimerTimeout()
	{
		// Diminuer progressivement l'échelle de la lumière du joueur lorsque le timer expire.
		vitality -= 0.01f; // Réduire l'échelle de 10%

		// Arrêter le timer lorsque la lumière est épuisée.
		if (vitality <= 0)
		{
			Die();
		}
		else
		{
			playerLight.TextureScale = vitality;
		}
	}
	
	public void OnPlayerSpriteAnimationLooped()
	{
		if (playerSprite.Animation == "attack_player")
		{
			playerSprite.Play("walk_player");
		}
	}
	
	public void OnOrbPickedUp(float orbVitality)
	{
		// Augmenter la vitalité du joueur sans dépasser la vitalité maximale
		vitality = Mathf.Min(vitality + orbVitality, 1);
		playerLight.TextureScale = vitality;
		Orbs++;
		EmitSignal(nameof(NombreObresChanged), Orbs);
	}
	
	public void OnMobContact(float damage)
	{
		// Augmenter la vitalité du joueur sans dépasser la vitalité maximale
		vitality = Mathf.Max(vitality - damage, 0);
		if (vitality <= 0)
		{
			Die();
			return;
		}
		playerLight.TextureScale = vitality;
	}

	public void StartEndGameAnimation()
	{
		in_menu = true;
		playerLight.TextureScale = 0.5f;
		CollisionLayer = 0;
		CollisionMask = 0;
		var t = GetTree().CreateTween();
		t.TweenProperty(camera, "zoom", new Vector2(10, 10), 1);

		deathTimer.WaitTime = 1.3f;
		deathTimer.Start();
		
		deathTimer.Timeout += () =>
		{
			playerSprite.Play("death_player");
			playerSprite.AnimationLooped += () =>
			{
				Hide();
				GetTree().ChangeSceneToFile("res://Scenes/replay_menu.tscn");
			};
		};
	}
}
