using Godot;
using System;
using System.Collections.Generic;

public partial class Killer : CharacterBody3D
{
	public enum MoveState { Standing, Walking, AttackRecovery, Lunging, LungeRecovery, CarryingSurvivor, Stunned }
	public enum InteractState { None, Recovery, Breaking, Vaulting, GrabbingSurvivor, DroppingSurvivor, HookingSurvivor, Mori }
	
	private Player _player;
	private const float BaseSpeed = 9.04f;
	private float _speed = 9.04f;
	private float _acceleration = 60.0f;
	private float _deceleration = 90.0f;
	private float _lungeSpeed = 13.56f;
	private float _haste = 1.0f;
	private float _mouseSensitivity = 0.002f;
	private Camera3D _camera;
	private ProgressBar _progressBar;
	private float _cameraPitch = 0.0f;
	private float _cameraPitchMin = -60.0f;
	private float _cameraPitchMax = 60.0f;
	private MoveState _movement = MoveState.Standing;
	private InteractState _interaction = InteractState.None;
	private Survivor _carriedSurvivor;
	private List<Node3D> _interactAreas = new List<Node3D>();
	private Node3D _interactTarget;
	private AnimationPlayer _killerAnim;
	private AnimationPlayer _weaponAnim;
	
	[Export] public float Speed
	{
		get { return _speed; }
		set { _speed = value; }
	}
	[Export] public float LungeSpeed
	{
		get { return _lungeSpeed; }
		set { _lungeSpeed = value; }
	}
	[Export] public float Haste
	{
		get { return _haste; }
		set { _haste = value; }
	}
	[Export] public MoveState Movement
	{ 
		get { return _movement; }
		set 
		{
			_movement = value;
			switch (_movement)
			{
				case MoveState.Lunging:
					_speed = _lungeSpeed;
					break;
				case MoveState.AttackRecovery:
				case MoveState.LungeRecovery:
					_speed = 4.52f;
					break;
				case MoveState.Walking:
				case MoveState.Standing:
					_speed = BaseSpeed;
					break;
			}
		}
	}
	[Export] public InteractState Interaction
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
		public Camera3D Camera
	{ 
		get { return _camera; }
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
		_speed = 9.04f;
		_lungeSpeed = 13.56f;
		_haste = 1.0f;
	}
	
	public void ProcessAnimations()
	{
		switch (_interaction)
		{
			// TODO
		}
		
		switch (_movement)
		{
			case MoveState.Standing:
			default:
				if (_killerAnim.CurrentAnimation != "UAL1/Idle")
				{
					_killerAnim.Play("UAL1/Idle");
				}
				return;
		}
	}
	
	public async void DoBasicAttack() 
	{
		GetNode<Timer>("Timers/AttackButton").Stop();
		if (!_weaponAnim.IsPlaying()) 
		{ 
			_weaponAnim.Play("attack");
		} 
		await ToSignal(_weaponAnim, AnimationPlayer.SignalName.AnimationFinished);
		GetNode<Timer>("Timers/MissedAttackRecovery").Start();
		Interaction = InteractState.Recovery;
		Movement = MoveState.AttackRecovery;
		List<Node3D> targets = GetNode<BasicAttackArea>("Camera3D/AttackArea").CollidingBodies;
		
		// TODO: Write working hit detection. 
		
	}
	
	public void Lunge()
	{
		Movement = MoveState.Lunging;
		if (GetNode<Timer>("Timers/LungeTime").IsStopped()) 
		{ 
			GetNode<Timer>("Timers/LungeTime").Start();
		}
	}
	
	public async void DoLungeAttack()
	{
		GetNode<Timer>("Timers/AttackButton").Stop();
		if (!_weaponAnim.IsPlaying())
		{
			_weaponAnim.Play("attack");
		}
		
		await ToSignal(_weaponAnim, AnimationPlayer.SignalName.AnimationFinished);
		GetNode<Timer>("Timers/MissedLungeRecovery").Start();
		Interaction = InteractState.Recovery;
		Movement = MoveState.LungeRecovery;
		List<Node3D> targets = GetNode<BasicAttackArea>("Camera3D/AttackArea").CollidingBodies;
		
		// TODO: Write working hit detection. 
	}
	
	public void Stun(Node3D pallet, Node3D survivor, float seconds)
	{
		_movement = MoveState.Stunned;
	}
	
	public override void _Ready()
	{
		_player = Owner.GetNode<Player>("%Player");
		
		_camera = GetNode<Camera3D>("Camera3D");
		_progressBar = GetNode<ProgressBar>("HUD/ProgressBar");
		_killerAnim = GetNode<AnimationPlayer>("Model/AnimationPlayer");
		_weaponAnim = GetNode<AnimationPlayer>("WeaponAnim");	
		
		if (_player.Type == Player.CharacterType.Survivor)
		{
			GetNode<SpotLight3D>("RedStain").Visible = true;
			GetNode<Node3D>("Model").Visible = true;
		}
		else if (_player.Type == Player.CharacterType.Killer)
		{
			GetNode<SpotLight3D>("RedStain").Visible = false;
			GetNode<Node3D>("Model").Visible = false;
		}
		GetNode<AnimationPlayer>("RedStainAnim").Play("Noise");
		
		// Lock the mouse cursor to the center of the screen and hide it
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustReleased("interaction 1") 
		&& _interaction != InteractState.Recovery)
		{
			DoBasicAttack();
		}
		if (Input.IsActionPressed("interaction 1")
		&& _interaction != InteractState.Recovery
		&& (_movement == MoveState.Standing || _movement == MoveState.Walking)
		&& GetNode<Timer>("Timers/AttackButton").IsStopped())
		{
			GetNode<Timer>("Timers/AttackButton").Start();
		}
		
		ProcessAnimations();
		
		GD.Print($"Movement: {_movement}, Interaction: {_interaction}");
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;
		
		// Apply gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}
		
		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("left", "right", "forward", "backward");
		Vector3 direction = Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y);
		direction = direction.Normalized();
		if (_movement == MoveState.Lunging)
		{
		direction = -_camera.GlobalTransform.Basis.Z;
		direction.Y = 0;
		direction = direction.Normalized();
		}
		if (direction != Vector3.Zero)
		{
			velocity.X = Mathf.MoveToward(velocity.X, direction.X * _speed, _acceleration * (float)delta);
			velocity.Z = Mathf.MoveToward(velocity.Z, direction.Z * _speed, _acceleration * (float)delta);
		}
		else
		{
			// Decelerate when no input is given.
			velocity.X = Mathf.MoveToward(Velocity.X, 0, _deceleration * (float)delta);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, _deceleration * (float)delta);
			_movement = MoveState.Standing;
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
