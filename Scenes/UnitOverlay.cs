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
			SetCell(0, cell, 0, Vector2I.Zero, 0);
		}
	}
}
