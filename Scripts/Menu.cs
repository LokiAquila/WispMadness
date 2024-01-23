using Godot;
using System;

public partial class Menu : Control
{
	private void _on_play_pressed()
	{
		GetTree().ChangeSceneToFile("res://Scenes/level.tscn");
	}
	private void _on_options_pressed()
	{
		GetTree().ChangeSceneToFile("res://Scenes/options.tscn");
	}
	private void _on_quit_pressed()
	{
		GetTree().Quit();
	}
}
