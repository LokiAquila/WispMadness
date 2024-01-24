using Godot;
using System;

public partial class Statue : Area2D, IPlayerInteractable
{
	private ItemList menu;
	private Control control;
	
	private Player joueur; 
	private bool in_menu { get; set; } = false;
	private bool doubleClique = false;
	public void Interact(Player player)
	{
		player.RemoveInteractable(this);
		control.Visible = true;
		player.in_menu = true;
		in_menu = true;
		joueur = player;
		menu.SetItemText(0, " "+joueur.fireRateUpgrade.GetPrice());
		menu.SetItemText(1, " "+joueur.speedUpgrade.GetPrice());
		menu.SetItemText(2, " "+joueur.piercingUpgrade.GetPrice());
		menu.SetItemText(3, " "+joueur.enduranceUpgrade.GetPrice());
		
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
		if (doubleClique)
		{
			if (index == 0)
			{
				GD.Print("Fire Rate Upgrade");
				joueur.UpgradeFireRate();

			}
			else if (index == 1)
			{
				GD.Print("Speed Upgrade");
				joueur.UpgradeSpeed();

			}
			else if (index == 2)
			{
				GD.Print("Enemie Piercing");
				joueur.UpgradePiercing();

			}
			else if (index == 3)
			{
				GD.Print("Light Endurance");
				joueur.UpgradeEndurance();

			}
			doubleClique = false;
		}
		else
		{
			doubleClique = true;
		}
	}
}
