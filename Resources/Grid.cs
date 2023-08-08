using Godot;
using System;

[GlobalClass]
public partial class Grid : Resource
{
	[Export]
	public Vector2 Size = new Vector2(20, 20);
	
	[Export]
	public Vector2 CellSize = new Vector2(80, 80);
	
	private Vector2 HalfSize;
	
	public Grid()
	{
		HalfSize = CellSize / 2;
	}
	
	public Vector2 CalculateMapPosition(Vector2 GridPosition)
	{
		return GridPosition * CellSize + HalfSize;
	}
	
	public Vector2 CalculateGridPosition(Vector2 MapPosition)
	{
		return (MapPosition / CellSize).Floor();
	}
	
	public bool IsWithinBounds(Vector2 Coords)
	{
		if (
			Coords.X >= 0 &&
			Coords.X < Size.X &&
			Coords.Y >= 0 &&
			Coords.Y < Size.Y
		) {
			return true;	
		}
		
		return false;
	}
	
	public Vector2 Clamped(Vector2 Coords)
	{
		return Coords.Clamp(
			new Vector2(0, 0),
			new Vector2(Size.X-1, Size.Y-1)
		);
	}
	
	public int AsIndex(Vector2 Cell)
	{
		return (int)(Cell.X + Size.X * Cell.Y);
	}
}
