using Godot;

public partial class GameManager : Node
{

    public static GameManager Instance { get; private set; }
    
    public PauseMenu pauseMenu;

    private bool pause;

    public override void _Ready()
    {
        if(Instance != null)
        {
            QueueFree();
            return;
        }

        Instance = this;
        pauseMenu = GetParent().GetNode<PauseMenu>("UI");
    }

    public override void _Process(double delta)
    {
        if(Input.IsActionJustPressed("Pause"))
        {
            SetPause(!pause);
        }

    }

    public void SetPause(bool pause)
    {
        GetTree().Paused = false;
        pauseMenu.SetOpened(pause);
    }

}