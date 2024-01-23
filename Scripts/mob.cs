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

    private void OnBodyEntered(Node2D body)
    {
        
        if (body == player)
        {
            EmitSignal(nameof(MobContactPlayer), this); // Émettre le signal
            QueueFree();
        }
    }

    private void OnAreaEntered(Node2D area)
    {
        if (area is Bullet)
        {
            QueueFree();
        }
    }
}