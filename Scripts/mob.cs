using Godot;
using System;

public partial class Mob : RigidBody2D
{
    [Export]
    private float speed = 100.0f;
    private Player player;

    public override void _Ready()
    {
        player = GetNode<Player>("../Player");
        SetRandomPosition();
        Connect("body_entered", new Callable(this, "_OnBodyEntered"));

    }

    public override void _Process(double delta)
    {
        if (player != null)
        {
            Vector2 direction = (player.GlobalPosition - GlobalPosition).Normalized();
            GlobalPosition += direction * speed * (float)delta;
        }
    }


    private void SetRandomPosition()
    {
        // Récupérer la taille de l'écran
        Vector2 screenSize = GetViewportRect().Size;

        // Choisir une position aléatoire en dehors de l'écran
        GlobalPosition = new Vector2(
            GD.Randf() * screenSize.X * 2 - screenSize.X, // Utilisez GD.Randf() au lieu de Mathf.Randf()
            GD.Randf() * screenSize.Y * 2 - screenSize.Y // Utilisez GD.Randf() au lieu de Mathf.Randf()
        );
    }

    private void _OnBodyEntered(Node body)
    {
        if (body is CharacterBody2D) // Vérifiez si le joueur est en collision
        {
            // Démarrez l'animation de mort
            AnimationPlayer animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            animationPlayer.Play("Death");

            // Désactivez le RigidBody2D pour arrêter le déplacement du Mob
            SetProcess(false);

            // Obtenez la durée de l'animation de mort
            float deathAnimationLength = animationPlayer.GetAnimation("Death").Length;

            // Supprimez le Mob du jeu après la durée de l'animation de mort
            CallDeferred("_RemoveMob", deathAnimationLength);
        }
    }


    private void _RemoveMob(float delay)
    {
        CallDeferred("_DoRemoveMob", delay);
    }

    private void _DoRemoveMob(float delay)
    {
        QueueFree();
    }

}

