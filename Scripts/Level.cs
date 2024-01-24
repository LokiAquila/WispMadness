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
	
	private Label nombreOrbesLabel;

	private double Score = 0;
	private Label ScoreLabel;
	private bool stopScore { get; set; } = false; //Mettre à true pour stopper le score
	
	
	private Label levelFireRateLabel;
	private Label levelSpeedLabel;
	private Label levelPiercingLabel;
	private Label levelEnduranceLabel;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player = GetNode<Player>("Player");
		player.PlayerDeath += EndGame;
		player.NombreObresChanged += _on_player_nombre_obres_changed;
		player.FireRateUpgraded += _on_player_fire_rate_upgraded;
		player.SpeedUpgraded += _on_player_speed_upgraded;
		player.PiercingUpgraded += _on_player_piercing_upgraded;
		player.EnduranceUpgraded += _on_player_endurance_upgraded;
		
		
		// Spawn first Orb TEST
		var orb = OrbScene.Instantiate<Orb>();
		orb.Position = new Vector2(30, 30);
		orb.OrbPickedUp += OnOrbPickedUp;
		AddChild(orb);
		
		ScoreLabel = GetNode<Label>("UI/Score");
		ScoreLabel.Text = "Score : " + Score;
		
		nombreOrbesLabel = GetNode<Label>("UI/orbeList/nombreOrbe");
		nombreOrbesLabel.Text = player.Orbs.ToString();
		
		levelFireRateLabel = GetNode<Label>("UI/augments/augment1/augment1T");
		levelSpeedLabel = GetNode<Label>("UI/augments/augment2/augment2T");
		levelPiercingLabel = GetNode<Label>("UI/augments/augment3/augment3T");
		levelEnduranceLabel = GetNode<Label>("UI/augments/augment4/augment4T");
		levelFireRateLabel.Text = "0";
		levelSpeedLabel.Text = "0";
		levelPiercingLabel.Text = "0";
		levelEnduranceLabel.Text = "0";
		
		
		
		
		StartGame();
	}
	

	// Called every frame. 'delta' is the elapsed time since the previous frame.q
	public override void _Process(double delta)
	{
		if (!stopScore)
		{
			Score += delta;
			ScoreLabel.Text = "Score : " + (int)Score;
		}
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
	
	private void _on_player_nombre_obres_changed(int nombreOrbes)
	{
		GD.Print("a");
		nombreOrbesLabel.Text = nombreOrbes.ToString();
	}
	
	private void _on_player_fire_rate_upgraded(int fireRateLevel)
	{
		levelFireRateLabel.Text = fireRateLevel.ToString();
	}
	
	private void _on_player_speed_upgraded(int speedLevel)
	{
		levelSpeedLabel.Text = speedLevel.ToString();
	}
	
	private void _on_player_piercing_upgraded(int piercingLevel)
	{
		levelPiercingLabel.Text = piercingLevel.ToString();
	}
	
	private void _on_player_endurance_upgraded(int enduranceLevel)
	{
		levelEnduranceLabel.Text = enduranceLevel.ToString();
	}
}
