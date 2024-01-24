using Godot;

[Tool]
public partial class AnimatedTextureRect : TextureRect
{

	[Export]
	private SpriteFrames Frames { get; set; }

	[Export]
	private string Animation { get; set; } = "default";

	[Export]
	private double SpeedScale { get; set; } = 1.0;

	[Export]
	private bool AutoPlay { get; set; } = false;
	
	[Export]
	private bool Playing { get; set; } = false;

	private double refreshRate = 1.0;
	private double framerate = 30.0;
	private double frameDelta = 0.0;

	private int frameIndex = 0;

	public override void _Ready()
	{
		if(Frames == null || !Frames.HasAnimation(Animation))
		{
			return;
		}

		LoadAnimationData(Animation);
		if(AutoPlay)
		{
			Play(Animation);
		}
		
	}

	public override void _Process(double delta)
	{
		if(Frames == null || !Frames.HasAnimation(Animation) || !Playing)
		{
			return;
		}

		LoadAnimationData(Animation);

		frameDelta += SpeedScale * delta;

		double timePerFrame = refreshRate / framerate;

		if(frameDelta >= timePerFrame)
		{
			frameIndex += 1;
			int frameCount = Frames.GetFrameCount(Animation);

			if(frameIndex >= frameCount)
			{
				frameIndex = 0;
				if(!Frames.GetAnimationLoop(Animation))
				{
					Playing = false;
					return;
				}

			}
			
			LoadAnimationData(Animation);

			Texture = Frames.GetFrameTexture(Animation, frameIndex);
			frameDelta -= timePerFrame;
		}

	}

	private void Play(string animationName)
	{
		frameIndex = 0;
		frameDelta = 0.0;

		Animation = animationName;
		LoadAnimationData(Animation);
		Playing = true;
	}

	private void Stop()
	{
		frameIndex = 0;
		Playing = false;
	}

	private void LoadAnimationData(string animationName)
	{
		framerate = Frames.GetAnimationSpeed(animationName);
		refreshRate = Frames.GetFrameDuration(animationName, frameIndex);
	}

}
