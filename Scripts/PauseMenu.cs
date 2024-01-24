using Godot;
using Godot.Collections;

public partial class PauseMenu : CanvasLayer
{
	
	private ColorRect pause;
	private VBoxContainer pauseMenu;
	private Control options;

	private bool optionOpened;

	public override void _Ready()
    {
	    pauseMenu = GetNode<VBoxContainer>("pause/PauseMenu");
        options = GetNode<Control>("MenuOptions");
        pause = GetNode<ColorRect>("pause");

        Button backButton = options.GetNode<Button>("Panel/Content/List/Button/Back");
		backButton.Pressed += OnBackButtonPressed;
    }
	

	private void _on_resume_pressed()
	{
		GetTree().Paused = false;
		pause.Visible = false;
	}

	private void _on_options_pressed()
	{
		optionOpened = true;
		pauseMenu.Visible = false;
		options.Visible = true;
	}

	private void _on_quit_pressed()
	{
		GetTree().Paused = false;
		pause.Visible = false;
		SceneSwitcher.Instance.SwitchScene("res://Scenes/menu.tscn");
	}
	
	private void OnBackButtonPressed()
	{
		optionOpened = false;
		pauseMenu.Visible = true;
		options.Visible = false;
	}

}
