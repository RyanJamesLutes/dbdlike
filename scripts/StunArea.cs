using Godot;
using System;
using System.Collections.Generic;

public partial class StunArea : Area3D
{
	private List<Node3D> _collidingBodies = new List<Node3D>();
	
	public List<Node3D> CollidingBodies
	{
		get { return _collidingBodies; }
		set { _collidingBodies = value; }
	}
	
	public void OnBodyEntered(Node3D body)
	{
		//GD.Print(body.Name + " entered area " + GetParent().Name + "/" + Name);
		_collidingBodies.Add(body);
	}
	
	public void OnBodyExited(Node3D body)
	{
		//GD.Print(body.Name + " exited area " + GetParent().Name + "/" + Name);
		_collidingBodies.Remove(body);
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
