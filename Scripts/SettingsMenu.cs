using Godot;

public partial class SettingsMenu : Control
{

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

		masterVolumeSlider.ValueChanged += UpdateMasterVolume;
		musicVolumeSlider.ValueChanged += UpdateMusicVolume;
		sfxVolumeSlider.ValueChanged += UpdateSFXVolume;
	}

	private void UpdateMasterVolume(double value)
	{
		AudioServer.SetBusVolumeDb(masterBusIndex, (float)Mathf.LinearToDb(value));
	}
	
	private void UpdateMusicVolume(double value)
	{
		AudioServer.SetBusVolumeDb(musicBusIndex, (float)Mathf.LinearToDb(value));
	}
	
	private void UpdateSFXVolume(double value)
	{
		AudioServer.SetBusVolumeDb(sfxBusIndex, (float)Mathf.LinearToDb(value));
	}

}
