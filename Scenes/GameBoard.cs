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

		if (OS.HasFeature("DebugMode"))
		{
			GD.Print(_units);
			Unit testUnit = GetNode<Unit>("Unit");
			_unitOverlay.DrawCells(GetWalkableCells(testUnit));
		}
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

	public bool IsOccupied(Vector2I cell)
	{
		return _units.ContainsKey(cell);
	}

	private void Reinitialize()
	{
		_units.Clear();

		foreach (Node child in GetChildren())
		{
            if (child is not Unit unit) { continue; }
			_units[unit.Cell] = unit;
		}
	}

	public Array<Vector2I> GetWalkableCells(Unit unit)
	{
		return FloodFill(unit.Cell, unit.MoveRange);
	}

	private Array<Vector2I> FloodFill(Vector2I cell, int maxDistance)
	{
		Array<Vector2I> result = new();

		System.Collections.Generic.Stack<Vector2I> CellStack = new();
		CellStack.Push(cell);
		while (CellStack.Count != 0)
		{
			Vector2I currentCell = CellStack.Pop();
			if (!Grid.IsWithinBounds(currentCell)) { continue; }
			if (result.Contains(currentCell)) { continue; }

			Vector2I difference = (currentCell - cell).Abs();
			int distance = difference.X + difference.Y;
			if (distance > maxDistance) { continue; }

			result.Add(currentCell);
			foreach (Vector2I direction in Directions)
			{
				Vector2I coords = currentCell + direction;
				if (IsOccupied(coords)) { continue; }
				if (result.Contains(coords)) { continue; }
				CellStack.Push(coords);
			}
		}

		return result;
	}

	private void SelectUnit(Vector2I cell)
	{
		if (!_units.ContainsKey(cell)) { return; }

		_activeUnit = _units[cell];
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

	private async void MoveActiveUnit(Vector2I newCell)
	{
		if (IsOccupied(newCell) || !_walkableCells.Contains(newCell)) { return; }

		_units.Remove(_activeUnit.Cell);
		_units[newCell] = _activeUnit;
		DeselectActiveUnit();

		_activeUnit.WalkAlong(_unitPath.CurrentPath);
		await ToSignal(_activeUnit, "walk_finished");
		ClearActiveUnit();
	}

	// Signal connected to GameBoard.Cursor.
	private void OnCursorMoved(Vector2I newCell)
	{
		if (_activeUnit != null && _activeUnit.IsSelected)
		{
			_unitPath.DrawPath(_activeUnit.Cell, newCell);
		}
	}

	// Signal connected to GameBoard.Cursor.
	private void OnCursorAcceptPressed(Vector2I cell)
	{
		if (_activeUnit == null) {
			SelectUnit(cell);
			return;
		}

		if (_activeUnit.IsSelected) { MoveActiveUnit(cell); }
	}
}
