using Godot;

public partial class SceneSwitcher : CanvasLayer
{

	public static SceneSwitcher Instance { get; set; }

	private AnimationPlayer animation;
	private Control panel;

	private Node currentScene;

	private string scenePath;

	private int state;

	private bool cacheMode;
	private bool animating;
	private bool loading;

	public override void _Ready()
	{
		Instance = this;

		panel = GetNode<Control>("Panel");
		animation = panel.GetNode<AnimationPlayer>("Transition");
		animation.AnimationFinished += OnAnimationEnd;

		Window root = GetTree().Root;
		currentScene = root.GetChild(root.GetChildCount() - 1);
	}

	public override void _Process(double delta)
	{
		if(!loading || animating)
		{
			return;
		}

		if(cacheMode)
		{
			EndLoading((PackedScene) ResourceLoader.Load(scenePath, null, ResourceLoader.CacheMode.Reuse));
			return;
		}

		ResourceLoader.ThreadLoadStatus status = ResourceLoader.LoadThreadedGetStatus(scenePath);
		if(animating || status == ResourceLoader.ThreadLoadStatus.InProgress)
		{
			return;
		}

		if(status == ResourceLoader.ThreadLoadStatus.Loaded)
		{
			EndLoading((PackedScene) ResourceLoader.LoadThreadedGet(scenePath));
			return;
		}

		GetTree().Quit(1);
	}

	public void SwitchScene(string scenePath)
	{
		CallDeferred(nameof(StartLoading), scenePath);
	}

	private void StartLoading(string scenePath)
	{
		this.scenePath = scenePath;

		state = 1;

		animation.Play("Fade");
		loading = true;
		animating = true;

		Visible = true;
		panel.MouseFilter = Control.MouseFilterEnum.Stop;

		if(ResourceLoader.HasCached(scenePath))
		{
			cacheMode = true;
			return;
		}

		ResourceLoader.LoadThreadedRequest(scenePath);
	}

	private void EndLoading(PackedScene scene)
	{
		currentScene.Free();

		currentScene = scene.Instantiate();
		currentScene.SetProcess(false);
		
		GetTree().Root.AddChild(currentScene);
		GetTree().CurrentScene = currentScene;

		animation.PlayBackwards("Fade");
		panel.MouseFilter = Control.MouseFilterEnum.Ignore;

		loading = false;
		scenePath = null;
	}

	public void OnAnimationEnd(StringName animationName)
	{
		animating = false;
		switch(state)
		{
			case 2:
				Visible = false;
				currentScene.SetProcess(true);
				state = 0;
				break;
			case 1:
				state = 2;
				break;
			default:
				break;
		}
		
	}

}
