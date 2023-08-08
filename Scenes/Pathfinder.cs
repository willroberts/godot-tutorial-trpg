using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class Pathfinder : RefCounted
{
    public Array<Vector2> DIRECTIONS = new()
    {
        Vector2.Left,
        Vector2.Right,
        Vector2.Up,
        Vector2.Down
    };

    private Grid _Grid;

    private AStar2D _AStar = new AStar2D();

    public Pathfinder(Grid Grid, Array<Vector2I> WalkableCells)
    {
        _Grid = Grid;
        Dictionary<Vector2I, int> CellMappings = new();
        foreach (Vector2I Cell in WalkableCells)
        {
            CellMappings[Cell] = _Grid.AsIndex(Cell);
        }
        _AddAndConnectPoints(CellMappings);
    }

    public Array<Vector2I> CalculatePointPath(Vector2I Start, Vector2I End)
    {
        int StartIndex = _Grid.AsIndex(Start);
        int EndIndex = _Grid.AsIndex(End);
        if (_AStar.HasPoint(StartIndex) && _AStar.HasPoint(EndIndex))
        {
            return UnpackArray(_AStar.GetPointPath(StartIndex, EndIndex));
        }
        return new Array<Vector2I>();
    }

    private void _AddAndConnectPoints(Dictionary<Vector2I, int> CellMappings)
    {
        foreach (var (Key, Value) in CellMappings)
        {
            _AStar.AddPoint(CellMappings[Key], Key);
        }
        foreach (var (Key, Value) in CellMappings)
        {
            foreach (int NeighborIndex in _FindNeighborIndices(Key, CellMappings))
            {
                _AStar.ConnectPoints(CellMappings[Key], NeighborIndex);
            }
        }
    }

    private Array<int> _FindNeighborIndices(Vector2I Cell, Dictionary<Vector2I, int> CellMappings)
    {
        Array<int> Result = new();
        foreach (Vector2I Direction in DIRECTIONS)
        {
            Vector2I Neighbor = Cell + Direction;
            if (!CellMappings.ContainsKey(Neighbor)) { continue; }

            if (!_AStar.ArePointsConnected(CellMappings[Cell], CellMappings[Neighbor]))
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
