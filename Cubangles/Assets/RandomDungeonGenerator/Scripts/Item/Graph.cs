using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph
{
    private static readonly HashSet<Vector2Int> graphVertices = new HashSet<Vector2Int>();
    private static readonly HashSet<Vector2Int> neighbourOffsets4DirectionsList = new HashSet<Vector2Int>
    {
        new Vector2Int(0, 1),   // UP
        new Vector2Int(1, 0),   // RIGHT
        new Vector2Int(0, -1),  // DOWN
        new Vector2Int(-1, 0)   // LEFT
    };

    private static readonly HashSet<Vector2Int> neighbourOffsets8DirectionsList = new HashSet<Vector2Int>
    {
        new Vector2Int(0, 1),   // UP
        new Vector2Int(1, 1),   // UP-RIGHT
        new Vector2Int(1, 0),   // RIGHT
        new Vector2Int(1, -1),  // RIGHT-DOWN
        new Vector2Int(0, -1),  // DOWN
        new Vector2Int(-1, -1), // DOWN-LEFT
        new Vector2Int(-1, 0),  // LEFT
        new Vector2Int(-1, 1)   // LEFT-UP
    };

    private HashSet<Vector2Int> graph;

    public Graph(IEnumerable<Vector2Int> vertices)
    {
        graph = new HashSet<Vector2Int>(vertices);
    }

    public HashSet<Vector2Int> GetNeighbours4Direction(Vector2Int startPosition)
    {
        return GetNeighbours(startPosition, neighbourOffsets4DirectionsList);
    }

    public HashSet<Vector2Int> GetNeighbours8Direction(Vector2Int startPosition)
    {
        return GetNeighbours(startPosition, neighbourOffsets8DirectionsList);
    }

    public HashSet<Vector2Int> GetNeighbours8DirectionWithoutAnyCondition(Vector2Int startPosition)
    {
        return GetNeighboursWithoutAnyCondition(startPosition, neighbourOffsets8DirectionsList);
    }


    private HashSet<Vector2Int> GetNeighbours(Vector2Int startPosition, HashSet<Vector2Int> neighbourOffsets)
    {
        HashSet<Vector2Int> neighbours = new HashSet<Vector2Int>();

        foreach (var neighbourOffset in neighbourOffsets)
        {
            Vector2Int potentialNeighbour = startPosition + neighbourOffset;

            if (graph.Contains(potentialNeighbour))
            {
                neighbours.Add(potentialNeighbour);
            }
        }

        return neighbours;
    }
    private HashSet<Vector2Int> GetNeighboursWithoutAnyCondition(Vector2Int startPosition, HashSet<Vector2Int> neighbourOffsets)
    {
        HashSet<Vector2Int> neighbours = new HashSet<Vector2Int>();

        foreach (var neighbourOffset in neighbourOffsets)
        {
            Vector2Int potentialNeighbour = startPosition + neighbourOffset;
            neighbours.Add(potentialNeighbour);
        }

        return neighbours;
    }
}






