using Godot;
using System;

public partial class Orb : Area2D
{
    [Signal]
    public delegate void OrbPickedUpEventHandler(Orb orb, float vitality);

    // Duration before the orb disappears
    [Export]
    public float ExpirationDuration = 1f;
    // Initial scale of the orb
    [Export]
    public Vector2 InitialScale = new Vector2(1, 1);

    // The amount of vitality the orb starts with
    [Export]
    public float InitialVitality = 0.3f;

    // The current vitality of the orb
    private float currentVitality;
    
    protected AnimatedSprite2D orbeSprites;

    public override void _Ready()
    {
        orbeSprites = GetNode<AnimatedSprite2D>("OrbeSprites");
        currentVitality = InitialVitality;
        Scale = InitialScale;

        BodyEntered += OnAreaEntered;

        // Configure the Timer
        Timer orbTimer = GetNode<Timer>("OrbTimer");
        orbTimer.WaitTime = ExpirationDuration; // Check every 0.1 seconds
        orbTimer.OneShot = false; // The timer runs repeatedly
        orbTimer.Timeout += OnTimerTimeout;
        orbTimer.Start();
        orbeSprites.Play("idle");
    }

    private void OnAreaEntered(Node2D body)
    {
        GD.Print("Collision");
        if (body.Name == "Player")
        {
            EmitSignal(nameof(OrbPickedUp), this, currentVitality);
            QueueFree();
        }
    }

    private void OnTimerTimeout()
    {
        // Decrease the orb's lifetime and vitality
        currentVitality -= InitialVitality * 0.05f;

        // Scale the orb down to visualize its reduced vitality
        // The scale is a proportion of the current vitality to the initial vitality
        Scale = InitialScale * (currentVitality / InitialVitality);

        // Remove the orb when the lifetime expires
        if (currentVitality <= 0)
        {
            QueueFree();
        }
    }

}