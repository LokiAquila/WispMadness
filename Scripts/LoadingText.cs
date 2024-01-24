using Godot;

[Tool]
public partial class LoadingText : Label
{

	[Export]
	private string BaseText { get; set; } = "Loading";

	[Export]
	private double Speed { get; set; } = 1.0;

	private double timeElapsed = 0.0;

	private int points = 0;

    public override void _Ready()
    {
        Text = BaseText;
    }

    public override void _Process(double delta)
	{
		if(timeElapsed >= Speed)
		{
			timeElapsed -= Speed;
			points++;

			if(points > 3)
			{
				points = 0;
			}

			string text = BaseText;
			for(int i = 0; i < points; i++)
			{
				text += '.';
			}
			
			Text = text;
		}

		timeElapsed += delta;
	}

}
