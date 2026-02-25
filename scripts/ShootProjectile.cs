using Godot;
using System;

public partial class ShootProjectile : Power
{
	private PackedScene _projectileScene;
	private float _minStrength = 25.0f;
	private float _maxStrength = 40.0f;
	private float _strength = 25.0f;
	private ProgressBar _chargeProgressBar;
	private Label _powerLabel;
	
	[Export] public PackedScene ProjectileScene
	{
		get { return _projectileScene; }
		set { _projectileScene = value; }
	}
	[Export] public float MinStrength
	{
		get { return _minStrength; }
		set { _minStrength = value; }
	}
	[Export] public float MaxStrength
	{
		get { return _maxStrength; }
		set { _maxStrength = value; }
	}
	[Export] public float Strength
	{
		get { return _strength; }
		set { _strength = value; }
	}
	
	public void Shoot(Projectile projectile, Vector3 direction, float strength)
	{
		projectile.ApplyImpulse(direction * strength);
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_chargeProgressBar = GetNode<ProgressBar>("HUD/ProgressBar");
		_powerLabel = GetNode<Label>("HUD/PowerLabel");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionPressed("power"))
		{	
			if (_strength < _maxStrength)
			{
				_strength += (_maxStrength - _minStrength) * (float)delta;
			}
			else
			{
				_strength = _maxStrength;
			}
			_chargeProgressBar.Value = (_strength - _minStrength) / (_maxStrength - _minStrength) * 100;
			_chargeProgressBar.Visible = true;
		}
		else
		{
			_chargeProgressBar.Visible = false;
		}
	}
	public override void _PhysicsProcess(double delta)
	{
		if (Input.IsActionJustReleased("power"))
		{
			// Get the direction the player is looking.
			Camera3D camera = GetParent<Killer>().Camera;
			Vector3 direction = -camera.GlobalTransform.Basis.Z;
			
			// Instantiate the projectile and add it to the map.
			Projectile projectileInstance = (Projectile)_projectileScene.Instantiate();
			GetParent<Killer>().GetParent<Map>().AddChild(projectileInstance);
			
			// Set projectile spawn coordinates and properly space it away from the body.
			projectileInstance.GlobalTransform = new Transform3D(
													camera.GlobalTransform.Basis.Rotated(Vector3.Up, Mathf.Pi / 2f),
													camera.GlobalTransform.Origin - camera.GlobalTransform.Basis.Z * 3.0f
												);
			
			Shoot(projectileInstance, direction, _strength);
			_strength = _minStrength;
		}
	}
}
