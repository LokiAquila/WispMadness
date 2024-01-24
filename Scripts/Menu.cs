using Godot;
using System;

public partial class Menu : Control
{

	private void _on_play_pressed()
	{
		SceneSwitcher.Instance.SwitchScene("res://Scenes/level.tscn");
	}

	private void _on_options_pressed()
	{
		GetTree().ChangeSceneToFile("res://Scenes/options_container.tscn");
	}

	private void _on_quit_pressed()
	{
		GetTree().Quit();
	}

}
