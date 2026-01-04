using Godot;
using System;
using BasicShapeCreation;

public partial class SkillCheck : CanvasGroup
{
	private Node2D _pointerPivot;
	private Node2D _zonePivot;
	private float _pointerSpeed = 4.0f;
	private bool _isActive = false;
	
	public void Start()
	{
		Reset();
		_isActive = true;
		Visible = true;
	}
	
	public void Reset()
	{
		_zonePivot.Rotation = Mathf.DegToRad(new Random().Next(90, 270));
		_pointerPivot.Rotation = 0f;
		_isActive = false;
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_pointerPivot = GetNode<Node2D>("PointerPivot");
		_zonePivot = GetNode<Node2D>("ZonePivot");
		
		// Start();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (_isActive && _pointerPivot.Rotation <= Mathf.DegToRad(360))
		{
			_pointerPivot.Rotation += _pointerSpeed * (float)delta;
		}
		else
		{
			Visible = false;
			Reset();
		}
	}
}
