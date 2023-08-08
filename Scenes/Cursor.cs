using Godot;
using System;

[GlobalClass, Tool]
public partial class Cursor : Node2D
{
	[Signal]
	public delegate void AcceptPressedEventHandler(Vector2I Cell);

	[Signal]
	public delegate void MovedEventHandler(Vector2I NewCell);

	[Export]
	public Grid Grid = ResourceLoader.Load("res://Resources/Grid.tres") as Grid;

	[Export]
	public float UICooldown = 0.1F;

	public Vector2I Cell = Vector2I.Zero;
	private Timer _timer;

	public override void _Ready()
	{
		_timer = GetNode<Timer>("Timer");
		_timer.WaitTime = UICooldown;
		Position = Grid.CalculateMapPosition(Cell);
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		// Handle mouse movement.
		if (@event is InputEventMouseMotion evt)
		{
			SetCell(Grid.CalculateGridPosition(evt.Position));
			return;
		}

		// Handle mouse click.
		if (@event.IsActionPressed("click") || @event.IsActionPressed("ui_accept"))
		{
			EmitSignal("AcceptPressed", Cell);
			GetViewport().SetInputAsHandled();
			return;
		}

		// Handle keyboard input.
		bool shouldMove = @event.IsPressed();
		if (@event.IsEcho()) { shouldMove = shouldMove && _timer.IsStopped(); }
		if (!shouldMove) { return; }

		if (@event.IsAction("ui_left")) { Cell += Vector2I.Left; }
		if (@event.IsAction("ui_right")) { Cell += Vector2I.Right; }
		if (@event.IsAction("ui_up")) {  Cell += Vector2I.Up; }
		if (@event.IsAction("ui_down")) { Cell += Vector2I.Down; }
	}

	public override void _Draw()
	{
		Rect2 rect = new(-Grid.CellSize / 2, Grid.CellSize);
		DrawRect(rect, Colors.AliceBlue, false, 2.0F);
	}

	public void SetCell(Vector2I value)
	{
		Vector2I newCell = Grid.Clamped(value);
		if (newCell.Equals(Cell)) { return; }

		Cell = newCell;
		Position = Grid.CalculateMapPosition(Cell);
		EmitSignal("Moved", Cell);
		_timer.Start();
	}
}
