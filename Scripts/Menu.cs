using Godot;

public partial class Menu : Control
{

	private Control playGame;
	private Control options;

    public override void _Ready()
    {
        playGame = GetNode<Control>("PlayGame");
        options = GetNode<Control>("MenuOptions");

        Button backButton = options.GetNode<Button>("Panel/Content/List/Button/Back");
		backButton.Pressed += _on_back_button_pressed;
    }

    private void _on_play_pressed()
	{
		SceneSwitcher.Instance.SwitchScene("res://Scenes/level.tscn");
	}

	private void _on_options_pressed()
	{
		playGame.Visible = false;
		options.Visible = true;
	}
	
	private void _on_back_button_pressed()
	{
		playGame.Visible = true;
		options.Visible = false;
	}

	private void _on_quit_pressed()
	{
		GetTree().Quit(0);
	}

}
