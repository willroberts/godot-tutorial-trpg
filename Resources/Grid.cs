using Godot;
using System;

[GlobalClass]
public partial class Grid : Resource
{
	[Export]
	public Vector2I Size = new(16, 9);

	[Export]
	public Vector2I CellSize = new(80, 80);

	private Vector2I _halfSize;

	public Grid()
	{
		_halfSize = CellSize / 2;
	}

	// Grid coordinates to Screen coordinates.
	public Vector2 CalculateMapPosition(Vector2I gridPosition)
	{
		return gridPosition * CellSize + _halfSize;
	}

	// Screen coordinates to Grid coordinates.
	public Vector2I CalculateGridPosition(Vector2 mapPosition)
	{
		Vector2 converted = (mapPosition / CellSize).Floor();
		return new Vector2I((int)converted.X, (int)converted.Y);
	}

	public bool IsWithinBounds(Vector2I coords)
	{
		return (
			coords.X >= 0 &&
			coords.X < Size.X &&
			coords.Y >= 0 &&
			coords.Y < Size.Y
		);
	}
	
	public Vector2I Clamped(Vector2I coords)
	{
		return coords.Clamp(
			Vector2I.Zero,
			new(Size.X-1, Size.Y-1)
		);
	}
	
	public int AsIndex(Vector2I cell)
	{
		return cell.X + Size.X * cell.Y;
	}
}
