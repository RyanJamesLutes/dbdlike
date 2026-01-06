using Godot;
using System;

public partial class Prop : Node3D
{
	private int _mapLimit;
	private int _tileLimit;
	
	public int MapLimit
	{
		get { return _mapLimit; }
		set { _mapLimit = value; }
	}
	
	public int TileLimit
	{
		get { return _tileLimit; }
		set { _tileLimit = value; }
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
