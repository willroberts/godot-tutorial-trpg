using Godot;
using Godot.Collections;
using System;
using System.Linq;

[GlobalClass]
public partial class UnitPath : TileMap
{
	[Export]
	public Grid Grid = ResourceLoader.Load("res://Resources/Grid.tres") as Grid;

	private Pathfinder _pathfinder;
	public Array<Vector2I> CurrentPath;

	public void Initialize(Array<Vector2I> WalkableCells)
	{
		_pathfinder = new Pathfinder(Grid, WalkableCells);
	}

	public override void _Ready()
	{
		// Debugging.
		/*
		Vector2 StartRect = new Vector2(4, 4);
		Vector2 EndRect = new Vector2(10, 8);
		Array<Vector2I> Points = new();
		foreach (int X in Enumerable.Range(0, (int)(EndRect.X - StartRect.X + 1)))
		{
			foreach (int Y in Enumerable.Range(0, (int)(EndRect.Y - StartRect.Y + 1)))
			{
				Points.Add(StartRect + new Vector2(X, Y));
			}
		}
		Initialize(Points);
		DrawPath(new Vector2I((int)StartRect.X, (int)StartRect.Y), new Vector2I(8, 7));
		*/
	}

	public void DrawPath(Vector2I StartCell, Vector2I EndCell)
	{
		Clear();
		CurrentPath = _pathfinder.CalculatePointPath(StartCell, EndCell);
		foreach (Vector2I Cell in CurrentPath)
		{
			SetCell(0, Cell, 0, new Vector2I(0, 0), 0);
		}
		SetCellsTerrainConnect(0, new Array<Vector2I>(CurrentPath), 0, 0);
	}

	public void Stop()
	{
		_pathfinder = null;
		Clear();
	}
}
