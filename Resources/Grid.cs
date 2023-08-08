using Godot;
using System;

[GlobalClass]
public partial class Grid : Resource
{
	[Export]
	public Vector2I Size = new(20, 20);

	[Export]
	public Vector2I CellSize = new(80, 80);

	private Vector2I _halfSize;

	public Grid()
	{
		_halfSize = CellSize / 2;
	}

	// Grid coordinates to Screen coordinates.
	public Vector2 CalculateMapPosition(Vector2I GridPosition)
	{
		return GridPosition * CellSize + _halfSize;
	}

	// Screen coordinates to Grid coordinates.
	public Vector2I CalculateGridPosition(Vector2 MapPosition)
	{
		Vector2 Converted = (MapPosition / CellSize).Floor();
		return new Vector2I((int)Converted.X, (int)Converted.Y);
	}

	public bool IsWithinBounds(Vector2I Coords)
	{
		return (
			Coords.X >= 0 &&
			Coords.X < Size.X &&
			Coords.Y >= 0 &&
			Coords.Y < Size.Y
		);
	}
	
	public Vector2I Clamped(Vector2I Coords)
	{
		return Coords.Clamp(
			new(0, 0),
			new(Size.X-1, Size.Y-1)
		);
	}
	
	public int AsIndex(Vector2I Cell)
	{
		return Cell.X + Size.X * Cell.Y;
	}
}
