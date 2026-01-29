using Godot;
using System;
using System.Collections.Generic;

public partial class Killer : CharacterBody3D
{
	public enum MoveState { Walking, Lunging, LungeRecovery, CarryingSurvivor, Stunned }
	public enum InteractState { None, AttackRecovery, Breaking, Vaulting, GrabbingSurvivor, DroppingSurvivor, HookingSurvivor, Mori }
	
	private float _speed = 9.2f;
	private float _haste = 1.0f;
	private float _mouseSensitivity = 0.002f;	
	private Camera3D _camera;
	private ProgressBar _progressBar;
	private float _cameraPitch = 0f;
	private float _cameraPitchMin = -60f;
	private float _cameraPitchMax = 60f;
	private MoveState _movement = MoveState.Walking;
	private InteractState _interaction = InteractState.None;
	private Survivor _carriedSurvivor;
	private List<Node3D> _interactAreas = new List<Node3D>();
	private BasicAttackArea _basicAttackArea;
	private Node3D _interactTarget;
	private AnimationPlayer _killerAnim;
	private AnimationPlayer _weaponAnim;
	
	[Export] public float Speed 
	{ 
		get { return _speed; }
		set { _speed = value; }
	}
	[Export] public float Haste
	{ 
		get { return _haste; }
		set { _haste = value; }
	}
	[Export]public MoveState Movement
	{ 
		get { return _movement; }
		set { _movement = value; }
	}
	[Export]public InteractState Interaction
	{ 
		get { return _interaction; }
		set { _interaction = value; }
	}
	public Survivor CarriedSurvivor
	{ 
		get { return _carriedSurvivor; }
		set { _carriedSurvivor = value; }
	}
	public List<Node3D> InteractAreas
	{ 
		get { return _interactAreas; }
		set { _interactAreas = value; }
	}
	
	public void ClearInteraction()
	{
		if (_interactTarget is Survivor survivor)
		{
			survivor.ClearInteraction();
		}
		
		_interactTarget = null;
		_interaction = InteractState.None;
		_progressBar.Visible = false;
	}
	public void ClearMovement()
	{
		_movement = MoveState.Walking;
		_speed = 4.6f;
		_haste = 1.0f;
	}
	
	public void DoBasicAttack()
	{
			_weaponAnim.Play("basicattack");
			_interaction = InteractState.AttackRecovery;
			GetNode<Timer>("Timers/AttackRecovery").Start();
			List<Node3D> targets = _basicAttackArea.CollidingBodies;
			
			// Sort list by distance to Killer.
			targets.Sort((a, b) => a.GlobalPosition.DistanceTo(this.GlobalPosition).CompareTo(b.GlobalPosition.DistanceTo(this.GlobalPosition)));
			
			if (targets[0] is Survivor survivor)
			{
				survivor.Injure();
			}
	}
	
	public void Stun(Node3D pallet, Node3D survivor, float seconds)
	{
		_movement = MoveState.Stunned;
	}
	
	public override void _Ready()
	{
		_camera = GetNode<Camera3D>("Camera3D");
		_progressBar = GetNode<ProgressBar>("HUD/ProgressBar");
		_weaponAnim = GetNode<AnimationPlayer>("WeaponAnim");	
		_basicAttackArea = GetNode<BasicAttackArea>("Camera3D/BasicAttackArea");
		
		GetNode<AnimationPlayer>("RedStainAnim").Play("noise");
		
		// Lock the mouse cursor to the center of the screen and hide it
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}
	
	public override void _Process(double delta)
	{
		if (Input.IsActionJustReleased("interaction 1") && _interaction != InteractState.AttackRecovery)
		{
			DoBasicAttack();
		}
	}
	
	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;
		
		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}
			
			// Get the input direction and handle the movement/deceleration.
			// As good practice, you should replace UI actions with custom gameplay actions.
			Vector2 inputDir = Input.GetVector("left", "right", "forward", "backward");
			Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
			if (direction != Vector3.Zero)
			{
				velocity.X = direction.X * _speed * _haste;
				velocity.Z = direction.Z * _speed * _haste;
			}
			else
			{
				velocity.X = Mathf.MoveToward(Velocity.X, 0, _speed * _haste);
				velocity.Z = Mathf.MoveToward(Velocity.Z, 0, _speed * _haste);
			}

		Velocity = velocity;
		MoveAndSlide();
	}
	
	public override void _Input(InputEvent @event)
	{
		// Handle mouse movement for camera rotation
		if (@event is InputEventMouseMotion mouseMotion)
		{
			// Horizontal rotation (around Y axis) applied to the parent body
			RotateY(-mouseMotion.Relative.X * _mouseSensitivity);
			// Vertical rotation (pitch, around X axis) applied to the camera
			_cameraPitch += -mouseMotion.Relative.Y * _mouseSensitivity;
			// Clamp the pitch to prevent flipping the camera upside down (e.g., -90 to 90 degrees)
			_cameraPitch = Mathf.Clamp(_cameraPitch, Mathf.DegToRad(_cameraPitchMin), Mathf.DegToRad(_cameraPitchMax));
			// Apply the rotation to the camera
			_camera.Rotation = new Vector3(_cameraPitch, _camera.Rotation.Y, _camera.Rotation.Z);
		}
	}
}
