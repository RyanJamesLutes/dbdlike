using Godot;
using System;

public partial class Projectile : RigidBody3D
{
	Vector3 direction;
	float velocity;
	private AnimationPlayer _projectileAnimations;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_projectileAnimations = GetNode<AnimationPlayer>("Animations");
		_projectileAnimations.Play("in-air");
	}
	
	void OnBodyEntered(Node3D body)
	{
		QueueFree();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
