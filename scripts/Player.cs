using Godot;
using System;

public partial class Player : Node
{
	public enum PlayerType { Killer, Survivor }
	
	private PlayerType _type;
	private PackedScene _character;
	
	public PlayerType Type
	{
		get { return _type; }
		set { _type = value; }
	}
	public PackedScene Character
	{
		get { return _character; }
		set { _character = value; }
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
