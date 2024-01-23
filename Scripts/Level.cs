using Godot;
using System;

public partial class Level : Node2D
{

	private Player player;
	
	[Export]
	public PackedScene OrbScene { get; set; }
	
	[Export]
	public PackedScene MobScene { get; set; }

	private Timer mobInterval;
	
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player = GetNode<Player>("Player");
		player.PlayerDeath += EndGame;
		
		// Spawn first Orb
		var orb = OrbScene.Instantiate<Orb>();
		orb.Position = new Vector2(30, 30);
		orb.OrbPickedUp += OnOrbPickedUp;
		AddChild(orb);

		mobInterval = new Timer();
		mobInterval.WaitTime = 1;
		mobInterval.Autostart = true;
		mobInterval.Timeout += SpawnMob;
		AddChild(mobInterval);
		SpawnMob();
		//Spawn first Mob
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.q
	public override void _Process(double delta)
	{
	}
	
	private void StartGame()
	{
		
	}

	private void SpawnMob()
	{
		GD.Print("Mob");
		var mob = MobScene.Instantiate<Mob>();
		mob.Position = GetRandomPositionOnCameraEdge();
		mob.MobContactPlayer += OnMobContactPlayer;
		AddChild(mob);
	}

	private void EndGame(Player player)
	{
		
	}
	
	private Vector2 GetRandomPositionOnCameraEdge()
	{
		var viewportRect = player.camera.GetViewportRect();
		var position = Vector2.Zero;

		// Choisir aléatoirement un côté du périmètre (haut, bas, gauche, droite)
		var side = GD.Randi() % 4;

		switch (side)
		{
			case 0: // Haut
				position.X = (float)GD.RandRange(viewportRect.Position.X, viewportRect.End.X);
				position.Y = viewportRect.Position.Y;
				break;
			case 1: // Bas
				position.X = (float)GD.RandRange(viewportRect.Position.X, viewportRect.End.X);
				position.Y = viewportRect.End.Y;
				break;
			case 2: // Gauche
				position.X = viewportRect.Position.X;
				position.Y = (float)GD.RandRange(viewportRect.Position.Y, viewportRect.End.Y);
				break;
			case 3: // Droite
				position.X = viewportRect.End.X;
				position.Y = (float)GD.RandRange(viewportRect.Position.Y, viewportRect.End.Y);
				break;
		}

		return position;
	}

	
	
	private void OnOrbPickedUp(Orb orb, float currentVitality)
	{
		GD.Print("Orb picked up. Restoring vitality.");

		// Augmenter la vitalité du joueur.
		player.OnOrbPickedUp(orb, currentVitality);
		
	}
	
	private void OnMobContactPlayer(Mob mob)
	{
		GD.Print("Level : mob colisione le player");
		player.OnMobContact();
	}
}
