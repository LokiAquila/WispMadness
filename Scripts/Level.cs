using Godot;
using System;

public partial class Level : Node2D
{
	private Player player;
	
	[Export]
	public PackedScene OrbScene { get; set; }
	
	[Export]
	public PackedScene MobScene { get; set; }
	
	[Export]
	public PackedScene ReaperScene{ get; set; }

	private Timer mobInterval;
	private Timer peaceTimer;

	
	private Label nombreOrbesLabel;

	private double Score = 0;
	private Label ScoreLabel;
	private bool stopScore { get; set; } = false; //Mettre à true pour stopper le score
	
	
	private Label levelFireRateLabel;
	private Label levelSpeedLabel;
	private Label levelPiercingLabel;
	private Label levelEnduranceLabel;

	private int entitiesSpawned = 0;
	private int nbActiveBoss = 0;
	
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
		
		
		peaceTimer = new Timer();
		peaceTimer.WaitTime = 15;
		peaceTimer.Autostart = false;
		peaceTimer.Timeout += OnPeaceEnd;
		AddChild(peaceTimer);
		
		
		// Spawn first Orb TEST
		var orb = OrbScene.Instantiate<Orb>();
		orb.Position = new Vector2(30, 30);
		orb.OrbPickedUp += OnOrbPickedUp;
		AddChild(orb);
		
		
		ScoreLabel = GetNode<Label>("UI/Score");
		ScoreLabel.Text = "Score : " + (int)Score;
		
		nombreOrbesLabel = GetNode<Label>("UI/orbeList/nombreOrbe");
		nombreOrbesLabel.Text = player.Orbs.ToString();
		
		levelFireRateLabel = GetNode<Label>("UI/augments/augment1/augment1T");
		levelSpeedLabel = GetNode<Label>("UI/augments/augment2/augment2T");
		levelPiercingLabel = GetNode<Label>("UI/augments/augment3/augment3T");
		levelEnduranceLabel = GetNode<Label>("UI/augments/augment4/augment4T");
		levelFireRateLabel.Text = "0/" + player.fireRateUpgrade.GetMaxLevel();
		levelSpeedLabel.Text = "0/" + player.speedUpgrade.GetMaxLevel();
		levelPiercingLabel.Text = "0/" + player.piercingUpgrade.GetMaxLevel();
		levelEnduranceLabel.Text = "0/" + player.enduranceUpgrade.GetMaxLevel();
		
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
		mobInterval.WaitTime = 0.7;
		mobInterval.Autostart = true;
		mobInterval.Timeout += CalculateWave;
		AddChild(mobInterval);
		// Spawn le premier mob
		CalculateWave();
	}

	private void CalculateWave()
	{
		if (nbActiveBoss > 0) return;
		
		if ((entitiesSpawned + 1) % 30 == 0)
		{
			
			var nbBoss = (entitiesSpawned + 1) / 120 + 1;
			GD.Print("Boss spawn ! " + nbBoss);
			for (int i = 0; i < nbBoss; i++)
			{
				SpawnBoss();
			}
		}
		else
		{
			SpawnMob();
		}
	}
	
	private void SpawnMob()
	{
		entitiesSpawned++;
		var mob = MobScene.Instantiate<Mob>();
		mob.Position = mob.GetRandomPositionOnCameraEdge(player.camera.GlobalPosition);
		mob.MobContactPlayer += OnMobContactPlayer;
		mob.OrbDropped += OnOrbDropped;
		AddChild(mob);
	}
	
	private void SpawnBoss()
	{
		entitiesSpawned++;
		nbActiveBoss++;
		var reaper = ReaperScene.Instantiate<Reaper>();
		reaper.Position = reaper.GetRandomPositionOnCameraEdge(player.camera.GlobalPosition);
		reaper.MobContactPlayer += OnMobContactPlayer;
		reaper.OrbDropped += OnOrbDropped;
		reaper.BossKilled += OnBossKilled;
		AddChild(reaper);
	}

	private void EndGame(Player player)
	{
		mobInterval.Autostart = false;
		mobInterval.Stop();
	}
	
	private void startPeace()
	{
		peaceTimer.Start();
		stopScore = true;
		mobInterval.Paused = true;
		player.lightTimer.Paused = true;
		ScoreLabel.Text = "Score : " + (int)Score + " (Freeze : Peace Time)";
	}

	private void OnPeaceEnd()
	{
		peaceTimer.Stop();
		stopScore = false;
		mobInterval.Paused = false;
		player.lightTimer.Paused = false;
	}

	private void OnBossKilled()
	{
		nbActiveBoss--;
		if (nbActiveBoss == 0)
		{
			startPeace();
		}
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
	
	private void OnMobContactPlayer(Mob mob, float damage)
	{
		player.OnMobContact(damage);
	}
	
	private void _on_player_nombre_obres_changed(int nombreOrbes)
	{
		nombreOrbesLabel.Text = nombreOrbes.ToString();
	}
	
	private void _on_player_fire_rate_upgraded(int level, int maxLevel)
	{
		levelFireRateLabel.Text = level + "/" + maxLevel;
	}
	
	private void _on_player_speed_upgraded(int level, int maxLevel)
	{
		levelSpeedLabel.Text = level + "/" + maxLevel;
	}
	
	private void _on_player_piercing_upgraded(int level, int maxLevel)
	{
		levelPiercingLabel.Text = level + "/" + maxLevel;
	}
	
	private void _on_player_endurance_upgraded(int level, int maxLevel)
	{
		levelEnduranceLabel.Text = level + "/" + maxLevel;
	}
}
