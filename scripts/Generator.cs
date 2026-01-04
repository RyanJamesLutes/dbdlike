using Godot;
using System;
using System.Timers;

public partial class Generator : StaticBody3D
{
	public enum GeneratorState { Idle, Repairing, Regressing, Blocked, Complete };
	
	[Export] private GeneratorState _state;
	[Export] private float _progress = 0f;
	private Node3D _light;
	private AudioStreamPlayer3D _audioDing;
	private AudioStreamPlayer3D _audioIdle;
	
	public GeneratorState State 
	{
		get { return _state; }
		set { _state = value; }
	}
	public float Progress
	{
		get { return _progress; }
		set { _progress = value; }
	}
	public bool IsRepairable
	{
		get
		{
			if (_state != GeneratorState.Blocked && _state != GeneratorState.Complete)
			{
				return true;
			}
			return false;
		}
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_light = GetNode<OmniLight3D>("OmniLight3D");
		_audioDing = GetNode<AudioStreamPlayer3D>("Sounds/Ding");
		_audioIdle = GetNode<AudioStreamPlayer3D>("Sounds/Idle");
		_state = GeneratorState.Idle;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (_progress > 0 && _audioIdle.Playing == false)
		{
			_audioIdle.Play();
		}
		if (_progress >= 100 && _state != GeneratorState.Complete)
		{
			_state = GeneratorState.Complete;
			_light.Visible = true;
			_audioDing.Play();
		}
	}
}
