using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class PropSpawner : Node3D
{
	private Node _propInstance;
	
	private static Dictionary<string, string> PropDict = new Dictionary<string, string>
	{
		{ "Generator", "res://scenes/props/generator.tscn" }
	};
	
	public void SpawnProp(string propName)
	{
		if (_propInstance != null)
		{
			DespawnProp();
		}
		// Load scene.
		PackedScene _prop = ResourceLoader.Load<PackedScene>(PropDict[propName]);
		// Instantiate scene and add it to SceneTree.
		_propInstance = _prop.Instantiate();
		AddChild(_propInstance);
	}
	
	public void DespawnProp()
	{
		if (_propInstance != null)
		{
			_propInstance.QueueFree();
		}
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
