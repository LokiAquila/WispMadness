using Godot;
using System;

public partial class Orb : Area2D
{
    [Signal]
    public delegate void OrbPickedUpEventHandler(Orb orb, float vitality);

    // Duration before the orb disappears
    [Export]
    public float Lifetime = 10.0f;

    // Initial scale of the orb
    [Export]
    public Vector2 InitialScale = new Vector2(1, 1);

    // The amount of vitality the orb starts with
    [Export]
    public float InitialVitality = 0.3f;

    // The current vitality of the orb
    private float currentVitality;

    public override void _Ready()
    {
        currentVitality = InitialVitality;
        Scale = InitialScale;

        BodyEntered += OnAreaEntered;

        // Configure the Timer
        Timer orbTimer = GetNode<Timer>("OrbTimer");
        orbTimer.WaitTime = 20f; // Check every 0.1 seconds
        orbTimer.OneShot = false; // The timer runs repeatedly
        orbTimer.Timeout += OnTimerTimeout;
        orbTimer.Start();
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
        Lifetime -= 0.1f;
        currentVitality -= InitialVitality / (Lifetime / 0.1f); // Decrease vitality linearly over time

        // Scale the orb down to visualize its reduced vitality
        float scaleMultiplier = Lifetime / 10.0f;
        Scale = InitialScale * scaleMultiplier;

        // Remove the orb when the lifetime expires
        if (Lifetime <= 0)
        {
            QueueFree();
        }
    }
}