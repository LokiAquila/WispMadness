using Godot;
using System;
using System.Collections.Generic;

[Tool]
public partial class SoundQueue : Node
{
    private int _next = 0;
    private List<AudioStreamPlayer> _audioStreamPlayers = new List<AudioStreamPlayer>();

    [Export]
    public int Count { get; set; } = 1;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        if (GetChildCount() == 0)
        {
            // Replace with whatever logging framework you are using.
            GD.Print("No AudioStreamPlayer child found.");
            return;
        }

        var child = GetChild(0);
        if (child is AudioStreamPlayer audioStreamPlayer)
        {
            _audioStreamPlayers.Add(audioStreamPlayer);

            for (int i = 0; i < Count; i++)
            {
                AudioStreamPlayer duplicate = audioStreamPlayer.Duplicate() as AudioStreamPlayer;
                AddChild(duplicate);
                _audioStreamPlayers.Add(duplicate);
            }
        }
    }

    public override string[] _GetConfigurationWarnings()
    {
        if (GetChildCount() == 0)
        {
            return new string[] { "No children found. Expected one AudioStreamPlayer child." };
        }

        if (GetChild(0) is not AudioStreamPlayer)
        {
            return new string[] { "Expected first child to be an AudioStreamPlayer." };
        }

        return base._GetConfigurationWarnings();
    }

    public void PlaySound()
    {
        if (!_audioStreamPlayers[_next].Playing)
        {
            _audioStreamPlayers[_next++].Play();
            _next %= _audioStreamPlayers.Count;
        }
    }
}