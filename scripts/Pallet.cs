using Godot;
using System;

public partial class Pallet : Node3D
{
	public enum PalletState { Up, Dropped, Broken, Blocked};
	public enum InteractState { Dropping, Vaulting, Breaking }
	
	private PalletState _state = PalletState.Up;
	private bool _isFragile = false;
	private StunArea _stunArea;
	private InteractState _interaction;
	private AnimationPlayer _animation;
	
	public PalletState State
	{
		get { return _state; }
		set { _state = value; }
	}
	public bool IsFragile
	{
		get { return _isFragile; }
	}
	public StunArea stunArea
	{
		get { return _stunArea; }
		set { stunArea = value; }
	}
	public InteractState Interaction
	{ 
		get { return _interaction; }
		set { _interaction = value; }
	}
	
	public void Drop(Node3D survivor)
	{
		_animation.Play("drop");
		_state = PalletState.Dropped;
		foreach(Node3D body in _stunArea.CollidingBodies)
		{
			if (body is Killer killer)
			{
				killer.Stun(this, survivor, 2.0f);
			}
		}
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_animation = GetNode<AnimationPlayer>("PalletAnim");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}
}
