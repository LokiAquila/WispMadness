using Godot;
using System;

public partial class Mob : Area2D
{
    [Export]
    private float speed = 100.0f;
    private Player player;
    private AnimatedSprite2D mobSprites;
    private CollisionShape2D mobCollision;

    [Signal]
    public delegate void MobContactPlayerEventHandler(Mob mob);
    
    [Signal]
    public delegate void OrbDroppedEventHandler(Vector2 position);

    public override void _Ready()
    {
        mobSprites = GetNode<AnimatedSprite2D>("MobSprites");
        mobCollision = GetNode<CollisionShape2D>("MobCollision");
        player = GetNode<Player>("../Player");
        player.PlayerDeath += OnPlayerDeath;
        
        // Connecter le signal AreaEntered à la méthode OnAreaEntered
        BodyEntered += OnBodyEntered;
        AreaEntered += OnAreaEntered;
        
        mobSprites.Play("walk_mob");
    }

    public override void _Process(double delta)
    {
        if (player.vitality > 0)
        {
            Vector2 direction = (player.GlobalPosition - GlobalPosition).Normalized();
            GlobalPosition += direction * speed * (float)delta;
        }
        else
        {
            Vector2 direction = (player.GlobalPosition - GlobalPosition).Normalized();
            // Inversez la direction en multipliant par -1
            direction *= -1;
            GlobalPosition += direction * speed * (float)delta;
        }
    }
    
    public Vector2 GetRandomPositionOnCameraEdge(Vector2 globalPosition)
    {
        // Générer un angle aléatoire entre 0 et 2*PI
        float angle = (float)GD.Randf() * 2.0f * Mathf.Pi;
    
        // Générer une distance aléatoire entre 0 et le rayon spécifié
        float distance = 1000;

        // Calculer les coordonnées x et y en utilisant l'angle et la distance
        float x = distance * Mathf.Cos(angle);
        float y = distance * Mathf.Sin(angle);

        // Ajouter le déplacement à la position actuelle de la caméra pour obtenir le point aléatoire
        Vector2 randomPoint = globalPosition + new Vector2(x, y);

        return randomPoint;

    }

    private void OnBodyEntered(Node2D body)
    {
        
        if (body == player)
        {
            CallDeferred("emit_signal", nameof(MobContactPlayer), this);
            Die();
        }
    }

    private void OnAreaEntered(Node2D area)
    {
        if (area is Bullet)
        {
            Die();
        }
    }

    protected void Die()
    {
        var chance = GD.Randi() % 10 + 1;
        GD.Print(chance);
        if (chance == 10)
        {
            CallDeferred("emit_signal", nameof(OrbDropped), Position);
        }
        
        mobSprites.Play("death_mob");
        
        CollisionLayer = 0;
        CollisionMask = 0; 
        
        mobSprites.AnimationFinished += QueueFree;
    }

    private void OnPlayerDeath(Player player)
    {
        var timer = new Timer();
        timer.OneShot = true;
        timer.WaitTime = 3;
        timer.Timeout += QueueFree;
        timer.Start();
    }
}
