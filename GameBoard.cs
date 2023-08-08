using Godot;
using Godot.Collections;
using System;
using System.Collections;

[GlobalClass]
public partial class GameBoard : Node2D
{
	public Array<Vector2I> DIRECTIONS = new(){
		Vector2I.Left,
		Vector2I.Right,
		Vector2I.Up,
		Vector2I.Down
	};

	public Grid Grid = ResourceLoader.Load("res://Resources/Grid.tres") as Grid;
	private Dictionary<Vector2I, Unit> _Units = new();
	private Unit _ActiveUnit;
	private Array<Vector2I> _WalkableCells;
	private UnitOverlay _UnitOverlay;
	private UnitPath _UnitPath;

	public override void _Ready()
	{
		_UnitOverlay = GetNode<UnitOverlay>("UnitOverlay");
		_UnitPath = GetNode<UnitPath>("UnitPath");
		_Reinitialize();

		// Debugging.
		//GD.Print(_Units);
		//Unit _TestUnit = GetNode<Unit>("Unit");
		//_UnitOverlay.DrawCells(GetWalkableCells(_TestUnit));
	}

	public override void _Process(double delta) {}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (_ActiveUnit == null) { return; }

		if (@event.IsActionPressed("ui_cancel"))
		{
			_DeselectActiveUnit();
			_ClearActiveUnit();
		}
	}

	public bool IsOccupied(Vector2I Cell)
	{
		if (_Units.ContainsKey(Cell))
		{
			return true;
		}
		return false;
	}

	private void _Reinitialize()
	{
		_Units.Clear();

		foreach (Node Child in GetChildren())
		{
			Unit ThisUnit = Child as Unit;
			if (ThisUnit == null) { continue; }

			Vector2I Coords = new Vector2I(
				(int)ThisUnit.Cell.X,
				(int)ThisUnit.Cell.Y
			);
			_Units[Coords] = ThisUnit;
		}
	}

	public Array<Vector2I> GetWalkableCells(Unit ThisUnit)
	{
		return _FloodFill(
			new Vector2I(
				(int)ThisUnit.Cell.X,
				(int)ThisUnit.Cell.Y
			),
			ThisUnit.MoveRange
		);
	}

	private Array<Vector2I> _FloodFill(Vector2I Cell, int MaxDistance)
	{
		Array<Vector2I> Result = new();

		System.Collections.Generic.Stack<Vector2I> CellStack = new();
		CellStack.Push(Cell);
		while (CellStack.Count != 0)
		{
			Vector2I CurrentCell = CellStack.Pop();
			if (!Grid.IsWithinBounds(CurrentCell)) { continue; }
			if (Result.Contains(CurrentCell)) { continue; }

			Vector2I Difference = (CurrentCell - Cell).Abs();
			int Distance = Difference.X + Difference.Y;
			if (Distance > MaxDistance) { continue; }

			Result.Add(CurrentCell);
			foreach (Vector2I Direction in DIRECTIONS)
			{
				Vector2I Coords = CurrentCell + Direction;
				if (IsOccupied(Coords)) { continue; }
				if (Result.Contains(Coords)) { continue; }
				CellStack.Push(Coords);
			}
		}

		return Result;
	}

	private void _SelectUnit(Vector2I Cell)
	{
		if (!_Units.ContainsKey(Cell)) { return; }

		_ActiveUnit = _Units[Cell];
		_ActiveUnit.IsSelected = true;
		_WalkableCells = GetWalkableCells(_ActiveUnit);
		_UnitOverlay.DrawCells(_WalkableCells);
		_UnitPath.Initialize(ToVector2(_WalkableCells));
	}

	private void _DeselectActiveUnit()
	{
		_ActiveUnit.IsSelected = false;
		_UnitOverlay.Clear();
		_UnitPath.Stop();
	}

	private void _ClearActiveUnit()
	{
		_ActiveUnit = null;
		_WalkableCells.Clear();
	}

	private async void _MoveActiveUnit(Vector2I NewCell)
	{
		if (IsOccupied(NewCell) || !_WalkableCells.Contains(NewCell)) { return; }

		_Units.Remove(new Vector2I(
			(int)_ActiveUnit.Cell.X,
			(int)_ActiveUnit.Cell.Y
		));
		_Units[NewCell] = _ActiveUnit;
		_DeselectActiveUnit();
		_ActiveUnit.WalkAlong(ToVector2(_UnitPath.CurrentPath));
		await ToSignal(_ActiveUnit, "walk_finished");
		_ClearActiveUnit();
	}

	private void _OnCursorMoved(Vector2 NewCell)
	{
		if (_ActiveUnit != null && _ActiveUnit.IsSelected)
		{
			_UnitPath.DrawPath(
				new Vector2I(
					(int)_ActiveUnit.Cell.X,
					(int)_ActiveUnit.Cell.Y
				),
				new Vector2I(
					(int)NewCell.X,
					(int)NewCell.Y
				)
			);
		}
	}

	private void _OnCursorAcceptPressed(Vector2 Cell)
	{
		if (_ActiveUnit == null) {
			_SelectUnit(new Vector2I(
				(int)Cell.X,
				(int)Cell.Y
			));
			return;
		}

		if (_ActiveUnit.IsSelected)
		{
			_MoveActiveUnit(new Vector2I(
				(int)Cell.X,
				(int)Cell.Y
			));
		}
	}

	// Dumb and inefficient. Should have started with Vector2I from the onset.
	private Array<Vector2> ToVector2(Array<Vector2I> Input)
	{
		Array<Vector2> Out = new();
		foreach (Vector2I V in Input)
		{
			Out.Add(new Vector2(V.X, V.Y));
		}
		return Out;
	}

	// Dumb and inefficient. Should have started with Vector2I from the onset.
	private Array<Vector2I> ToVector2I(Array<Vector2> Input)
	{
		Array<Vector2I> Out = new();
		foreach (Vector2 V in Input)
		{
			Out.Add(new Vector2I((int)V.X, (int)V.Y));
		}
		return Out;
	}
}