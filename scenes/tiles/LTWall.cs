using Godot;
using System;

public partial class LTWall : Node3D
{	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Random RNG = new Random();
		
		GetNode<PropSpawner>("GeneratorSpawn").SpawnProp("Generator");
		RotateY(Mathf.DegToRad(90 * RNG.Next(4)));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
