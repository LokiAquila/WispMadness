using Godot;
using System;

public partial class Reaper : Mob // Héritage de la classe Mob
{
    [Export]
    private int maxHealth = 25;
    private int currentHealth;
    [Export]
    private double attackInterval = 1; // Intervalle entre les attaques en secondes
    private double timeSinceLastAttack = 0;
    
    private bool isPlayerInContact = false;

    private PointLight2D mobLight;
    
    [Signal]
    public delegate void BossKilledEventHandler();
    
    public override void _Ready()
    {
        mobSprites = GetNode<AnimatedSprite2D>("MobSprites");
        player = GetNode<Player>("../Player");
        
        mobLight = GetNode<PointLight2D>("MobLight");
        
        currentHealth = maxHealth;
        Damage = 0.2f;
        
        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;
        
        AreaEntered += OnAreaEntered;
        mobSprites.AnimationLooped += OnReaperAnimationLooped;
        mobSprites.Play("moove");
    }

    public override void _Process(double delta)
    {
        base._Process(delta); // Appel de la méthode _Process de la classe parente
        
        
        // Gérer l'intervalle d'attaque
        if (timeSinceLastAttack > 0)
        {
            timeSinceLastAttack -= delta;
        }
        else if(isPlayerInContact)
        {
            AttackPlayer();
        }
    }

    private new void OnBodyEntered(Node2D body)
    {
        if (body is Player)
        {
            isPlayerInContact = true;
            if (timeSinceLastAttack <= 0)
            {
                // Effectuer l'attaque
                AttackPlayer();
            }
        }
    }
    
    private void OnBodyExited(Node2D body)
    {
        if (body is Player)
        {
            isPlayerInContact = false; // Marquer le Player comme n'étant plus en contact
        }
    }

    private new void OnAreaEntered(Node2D area)
    {
        if (area is Bullet)
        {
            var t = GetTree().CreateTween();
            t.TweenProperty(mobLight, "color", new Color(1,1,1,1), 0.1);
            t.TweenProperty(mobLight, "color", new Color(1,0,0,1), 0.1);
            
            // Gérer la réduction des points de vie
            currentHealth -= 1;
            if (currentHealth == 10)
            {
                Modulate = new Color(1, 0, 0);
                mobLight.TextureScale = 0.75f;
                speed = speed * 1.2f;
            }
            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }
    
    public void OnReaperAnimationLooped()
    {
        if (mobSprites.Animation == "attack")
        {
            mobSprites.Play("moove");
        }
    }

    private void AttackPlayer()
    {
        mobSprites.Play("attack");
        timeSinceLastAttack = attackInterval;
        EmitSignal(nameof(MobContactPlayer), this, Damage); // Émettre le signal
    }

    protected new void Die()
    {
        EmitSignal(nameof(OrbDropped), Position);
        EmitSignal(nameof(BossKilled));
        QueueFree();
    }
}