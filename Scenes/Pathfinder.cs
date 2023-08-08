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

    private AStar2D _aStar = new();

    public Pathfinder(Grid grid, Array<Vector2I> walkableCells)
    {
        _grid = grid;
        Dictionary<Vector2I, int> cellMappings = new();
        foreach (Vector2I cell in walkableCells)
        {
            cellMappings[cell] = _grid.AsIndex(cell);
        }
        AddAndConnectPoints(cellMappings);
    }

    public Array<Vector2I> CalculatePointPath(Vector2I start, Vector2I end)
    {
        int startIndex = _grid.AsIndex(start);
        int endIndex = _grid.AsIndex(end);

        if (_aStar.HasPoint(startIndex) && _aStar.HasPoint(endIndex))
        {
            return UnpackArray(_aStar.GetPointPath(startIndex, endIndex));
        }

        GD.Print("Error: Failed to get point path from AStar");
        return new Array<Vector2I>();
    }

    private void AddAndConnectPoints(Dictionary<Vector2I, int> cellMappings)
    {
        foreach (Vector2I cell in cellMappings.Keys)
        {
            _aStar.AddPoint(cellMappings[cell], cell);
        }
        foreach (Vector2I cell in cellMappings.Keys)
        {
            foreach (int neighborIndex in FindNeighborIndices(cell, cellMappings))
            {
                _aStar.ConnectPoints(cellMappings[cell], neighborIndex);
            }
        }
    }

    private Array<int> FindNeighborIndices(Vector2I cell, Dictionary<Vector2I, int> cellMappings)
    {
        Array<int> result = new();
        foreach (Vector2I direction in Directions)
        {
            Vector2I neighbor = cell + direction;
            if (!cellMappings.ContainsKey(neighbor)) { continue; }

            if (!_aStar.ArePointsConnected(cellMappings[cell], cellMappings[neighbor]))
            {
                result.Add(cellMappings[neighbor]);
            }
        }
        return result;
    }

    // Convenience function for converting Vector2[]] to Array<Vector2I>.
    // Useful when dealing with AStar, which returns Vector2 values.
    private Array<Vector2I> UnpackArray(Vector2[] input)
    {
            Array<Vector2I> converted = new();
            foreach (Vector2 value in input)
            {
                converted.Add(new Vector2I((int)value.X, (int)value.Y));
            }
            return converted;
    }
}
