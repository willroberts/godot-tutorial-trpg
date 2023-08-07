using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class UnitPath : TileMap
{
	[Export]
	public Grid Grid = ResourceLoader.Load("res://Grid.tres") as Grid;

	private Pathfinder _Pathfinder;
	public Vector2[] CurrentPath;

	public UnitPath(Array<Vector2> WalkableCells)
	{
		_Pathfinder = new Pathfinder(Grid, WalkableCells);
	}

	public override void _Ready() {}
	public override void _Process(double delta) {}

    public void Draw(Vector2 StartCell, Vector2 EndCell)
    {
        Clear();
		CurrentPath = _Pathfinder.CalculatePointPath(StartCell, EndCell);
		foreach (Vector2 Cell in CurrentPath)
		{
			//SetCell(0, SomePosition, 0, Cell, 0); // Check interface.
		}
		//UpdateBitmaskRegion(); // Old Godot 3 function. No equivalent.
		//SetCellsTerrainConnect(0, CurrentPath, 0, 0); // Vector2[] vs Array<Vector2>.
    }

	public void Stop()
	{
		_Pathfinder = null;
		Clear();
	}
}
