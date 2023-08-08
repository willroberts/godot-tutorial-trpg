using Godot;
using System;

[GlobalClass, Tool]
public partial class Cursor : Node2D
{
	//[Signal]
	//blah AcceptPressedSignalHandler(Vector2 Cell)

	//[Signal]
	//blah MovedSignalHandler(Vector2 NewCell)

	[Export]
	public Grid Grid = ResourceLoader.Load("res://Grid.tres") as Grid;

	[Export]
	public float UICooldown = 0.1F;

	public Vector2 Cell = Vector2.Zero;
	private Timer _Timer;

	public override void _Ready()
	{
		_Timer = GetNode<Timer>("Timer");
		_Timer.WaitTime = UICooldown;
		Position = Grid.CalculateMapPosition(Cell);
	}

	public override void _Process(double delta) {}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventMouseMotion evt)
		{
			Cell = Grid.CalculateGridPosition(evt.Position);
		}
		else if (@event.IsActionPressed("click") ||
				 @event.IsActionPressed("ui_accept"))
		{
			EmitSignal("accept_pressed", Cell);
			GetViewport().SetInputAsHandled();
		}

		bool ShouldMove = @event.IsPressed();
		if (@event.IsEcho())
		{
			ShouldMove = ShouldMove && _Timer.IsStopped();
		}
		if (!ShouldMove) { return; }

		if (@event.IsAction("ui_up")) { Cell = Vector2.Up; }
		if (@event.IsAction("ui_down")) { Cell = Vector2.Down; }
		if (@event.IsAction("ui_left")) { Cell = Vector2.Left; }
		if (@event.IsAction("ui_right")) { Cell = Vector2.Right; }
	}

	public override void _Draw()
	{
		DrawRect(new Rect2(-Grid.CellSize / 2, Grid.CellSize),
				 Colors.AliceBlue, false, 2.0F);
	}

	public void SetCell(Vector2 Value)
	{
		Vector2 NewCell = Grid.Clamped(Value);
		if (NewCell.IsEqualApprox(Cell)) { return; }

		Cell = NewCell;
		Position = Grid.CalculateMapPosition(Cell);
		EmitSignal("moved", Cell);
		_Timer.Start();
	}
}
