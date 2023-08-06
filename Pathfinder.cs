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

    public Pathfinder(Grid Grid, Array<Vector2> WalkableCells)
    {
        _Grid = Grid;
        Dictionary<Vector2, int> CellMappings = new();
        foreach (Vector2 Cell in WalkableCells)
        {
            CellMappings[Cell] = _Grid.AsIndex(Cell);
        }
        _AddAndConnectPoints(CellMappings);
    }

    public Vector2[] CalculatePointPath(Vector2 Start, Vector2 End)
    {
        int StartIndex = _Grid.AsIndex(Start);
        int EndIndex = _Grid.AsIndex(End);
        if (_AStar.HasPoint(StartIndex) && _AStar.HasPoint(EndIndex))
        {
            return _AStar.GetPointPath(StartIndex, EndIndex);
        }
        return new Vector2[]{};
    }

    private void _AddAndConnectPoints(Dictionary<Vector2, int> CellMappings)
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

    private Array<int> _FindNeighborIndices(Vector2 Cell, Dictionary<Vector2, int> CellMappings)
    {
        Array<int> Result = new();
        foreach (Vector2 Direction in DIRECTIONS)
        {
            Vector2 Neighbor = Cell + Direction;
            if (!CellMappings.ContainsKey(Neighbor)) { continue; }

            if (!_AStar.ArePointsConnected(CellMappings[Cell], CellMappings[Neighbor]))
            {
                Result.Add(CellMappings[Neighbor]);
            }
        }
        return Result;
    }
}
