using Godot;
using System;
using System.Collections.Generic;

public partial class ScratchSpawner : Node3D
{
	private Random _RNG = new Random();
	private List<RayCast3D> _rayCasts = new List<RayCast3D>();
	private Timer _spawnTimer;
	
	[Export] public PackedScene scratchDecal = ResourceLoader.Load<PackedScene>("res://scenes/scratchdecal.tscn");
	
	public void SpawnScratch()
	{
		if (GetParent() is Survivor parent && parent.Movement == Survivor.MoveState.Running)
		{
			foreach(RayCast3D rayCast in _rayCasts)
			{
				if (rayCast.IsColliding())
				{
					// Get collision data.
					Vector3 collisionPoint = rayCast.GetCollisionPoint();
					Vector3 collisionNormal = rayCast.GetCollisionNormal();
					Node3D collider = rayCast.GetCollider() as Node3D;
					
					// Instantiate decal.
					Decal scratchInstance = scratchDecal.Instantiate<Decal>();
					GetTree().Root.AddChild(scratchInstance);
					
					// Position decal with random offset.
					scratchInstance.GlobalPosition = collisionPoint + collisionNormal * 0.01f; // Prevent Z-fighting with normal offset.
					
					// Rotate to align with surface normal. Almost certainly not the best way to do things but it works and I'm tired of thinking about vector transformations.
					switch (rayCast.Name)
					{
						case "RayCastFront":
							scratchInstance.LookAt(GlobalPosition.Normalized(), Vector3.Back);
							break;
						case "RayCastLeft":
							scratchInstance.LookAt(GlobalPosition.Normalized(), Vector3.Right);
							break;
						case "RayCastRight":
							scratchInstance.LookAt(GlobalPosition.Normalized(), Vector3.Left);
							break;
						case "RayCastBack":
							scratchInstance.LookAt(GlobalPosition.Normalized(), Vector3.Forward);
							break;
						case "RayCastDown":
							scratchInstance.LookAt(GlobalPosition.Normalized(), Vector3.Up);
							break;
					}
				}
			}
		}
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		foreach (Node child in GetChildren())
		{
			if (child is RayCast3D rayCast)
			{
				_rayCasts.Add(rayCast);
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
