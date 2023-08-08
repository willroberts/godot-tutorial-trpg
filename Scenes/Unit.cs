using Godot;
using Godot.Collections;
using System;

[GlobalClass, Tool]
public partial class Unit : Path2D
{
	[Signal]
	public delegate void WalkFinishedEventHandler();

	[Export]
	public Grid Grid = ResourceLoader.Load("res://Resources/Grid.tres") as Grid;

	[Export]
	public int MoveRange = 4;

	[Export]
	public Texture Skin;

	[Export]
	public  Vector2 SkinOffset = Vector2.Zero;

	[Export]
	public float MoveSpeed = (float)600.0;

	public Vector2I Cell = Vector2I.Zero;
	public bool IsSelected = false;
	private bool _isWalking = false;
	private Sprite2D _sprite;
	private AnimationPlayer _animPlayer;
	private PathFollow2D _pathFollow;

	public override void _Ready()
	{
		SetProcess(false);

		_pathFollow = GetNode<PathFollow2D>("PathFollow2D");
		_sprite = _pathFollow.GetNode<Sprite2D>("Sprite");
		_animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

		SetCell(Grid.CalculateGridPosition(Position));
		Position = Grid.CalculateMapPosition(Cell);
		
		if (!Engine.IsEditorHint()) { Curve = new Curve2D(); }
	}

	public override void _Process(double delta)
	{
		_pathFollow.Progress += MoveSpeed * (float)delta;
		
		if (_pathFollow.ProgressRatio >= 1.0)
		{
			SetIsWalking(false);
			_pathFollow.Progress = 0.0F;
			Position = Grid.CalculateMapPosition(Cell);
			Curve.ClearPoints();
			EmitSignal("WalkFinished");
		}
	}

	public void SetCell(Vector2I value)
	{
		Cell = Grid.Clamped(value);
	}

	public void SetIsSelected(bool value)
	{
		IsSelected = value;

		if (IsSelected) { _animPlayer.Play("selected"); }
		else { _animPlayer.Play("idle"); }
	}

	public async void SetSkin(Texture2D value)
	{
		Skin = value;
		if (_sprite == null)
		{
			// Wait until _Ready() is done.
			await ToSignal(this, "ready");
		}
		_sprite.Texture = value;
	}

	public async void SetSkinOffset(Vector2 value)
	{
		SkinOffset = value;
		if (_sprite == null)
		{
			// Wait until _Ready() is done.
			await ToSignal(this, "ready");
		}
		_sprite.Position = value;
	}

	public void WalkAlong(Array<Vector2I> path)
	{
		if (path.Count == 0) {
			GD.Print("Error: Empty path provided to WalkAlong()");
			return;
		}
		
		Curve.AddPoint(Vector2.Zero);
		foreach (Vector2I point in path)
		{
			Curve.AddPoint(Grid.CalculateMapPosition(point) - Position);
		}
		
		SetCell(path[^1]);
		SetIsWalking(true);
	}

	private void SetIsWalking(bool value)
	{
		_isWalking = value;
		SetProcess(_isWalking);
	}
}
