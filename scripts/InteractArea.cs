using Godot;
using System;
using System.Collections.Generic;

public partial class InteractArea : Area3D
{
	private Node3D _interactingBody;
	private List<Node3D> _collidingBodies = new List<Node3D>();
	
	public Node3D InteractingBody
	{
		get { return _interactingBody; }
		set { _interactingBody = value; }
	}
	public List<Node3D> CollidingBodies
	{
		get { return _collidingBodies; }
		set { _collidingBodies = value; }
	}
	
	void OnBodyEntered(Node3D body)
	{
		//GD.Print(body.Name + " entered area " + GetParent().Name + "/" + Name);
		_collidingBodies.Add(body);
		if (body is Killer killer)
		{
			killer.InteractAreas.Add(this);
		}
		if (body is Survivor survivor)
		{
			survivor.InteractAreas.Add(this);
		}
	}
	
	void OnBodyExited(Node3D body)
	{
		//GD.Print(body.Name + " exited area " + GetParent().Name + "/" + Name);
		_collidingBodies.Remove(body);
		if (body is Killer killer)
		{
			killer.ClearInteraction();
			killer.InteractAreas.Remove(this);
		}
		if (body is Survivor survivor)
		{
			survivor.ClearInteraction();
			survivor.InteractAreas.Remove(this);
		}
		_interactingBody = null;
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
