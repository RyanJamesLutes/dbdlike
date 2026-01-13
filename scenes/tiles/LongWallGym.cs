using Godot;
using System;

public partial class LongWallGym : Node3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Random RNG = new Random();
		RotateY(Mathf.DegToRad(90 * RNG.Next(3)));
		GetNode<PropSpawner>("GeneratorSpawn").SpawnProp("Generator");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
