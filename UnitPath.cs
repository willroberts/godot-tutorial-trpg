using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class UnitPath : TileMap
{
	[Export]
	public Grid Grid = ResourceLoader.Load("res://Grid.tres") as Grid;

	private Pathfinder _Pathfinder;
	public Array<Vector2I> CurrentPath;

	public UnitPath(Array<Vector2> WalkableCells)
	{
		_Pathfinder = new Pathfinder(Grid, WalkableCells);
	}

	public override void _Ready() {}
	public override void _Process(double delta) {}

    public void Draw(Vector2I StartCell, Vector2I EndCell)
    {
        Clear();
		CurrentPath = ToVector2I(new Array<Vector2>(_Pathfinder.CalculatePointPath(StartCell, EndCell)));
		foreach (Vector2I Cell in CurrentPath)
		{
			SetCell(0, Cell, 0, new Vector2I(0, 0), 0);
		}
		SetCellsTerrainConnect(0, new Array<Vector2I>(CurrentPath), 0, 0); // Vector2[] vs Array<Vector2>.
    }

	public void Stop()
	{
		_Pathfinder = null;
		Clear();
	}

	private Array<Vector2I> ToVector2I(Array<Vector2> Input)
	{
		Array<Vector2I> Out = new();
		foreach (Vector2 V in Input)
		{
			Out.Add(new Vector2I((int)V.X, (int)V.Y));
		}
		return Out;
	}
}
