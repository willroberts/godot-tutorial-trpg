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

	public Grid Grid = ResourceLoader.Load("res://Grid.tres") as Grid;
	private Dictionary<Vector2I, Unit> _Units = new();

	public override void _Ready()
	{
		_Reinitialize();

		// Debugging.
		//GD.Print(_Units);
	}

	public override void _Process(double delta) {}

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
}
