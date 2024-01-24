using Godot;

public partial class SettingsMenu : Control
{

	[Export(PropertyHint.Range,"0,100,1")]
	private float DefaultVolume { get; set; } = 50f;

	[Export]
	private TextSlider masterVolumeSlider;

	[Export]
	private TextSlider musicVolumeSlider;

	[Export]
	private TextSlider sfxVolumeSlider;

	private int masterBusIndex;
	private int musicBusIndex;
	private int sfxBusIndex;

	public override void _Ready()
	{
		masterBusIndex = AudioServer.GetBusIndex("Master");
		musicBusIndex = AudioServer.GetBusIndex("Music");
		sfxBusIndex = AudioServer.GetBusIndex("SFX");

		masterVolumeSlider.Value = DefaultVolume;
		musicVolumeSlider.Value = DefaultVolume;
		sfxVolumeSlider.Value = DefaultVolume;

		masterVolumeSlider.ValueChanged += UpdateMasterVolume;
		musicVolumeSlider.ValueChanged += UpdateMusicVolume;
		sfxVolumeSlider.ValueChanged += UpdateSFXVolume;

		UpdateMasterVolume(DefaultVolume);
		UpdateMusicVolume(DefaultVolume);
		UpdateSFXVolume(DefaultVolume);
	}

	private void UpdateMasterVolume(double value)
	{
		AudioServer.SetBusVolumeDb(masterBusIndex, (float)Mathf.LinearToDb(value / 100d));
	}
	
	private void UpdateMusicVolume(double value)
	{
		AudioServer.SetBusVolumeDb(musicBusIndex, (float)Mathf.LinearToDb(value / 100d));
	}
	
	private void UpdateSFXVolume(double value)
	{
		AudioServer.SetBusVolumeDb(sfxBusIndex, (float)Mathf.LinearToDb(value / 100d));
	}

}
