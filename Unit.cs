using Godot;
using Godot.Collections;
using System;

[GlobalClass, Tool]
public partial class Unit : Path2D
{
	[Export]
	public Grid Grid = ResourceLoader.Load("res://Grid.tres") as Grid;

	[Export]
	public int MoveRange = 4;

	[Export]
	public Texture Skin;

	[Export]
	public  Vector2 SkinOffset = Vector2.Zero;

	[Export]
	public float MoveSpeed = (float)600.0;

	public Vector2 Cell = Vector2.Zero;
	public bool IsSelected = false;
	private bool _IsWalking = false;
	private Sprite2D _Sprite;
	private AnimationPlayer _AnimPlayer;
	private PathFollow2D _PathFollow;

	public override void _Ready()
	{
		SetProcess(false);

		_PathFollow = GetNode<PathFollow2D>("PathFollow2D");
		_Sprite = _PathFollow.GetNode<Sprite2D>("Sprite");
		_AnimPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

		SetCell(Grid.CalculateGridPosition(Position));
		Position = Grid.CalculateMapPosition(Cell);
		
		if (!Engine.IsEditorHint())	{ Curve = new Curve2D(); }
	}

	public override void _Process(double delta)
	{
		_PathFollow.Progress += MoveSpeed * (float)delta;
		
		if (_PathFollow.ProgressRatio >= 1.0)
		{
			_SetIsWalking(false);
			_PathFollow.Progress = 0.0F;
			Position = Grid.CalculateMapPosition(Cell);
			Curve.ClearPoints();
			EmitSignal("walk_finished");
		}
	}

	public void SetCell(Vector2 Value)
	{
		Cell = Grid.Clamped(Value);
	}

	public void SetIsSelected(bool Value)
	{
		IsSelected = Value;
		if (IsSelected)
		{
			_AnimPlayer.Play("selected");
		}
		else
		{
			_AnimPlayer.Play("idle");
		}
	}

	public async void SetSkin(Texture2D Value)
	{
		Skin = Value;
		if (_Sprite == null)
		{
			// Wait until _Ready() is done.
			await ToSignal(this, "ready");
		}
		_Sprite.Texture = Value;
	}

	public async void SetSkinOffset(Vector2 Value)
	{
		SkinOffset = Value;
		if (_Sprite == null)
		{
			// Wait until _Ready() is done.
			await ToSignal(this, "ready");
		}
		_Sprite.Position = Value;
	}

	public void WalkAlong(Godot.Collections.Array<Vector2> Path)
	{
		if (Path.Count == 0) { return; }
		
		Curve.AddPoint(Vector2.Zero);
		foreach (Vector2 Point in Path)
		{
			Curve.AddPoint(Grid.CalculateMapPosition(Point) - Position);
		}
		
		SetCell(Path[Path.Count-1]);
		_SetIsWalking(true);
	}

	private void _SetIsWalking(bool Value)
	{
		_IsWalking = Value;
		SetProcess(_IsWalking);
	}
}
