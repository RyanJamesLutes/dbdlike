using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class TileSpawner : Node3D
{
	private static Random _RNG = new Random();
	private PackedScene _tile;
	private Node3D _tileInstance;
	
	public static Dictionary<string, string> TileDict = new Dictionary<string, string>
	{
		{ "LTWall", "res://scenes/tiles/ltwall.tscn" },
		{ "LongWallGym", "res://scenes/tiles/longwallgym.tscn"}
	};
	
	private List<Node3D> _tiles = new List<Node3D>();
	private List<Node3D> _props = new List<Node3D>();
	private List <Node3D> _pallets = new List <Node3D>();
	
		public List<Node3D> Props
	{
		get { return _props; }
		set { _props = value; }
	}
		public List<Node3D> Pallets
	{
		get { return _pallets; }
		set { _pallets = value; }
	}
	
	public static string RandomTileName()
	{
		return TileDict.ElementAt(_RNG.Next(0, TileDict.Count)).Key;
	}
	
	public void SpawnTile(string tileName)
	{
		// Load scene.
		_tile = ResourceLoader.Load<PackedScene>(TileDict[tileName]);
		// Instantiate scene and add it to SceneTree.
		_tileInstance = (Node3D)_tile.Instantiate();
		AddChild(_tileInstance);
		GD.Print(Name + " spawned tile: " + tileName);
	}
	
	public void DespawnTile()
	{
		_tileInstance.QueueFree();
		_tileInstance = null;
		GD.Print(Name + " despawned tile.");
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SpawnTile(RandomTileName());
		_tileInstance.RotateY(Mathf.DegToRad(90 * _RNG.Next(3)));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
