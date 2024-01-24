using Godot;
using System;

public partial class replay_menu : Control
{
	private VBoxContainer vbox;
	
	public override void _Ready()
	{
		
		Label score = GetNode<Label>("VBoxContainer/Score");
		score.Text = "Score : " + SwitchScene.Score;
		
	}

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

	public void _on_play_again_tree_entered()
	{
		vbox = GetNode<VBoxContainer>("VBoxContainer");
		vbox.Modulate = new Color(1, 1, 1, 0);
		var t = GetTree().CreateTween();
		t.TweenProperty(vbox, "modulate", new Color(1, 1, 1, 1), 0.5f);
	}
	
	
}
