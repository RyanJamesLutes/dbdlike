using Godot;
using System;
using System.Collections.Generic;

public partial class Survivor : CharacterBody3D
{
	public enum HealthState { Healthy, Injured, Wounded, Dying };
	public enum MoveState { Crawling, Crouching, Standing, Walking, Running, Falling, Staggered, Interacting }
	public enum InteractState { None, Repairing, Healing, DroppingPallet, VaultingPallet, VaultingWindow, CleansingTotem, BooningTotem, Invoking }
	
	private Player _player;
	private Node3D _model;
	[Export] private float _speed = 2.26f;
	private float _haste = 1f;
	private float _repairSpeed = 0.016f;
	private float _mouseSensitivity = 0.002f;
	[Export] private HealthState _health = HealthState.Healthy;
	private MoveState _movement = MoveState.Standing;
	private InteractState _interaction = InteractState.None;
	private List<Node3D> _interactAreas = new List<Node3D>();
	private Node3D _interactTarget;
	private Item _heldItem;
	private AnimationPlayer _survivorAnim;
	// Camera
	private Node3D _cameraPivot;
	private SpringArm3D _cameraArm;
	private Camera3D _camera;
	private float _cameraAngleMin = -45f;
	private float _cameraAngleMax = 45f;
	
	public float Speed
	{ 
		get 
		{
			switch (_movement)
			{
				case MoveState.Running:
					return 8f * _haste;
				case MoveState.Interacting:
					return 0f;
				case MoveState.Walking:
				default:
					return 4.52f * _haste;
			}
		}
		set { _speed = value; }
	}
	public HealthState Health
	{ 
		get { return _health; }
		set { _health = value; }
	}
	public MoveState Movement
	{ 
		get { return _movement; }
		set { _movement = value; }
	}
	public InteractState Interaction
	{ 
		get { return _interaction; }
		set { _interaction = value; }
	}
	public List<Node3D> InteractAreas
	{ 
		get { return _interactAreas; }
		set { _interactAreas = value; }
	}
	public Node3D InteractTarget
	{ 
		get { return _interactTarget; }
		set { _interactTarget = value; }
	}
	public float RepairSpeed
	{
		get { return _repairSpeed; }
	}
		public Item HeldItem
	{ 
		get { return _heldItem; }
		set { _heldItem = value; }
	}
	
	public void ClearInteraction()
	{
		_interactTarget = null;
		_interaction = InteractState.None;
		GetNode<ProgressBar>("HUD/ProgressBar").Visible = false;
	}
	
	public void Injure()
	{
		switch (_health)
		{
			// TODO: Handle status effects like Endurance and Exposed.
			case HealthState.Healthy:
				_health = HealthState.Injured;
				break;
			case HealthState.Injured:
				_health = HealthState.Dying;
				break;
			case HealthState.Wounded:
				_health = HealthState.Dying;
				break;
		}
	}
	
	public void ProcessAnimations()
	{
		switch (_interaction)
		{
			// TODO
		}
		
		switch (_movement)
		{
			case MoveState.Walking: 
				if (_survivorAnim.CurrentAnimation != "UAL1/Walk")
				{
					_survivorAnim.Play("UAL1/Walk");
				}
				return;
			case MoveState.Running:
				if (_survivorAnim.CurrentAnimation != "UAL1/Sprint")
				{
					_survivorAnim.Play("UAL1/Sprint");
				}
				return;
			case MoveState.Standing:
			default:
				if (_survivorAnim.CurrentAnimation != "UAL1/Idle")
				{
					_survivorAnim.Play("UAL1/Idle");
				}
				return;
			}
		}
	
	public override void _Ready()
	{
		_player = Owner.GetNode<Player>("%Player");
		
		_model = GetNode<Node3D>("Model");
		_cameraPivot = GetNode<Node3D>("CameraPivot");
		_cameraArm = GetNode<SpringArm3D>("CameraPivot/SpringArm3D");
		_camera = GetNode<Camera3D>("CameraPivot/SpringArm3D/Camera3D");
		_survivorAnim = GetNode<AnimationPlayer>("Model/AnimationPlayer");
		// Lock the mouse cursor to the center of the screen and hide it
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}
	
	public override void _Process(double delta)
	{
		// Interaction
		if (Input.IsActionPressed("interaction 1"))
		{
			foreach (InteractArea area in _interactAreas)
			{
				// Interaction priority is determined by this order.
				if (area.GetParent() is Generator gen 
					&& gen.IsRepairable
					&& (area.InteractingBody == null || area.InteractingBody == this))
				{
					ProgressBar progressBar = GetNode<ProgressBar>("HUD/ProgressBar");
					
					_interactTarget = gen;
					_movement = MoveState.Interacting;
					_interaction = InteractState.Repairing;
					progressBar.Visible = true;
					gen.Progress += RepairSpeed;
					progressBar.Value = gen.Progress;
					
					return;
				}
			}
		}
		else if (_interaction != InteractState.None)
		{
			ClearInteraction();
		}
		
		// Movement
		if (Input.IsActionPressed("run"))
		{
			_movement = MoveState.Running;
			// GD.Print(Name + " is running.");
		}
		else if (Input.IsActionPressed("forward") 
			  || Input.IsActionPressed("backward") 
			  || Input.IsActionPressed("left")
			  || Input.IsActionPressed("right"))
		{
			_movement = MoveState.Walking;
			// GD.Print(Name + " is walking.");
		}
		else
		{
			_movement = MoveState.Standing;
			// GD.Print(Name + " is standing.");
		}
		
		ProcessAnimations();
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
		Vector2 inputDir = Input.GetVector("left", "right", "forward", "backward");
		Vector3 direction = Vector3.Zero;
		
		// Align movement direction with the camera's horizontal plane
		Vector3 cameraForward = -_cameraArm.GlobalTransform.Basis.Z;
		Vector3 cameraRight = _cameraArm.GlobalTransform.Basis.X;
		
		if (inputDir != Vector2.Zero)
		{
		cameraForward.Y = 0; // Keep movement horizontal
		cameraRight.Y = 0;
		direction = -cameraForward * inputDir.Y + cameraRight * inputDir.X;
		_model.LookAt(GlobalPosition + -direction, Vector3.Up);
		}
		
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}
		
		Velocity = velocity;
		MoveAndSlide();
	}
	
	public override void _Input(InputEvent @event)
	{
		// Handle mouse movement for camera rotation
		if (@event is InputEventMouseMotion mouseMotion)
		{
			// Rotate the horizontal pivot (Player/CameraPivot) based on mouse X movement (yaw)
			_cameraPivot.RotateY(-mouseMotion.Relative.X * +_mouseSensitivity);

			// Rotate the vertical spring arm (pitch) based on mouse Y movement
			_cameraArm.RotateX(-mouseMotion.Relative.Y * _mouseSensitivity);

			// Clamp vertical rotation to prevent flipping upside down
			Vector3 armRotation = _cameraArm.Rotation;
			armRotation.X = Mathf.Clamp(armRotation.X, Mathf.DegToRad(_cameraAngleMin), Mathf.DegToRad(_cameraAngleMax));
			_cameraArm.Rotation = armRotation;
		}
	}
}
