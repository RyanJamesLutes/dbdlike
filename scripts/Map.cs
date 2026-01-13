using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Map : Node3D
{
	private List<Node3D> _tiles = new List<Node3D>();
	
	public List<Node3D> Tiles
	{
		get { return _tiles; }
		set { _tiles = value; }
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Instantiate tiles.
		List<TileSpawner> tileSpawns;
		
		tileSpawns = GetChildren()
			.Where(child => child is TileSpawner)
			.Select(child => child)
			.Cast<TileSpawner>()
			.ToList();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
