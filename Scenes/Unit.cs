using Godot;
using Godot.Collections;
using System;

[GlobalClass, Tool]
public partial class Unit : Path2D
{
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
			EmitSignal("walk_finished");
		}
	}

	public void SetCell(Vector2I Value)
	{
		Cell = Grid.Clamped(Value);
	}

	public void SetIsSelected(bool Value)
	{
		IsSelected = Value;

		if (IsSelected) { _animPlayer.Play("selected"); }
		else { _animPlayer.Play("idle"); }
	}

	public async void SetSkin(Texture2D Value)
	{
		Skin = Value;
		if (_sprite == null)
		{
			// Wait until _Ready() is done.
			await ToSignal(this, "ready");
		}
		_sprite.Texture = Value;
	}

	public async void SetSkinOffset(Vector2 Value)
	{
		SkinOffset = Value;
		if (_sprite == null)
		{
			// Wait until _Ready() is done.
			await ToSignal(this, "ready");
		}
		_sprite.Position = Value;
	}

	public void WalkAlong(Array<Vector2I> Path)
	{
		if (Path.Count == 0) { return; }
		
		Curve.AddPoint(Vector2.Zero);
		foreach (Vector2I Point in Path)
		{
			Curve.AddPoint(Grid.CalculateMapPosition(Point) - Position);
		}
		
		SetCell(Path[Path.Count-1]);
		SetIsWalking(true);
	}

	private void SetIsWalking(bool Value)
	{
		_isWalking = Value;
		SetProcess(_isWalking);
	}
}
