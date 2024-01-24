using Godot;

public partial class PauseMenu : CanvasLayer
{

	private ColorRect panel;
	private VBoxContainer pauseMenu;
	private Control options;

	private bool optionOpened;

	public override void _Ready()
    {
        panel = GetNode<ColorRect>("Panel");
        pauseMenu = panel.GetNode<VBoxContainer>("PauseMenu");
        options = GetNode<Control>("MenuOptions");

        Button backButton = options.GetNode<Button>("Panel/Content/List/Button/Back");
		backButton.Pressed += OnBackButtonPressed;
    }

	public void SetOpened(bool opened)
	{
		panel.Visible = opened;

		if(!opened && optionOpened)
		{
			OnBackButtonPressed();
		}

	}
	
	private void OnResumePressed()
	{
		GameManager.Instance.SetPause(false);
	}

	private void OnOptionsPressed()
	{
		optionOpened = true;
		pauseMenu.Visible = false;
		options.Visible = true;
	}
	
	private void OnBackButtonPressed()
	{
		optionOpened = false;
		pauseMenu.Visible = true;
		options.Visible = false;
	}

}
