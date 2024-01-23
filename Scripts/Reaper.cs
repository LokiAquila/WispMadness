using Godot;
using System;

public partial class Reaper : Mob // Héritage de la classe Mob
{
    [Export]
    private int maxHealth = 50;
    private int currentHealth;
    [Export]
    private double attackInterval = 2.0; // Intervalle entre les attaques en secondes
    private double timeSinceLastAttack = 0;

    
    [Signal]
    public delegate void MobContactPlayerEventHandler(Mob mob);
    
    [Signal]
    public delegate void OrbDroppedEventHandler(Vector2 position);
    public override void _Ready()
    {
        base._Ready(); // Appel de la méthode _Ready de la classe parente
        currentHealth = maxHealth;
        BodyEntered += OnBodyEntered;
        AreaEntered += OnAreaEntered;
    }

    public override void _Process(double delta)
    {
        base._Process(delta); // Appel de la méthode _Process de la classe parente
        
        // Gérer l'intervalle d'attaque
        if (timeSinceLastAttack > 0)
        {
            timeSinceLastAttack -= delta;
        }
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body is Player)
        {
            if (timeSinceLastAttack <= 0)
            {
                // Effectuer l'attaque
                AttackPlayer();
                timeSinceLastAttack = attackInterval;
            }
        }
    }

    private void OnAreaEntered(Node2D area)
    {
        if (area is Bullet)
        {
            // Gérer la réduction des points de vie
            currentHealth -= 1;
            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    private void AttackPlayer()
    {
        EmitSignal(nameof(MobContactPlayer), this); // Émettre le signal
    }

    protected new void Die()
    {
        EmitSignal(nameof(OrbDropped), Position);
        QueueFree();
    }
}