using Godot;
using Godot.Collections;
using System;
using System.Collections;

[GlobalClass]
public partial class GameBoard : Node2D
{
	public readonly Vector2I[] Directions = {
		Vector2I.Left,
		Vector2I.Right,
		Vector2I.Up,
		Vector2I.Down
	};

	public Grid Grid = ResourceLoader.Load("res://Resources/Grid.tres") as Grid;
	private Dictionary<Vector2I, Unit> _units = new();
	private Unit _activeUnit;
	private Array<Vector2I> _walkableCells;
	private UnitOverlay _unitOverlay;
	private UnitPath _unitPath;

	public override void _Ready()
	{
		_unitOverlay = GetNode<UnitOverlay>("UnitOverlay");
		_unitPath = GetNode<UnitPath>("UnitPath");
		Reinitialize();

		// Debugging.
		//GD.Print(_units);
		//Unit _TestUnit = GetNode<Unit>("Unit");
		//_unitOverlay.DrawCells(GetWalkableCells(_TestUnit));
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (_activeUnit == null) { return; }

		if (@event.IsActionPressed("ui_cancel"))
		{
			DeselectActiveUnit();
			ClearActiveUnit();
		}
	}

	public bool IsOccupied(Vector2I Cell)
	{
		return _units.ContainsKey(Cell);
	}

	private void Reinitialize()
	{
		_units.Clear();

		foreach (Node Child in GetChildren())
		{
            if (Child is not Unit ThisUnit) { continue; }
			_units[ThisUnit.Cell] = ThisUnit;
		}
	}

	public Array<Vector2I> GetWalkableCells(Unit ThisUnit)
	{
		return FloodFill(ThisUnit.Cell, ThisUnit.MoveRange);
	}

	private Array<Vector2I> FloodFill(Vector2I Cell, int MaxDistance)
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
			foreach (Vector2I Direction in Directions)
			{
				Vector2I Coords = CurrentCell + Direction;
				if (IsOccupied(Coords)) { continue; }
				if (Result.Contains(Coords)) { continue; }
				CellStack.Push(Coords);
			}
		}

		return Result;
	}

	private void SelectUnit(Vector2I Cell)
	{
		if (!_units.ContainsKey(Cell)) { return; }

		_activeUnit = _units[Cell];
		_activeUnit.IsSelected = true;
		_walkableCells = GetWalkableCells(_activeUnit);
		_unitOverlay.DrawCells(_walkableCells);
		_unitPath.Initialize(_walkableCells);
	}

	private void DeselectActiveUnit()
	{
		_activeUnit.IsSelected = false;
		_unitOverlay.Clear();
		_unitPath.Stop();
	}

	private void ClearActiveUnit()
	{
		_activeUnit = null;
		_walkableCells.Clear();
	}

	private async void MoveActiveUnit(Vector2I NewCell)
	{
		if (IsOccupied(NewCell) || !_walkableCells.Contains(NewCell)) { return; }

		_units.Remove(_activeUnit.Cell);
		_units[NewCell] = _activeUnit;
		DeselectActiveUnit();
		_activeUnit.WalkAlong(_unitPath.CurrentPath);
		await ToSignal(_activeUnit, "walk_finished");
		ClearActiveUnit();
	}

	// Signal connected to GameBoard.Cursor.
	private void OnCursorMoved(Vector2I NewCell)
	{
		if (_activeUnit != null && _activeUnit.IsSelected)
		{
			_unitPath.DrawPath(_activeUnit.Cell, NewCell);
		}
	}

	// Signal connected to GameBoard.Cursor.
	private void OnCursorAcceptPressed(Vector2I Cell)
	{
		if (_activeUnit == null) {
			SelectUnit(Cell);
			return;
		}

		if (_activeUnit.IsSelected) { MoveActiveUnit(Cell); }
	}
}
