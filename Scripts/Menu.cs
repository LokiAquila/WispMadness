using Godot;
using System;

public partial class Menu : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	private void _on_PlayPressed()
	{
		GetTree().Root.AddChild(level);
	}

	// Appelé lorsque le bouton "Options" est pressé
	private void _on_OptionsPressed()
	{
		GetTree().ChangeScene("res://Scenes/options.tscn");
	}

	// Appelé lorsque le bouton "Quit" est pressé
	private void _on_QuitPressed()
	{
		GetTree().Quit();
	}
}
