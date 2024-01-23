using Godot;
using System;

public partial class Statue : Area2D, IPlayerInteractable
{
	
	private ItemList menu;
	private Control control;
	
	private Player joueur; //Atribué lors de l'interaction
	private bool in_menu { get; set; } = false;
	public void Interact(Player player)
	{
		player.RemoveInteractable(this);
		control.Visible = true;
		player.in_menu = true;
		in_menu = true;
		joueur = player;
	}

	public string InteractionName
	{
		get => "Buy Upgrade";
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		menu = GetNode<ItemList>("Control/VBoxContainer/Menu");
		control = GetNode<Control>("Control");
		BodyEntered += _on_body_entered;
		BodyExited += _on_body_exited;
		menu.ItemActivated += _on_menu_item_activated;
		control.Visible = false;
	}
	
	public void quitterMenu()
	{
		control.Visible = false;
		joueur.in_menu = false;
		in_menu = false;
		joueur.AddInteractable(this);
	}
	
	public override void _Process(double delta)
	{
		if(Input.IsActionPressed("Quitter") && in_menu)
		{
			quitterMenu();
		}

		if (in_menu && Input.IsMouseButtonPressed(MouseButton.Left))
		{
			if (!control.GetRect().HasPoint(GetLocalMousePosition()))
			{
				quitterMenu();
			}
		}
	}
	
	public void _on_body_entered(Node body)
	{
		if(!body.IsInGroup("Player"))
		{
			return;
		}

		Player player = (Player) body;
		player.AddInteractable(this);
	}
	
	public void _on_body_exited(Node body)
	{
		if(!body.IsInGroup("Player"))
		{
			return;
		}

		Player player = (Player) body;
		player.RemoveInteractable(this);	
	}
	
	public void _on_menu_item_activated(long index)
	{
		if (index == 0)
		{
			GD.Print("You chose the first item");
			
		}else if (index == 1)
		{
			GD.Print("You chose the second item");
		}else if (index == 2)
		{
			GD.Print("You chose the third item");
		}else if (index == 3)
		{
			GD.Print("You chose the fourth item");
		}else if (index == 4)
		{
			GD.Print("You chose the fifth item");
		}
	}
}
