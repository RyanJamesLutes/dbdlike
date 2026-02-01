using Godot;
using System;

public partial class Player : Node
{
	[Serializable]
	public enum CharacterType { Killer, Survivor }
	
	private CharacterType _type;
	private int _blood;
	private PackedScene _characterScene;
	private CharacterBody3D _character;
	
	public CharacterType Type
	{
		get { return _type; }
		set { _type = value; }
	}
	public PackedScene CharacterScene
	{
		get { return _characterScene; }
		set { _characterScene = value; }
	}
	public CharacterBody3D Character
	{
		get { return _character; }
		set 
		{
			if (value is Killer)
			{
				_type = CharacterType.Killer;
			}
			else
			{
				_type = CharacterType.Survivor;
			}
			_character = value; 
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
