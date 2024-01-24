using Godot;
using System;

public partial class TextSlider : HSlider
{

    [Export]
	private Label Label { get; set; }

    public override void _Ready()
    {
        ValueChanged += UpdateLabel;
    }

	private void UpdateLabel(double value)
	{
		if(Label == null)
		{
			return;
		}

		Label.Text = value.ToString("0") + "%";
	}

}
