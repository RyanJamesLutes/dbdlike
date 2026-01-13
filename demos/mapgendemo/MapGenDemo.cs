using Godot;
using System;
using System.Collections.Generic;

public partial class MapGenDemo : Node3D
{
	private Node3D _cameraPivot;
	private List<TileSpawner> _spawners = new List<TileSpawner>();
	
	public void OnGenerateButtonPressed()
	{
		foreach (TileSpawner spawner in _spawners)
		{
			spawner.DespawnTile();
			spawner.SpawnTile(TileSpawner.RandomTileName());
		}
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_cameraPivot = GetNode<Node3D>("CameraPivot");
		foreach (Node child in GetChildren())
		{
			if (child is TileSpawner spawner)
			{
				_spawners.Add(spawner);
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		_cameraPivot.RotateY(Mathf.DegToRad(new Vector3(0, 10, 0).Y) * (float)delta);
	}
}
