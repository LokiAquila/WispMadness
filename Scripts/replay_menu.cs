using Godot;
using System;

public partial class replay_menu : Control
{
	public void _on_play_again_pressed()
	{
		GD.Print("oui");
		GetTree().ChangeSceneToFile("res://Scenes/level.tscn");
	}
	
	public void _on_options_pressed()
	{
		GetTree().ChangeSceneToFile("res://Scenes/options_container.tscn");
	}
	
	public void _on_quit_pressed()
	{
		GetTree().Quit();
	}
}
