using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class UnitOverlay : TileMap
{
	public void DrawCells(Array<Vector2I> cells)
	{
		Clear();
		foreach (Vector2I cell in cells)
		{
			SetCell(0, cell, 0, new Vector2I(0, 0), 0);
		}
	}
}
