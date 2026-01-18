using Godot;
using System;

public partial class ScratchDecal : Decal
{
	private Timer _lifespanTimer;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_lifespanTimer = GetNode<Timer>("LifespanTimer");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (AlbedoMix < 1.0f)
		{
			AlbedoMix += (float)delta;
		}
		if (_lifespanTimer.TimeLeft <= 1)
		{
			AlbedoMix -= (float)delta;
		}
		
	}
}
