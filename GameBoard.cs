using Godot;
using Godot.Collections;
using System;

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
		GD.Print(_Units);
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
}
