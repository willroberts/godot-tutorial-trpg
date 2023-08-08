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

	public void Initialize(Array<Vector2I> walkableCells)
	{
		_pathfinder = new Pathfinder(Grid, walkableCells);
	}

	public override void _Ready()
	{
		if (OS.HasFeature("debugmode"))
		{
			Vector2I startRect = new(4, 4);
			Vector2I endRect = new(10, 8);
			Array<Vector2I> points = new();
			foreach (int x in Enumerable.Range(0, endRect.X - startRect.X + 1))
			{
				foreach (int y in Enumerable.Range(0, endRect.Y - startRect.Y + 1))
				{
					points.Add(startRect + new Vector2I(x, y));
				}
			}
			Initialize(points);
			DrawPath(startRect, new Vector2I(8, 7));
		}
	}

	public void DrawPath(Vector2I startCell, Vector2I endCell)
	{
		Clear();
		CurrentPath = _pathfinder.CalculatePointPath(startCell, endCell);
		foreach (Vector2I cell in CurrentPath)
		{
			SetCell(0, cell, 0, new Vector2I(0, 0), 0);
		}
		SetCellsTerrainConnect(0, new Array<Vector2I>(CurrentPath), 0, 0);
	}

	public void Stop()
	{
		_pathfinder = null;
		Clear();
	}
}
