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
		
		// Spawn first Orb TEST
		var orb = OrbScene.Instantiate<Orb>();
		orb.Position = new Vector2(30, 30);
		orb.OrbPickedUp += OnOrbPickedUp;
		AddChild(orb);

		StartGame();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.q
	public override void _Process(double delta)
	{
	}
	
	private void StartGame()
	{
		//Ajout et Configuration du Timer pour géré le Spawn des Mobs
		mobInterval = new Timer();
		mobInterval.WaitTime = 0.65;
		mobInterval.Autostart = true;
		mobInterval.Timeout += SpawnMob;
		AddChild(mobInterval);
		
		// Spawn le premier mob
		SpawnMob();
	}

	private void SpawnMob()
	{
		var mob = MobScene.Instantiate<Mob>();
		mob.Position = mob.GetRandomPositionOnCameraEdge(player.camera.GlobalPosition);
		mob.MobContactPlayer += OnMobContactPlayer;
		mob.OrbDropped += OnOrbDropped;
		AddChild(mob);
	}

	private void EndGame(Player player)
	{
		// TODO afficher le game over ici
	}
	
	private void OnOrbDropped(Vector2 position)
	{
		var orb = OrbScene.Instantiate<Orb>();
		orb.Position = position;
		orb.OrbPickedUp += OnOrbPickedUp;
		AddChild(orb);
	}
	
	private void OnOrbPickedUp(Orb orb, float currentVitality)
	{
		player.OnOrbPickedUp(orb, currentVitality);
	}
	
	private void OnMobContactPlayer(Mob mob)
	{
		player.OnMobContact();
	}
}
