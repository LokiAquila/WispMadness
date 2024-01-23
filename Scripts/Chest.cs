using Godot;
using System;

public partial class Chest : StaticBody2D, IPlayerInteractable
{

	private AnimatedSprite2D sprite;
	private Area2D interactZone;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		interactZone = GetNode<Area2D>("Interact Zone");
		sprite = GetNode<AnimatedSprite2D>("Sprite");

		interactZone.BodyEntered += OnPlayerEnter;
		interactZone.BodyExited += OnPlayerExit;
		sprite.AnimationFinished += OnOpeningFinished;
	}

	private void OnPlayerEnter(Node2D body)
	{
		if(!body.IsInGroup("Player"))
		{
			return;
		}

		Player player = (Player) body;
		player.AddInteractable(this);
	}
	
	private void OnPlayerExit(Node2D body)
	{
		if(!body.IsInGroup("Player"))
		{
			return;
		}

		Player player = (Player) body;
		player.RemoveInteractable(this);
	}

	public void Interact(Player player)
	{
		player.RemoveInteractable(this);
		interactZone.QueueFree();

		sprite.Play("Opening");
	}
	
	public void OnOpeningFinished()
	{
		if(sprite.Animation != "Opening")
		{
			return;
		}

		GD.Print("You opened a chest!");
	}

	public string InteractionName
	{
		get => "Open Chest";
	}
	
}
