using Godot;
using System;

public partial class Scratch : Node3D
{
	private Timer _lifespanTimer;
	private Decal _decal;
	
	public Decal GetDecal
	{
		get { return _decal; }
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_lifespanTimer = GetNode<Timer>("LifespanTimer");
		_decal = GetNode<Decal>("Decal");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (_lifespanTimer.TimeLeft >= 9)
		{
			_decal.AlbedoMix += (float)delta;
			_decal.EmissionEnergy += (float)delta;
		}
		if (_lifespanTimer.TimeLeft <= 1)
		{
			_decal.AlbedoMix -= (float)delta;
			_decal.EmissionEnergy -= (float)delta;
		}
		
	}
}
