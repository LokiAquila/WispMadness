using Godot;
using System;

public partial class Mob : Area2D
{
    [Export]
    private float speed = 100.0f;
    private Player player;
    private AnimatedSprite2D mobSprites;

    [Signal]
    public delegate void MobContactPlayerEventHandler(Mob mob);
    
    [Signal]
    public delegate void OrbDroppedEventHandler(Vector2 position);

    public override void _Ready()
    {
        mobSprites = GetNode<AnimatedSprite2D>("MobSprites");
        player = GetNode<Player>("../Player");
        
        // Connecter le signal AreaEntered à la méthode OnAreaEntered
        BodyEntered += OnBodyEntered;
        AreaEntered += OnAreaEntered;
        
        mobSprites.Play("walk_mob");
    }

    public override void _Process(double delta)
    {
        if (player != null)
        {
            Vector2 direction = (player.GlobalPosition - GlobalPosition).Normalized();
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
            EmitSignal(nameof(MobContactPlayer), this); // Émettre le signal
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
            EmitSignal(nameof(OrbDropped), Position);
        }
        QueueFree();
    }
}