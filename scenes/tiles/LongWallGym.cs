using Godot;
using System;

public partial class LongWallGym : Node3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode<PropSpawner>("GeneratorSpawn").SpawnProp("Generator");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
