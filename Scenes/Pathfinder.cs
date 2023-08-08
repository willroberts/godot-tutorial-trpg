using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class Pathfinder : RefCounted
{
    public readonly Vector2I[] Directions = {
        Vector2I.Left,
        Vector2I.Right,
        Vector2I.Up,
        Vector2I.Down
    };

    private Grid _grid;

    private AStar2D _aStar = new AStar2D();

    public Pathfinder(Grid Grid, Array<Vector2I> WalkableCells)
    {
        _grid = Grid;
        Dictionary<Vector2I, int> CellMappings = new();
        foreach (Vector2I Cell in WalkableCells)
        {
            CellMappings[Cell] = _grid.AsIndex(Cell);
        }
        AddAndConnectPoints(CellMappings);
    }

    public Array<Vector2I> CalculatePointPath(Vector2I Start, Vector2I End)
    {
        int StartIndex = _grid.AsIndex(Start);
        int EndIndex = _grid.AsIndex(End);
        if (_aStar.HasPoint(StartIndex) && _aStar.HasPoint(EndIndex))
        {
            return UnpackArray(_aStar.GetPointPath(StartIndex, EndIndex));
        }
        return new Array<Vector2I>();
    }

    private void AddAndConnectPoints(Dictionary<Vector2I, int> CellMappings)
    {
        foreach (var (Key, Value) in CellMappings)
        {
            _aStar.AddPoint(CellMappings[Key], Key);
        }
        foreach (var (Key, Value) in CellMappings)
        {
            foreach (int NeighborIndex in FindNeighborIndices(Key, CellMappings))
            {
                _aStar.ConnectPoints(CellMappings[Key], NeighborIndex);
            }
        }
    }

    private Array<int> FindNeighborIndices(Vector2I Cell, Dictionary<Vector2I, int> CellMappings)
    {
        Array<int> Result = new();
        foreach (Vector2I Direction in Directions)
        {
            Vector2I Neighbor = Cell + Direction;
            if (!CellMappings.ContainsKey(Neighbor)) { continue; }

            if (!_aStar.ArePointsConnected(CellMappings[Cell], CellMappings[Neighbor]))
            {
                Result.Add(CellMappings[Neighbor]);
            }
        }
        return Result;
    }

    // Convenience function for converting Vector2[]] to Array<Vector2I>.
    // Useful when dealing with AStar, which returns Vector2 values.
    private Array<Vector2I> UnpackArray(Vector2[] Input)
    {
            Array<Vector2I> Converted = new();
            foreach (Vector2 Value in Input)
            {
                Converted.Add(new Vector2I((int)Value.X, (int)Value.Y));
            }
            return Converted;
    }
}
