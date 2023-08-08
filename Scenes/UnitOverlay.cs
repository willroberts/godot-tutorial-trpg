using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class UnitOverlay : TileMap
{
	public void DrawCells(Array<Vector2I> Cells)
	{
		Clear();
		foreach (Vector2I Cell in Cells)
		{
			SetCell(0, Cell, 0, new Vector2I(0, 0), 0);
		}
	}
}
