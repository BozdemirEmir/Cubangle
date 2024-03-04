using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CorridorFirstDungeonGenerator : SimpleRandomWalkMapGenerator
{
    [SerializeField]
    private int corridorLength = 14, corridorCount = 5, corridorSize;
    
    [SerializeField]
    [Range(0.1f, 1)]
    private float roomPercent = 0.8f;

    [SerializeField]
    internal int numberOfTreasureRoom;

    //PCG Data
    public Dictionary<Vector2Int, HashSet<Vector2Int>> roomsDictionary = new Dictionary<Vector2Int, HashSet<Vector2Int>>();
    private HashSet<Vector2Int> floorPositions;
    [HideInInspector] public List<Vector2Int> floorPosition;
    [HideInInspector] public List<Vector2Int> roomPosition;
    [HideInInspector] public List<Vector2Int> roomGuidPositions;
    private HashSet<Vector2Int> corridorPositions;
    [HideInInspector] public  HashSet<Vector2Int> corridorTilePositions;
    [HideInInspector] public  HashSet<Vector2Int> wallTilePositions;
    public int roomToCreateCount;

    public CorridorDungeonDictionaryItemPlacement dip;



    protected override void RunProceduralGeneration()
    {
        
        CorridorFirstGeneration();
    }

    private void CorridorFirstGeneration()
    {
        foreach (var gameobjects in GameObject.FindGameObjectsWithTag("Item"))
        {
            Destroy(gameobjects);
        }
        floorPositions = new HashSet<Vector2Int>();
        corridorTilePositions = new HashSet<Vector2Int>();
        HashSet<Vector2Int> potentialRoomPositions = new HashSet<Vector2Int>();

        List<List<Vector2Int>> corridors = CreateCorridors(floorPositions, potentialRoomPositions);

        HashSet<Vector2Int> roomPositions = CreateRooms(potentialRoomPositions);

        List<Vector2Int> deadEnds = FindAllDeadEnds(floorPositions);

       
        CreateRoomsAtDeadEnd(deadEnds, roomPositions);

        floorPositions.UnionWith(roomPositions);

        for (int i = 0; i < corridors.Count; i++)
        {
            corridors[i] = IncreaseCorridorSize(corridors[i]);
            corridorTilePositions.UnionWith(corridors[i]);
            floorPositions.UnionWith(corridors[i]);
        }

        wallTilePositions = WallGenerator.FindWallPositions(floorPositions, tilemapVisualizer);
        WallGenerator.CreateWalls(floorPositions, tilemapVisualizer);

        List<Vector2Int> corridorTilePos = new List<Vector2Int>(corridorTilePositions);

        foreach (var pos in corridorTilePos)
        {
            if (floorPositions.Contains(pos))
            {
               floorPositions.Remove(pos);
            }
        }
        
        floorPosition = new List<Vector2Int>(floorPositions);
        roomPosition = new List<Vector2Int>(roomPositions);


        tilemapVisualizer.PaintFloorTiles(floorPosition, tilemapVisualizer.floorTile, tilemapVisualizer.floorTilemap);
        tilemapVisualizer.PaintFloorTiles(corridorTilePositions, tilemapVisualizer.corridorTile, tilemapVisualizer.corridorTilemap);

       
        dip.PlaceObject();
    }

    private List<Vector2Int> IncreaseCorridorSize(List<Vector2Int> corridor)
    {
        List<Vector2Int> newCorridor = new List<Vector2Int>();

        for (int i = 1; i < corridor.Count; i++)
        {
            for (int x = -1; x < corridorSize; x++)
            {
                for (int y = -1; y < corridorSize; y++)
                {
                    newCorridor.Add(corridor[i - 1] + new Vector2Int(x, y));
                }
            }
        }

        return newCorridor;
    }

    private void CreateRoomsAtDeadEnd(List<Vector2Int> deadEnds, HashSet<Vector2Int> roomFloors)
    {
        foreach (var position in deadEnds)
        {
            if (roomFloors.Contains(position) == false)
            {
                var room = RunRandomWalk(randomWalkParameters, position);
                roomFloors.UnionWith(room);
                SaveRoomData(room.First() , room);

            }
        }

    }

    private List<Vector2Int> FindAllDeadEnds(HashSet<Vector2Int> floorPositions)
    {
        List<Vector2Int> deadEnds = new List<Vector2Int>();
        foreach (var position in floorPositions)
        {
            int neighboursCount = 0;
            foreach (var direction in Direction2D.cardinalDirectionsList)
            {
                if (floorPositions.Contains(position + direction))
                    neighboursCount++;

            }
            if (neighboursCount == 1)
                deadEnds.Add(position);
        }
        return deadEnds;
    }

    private HashSet<Vector2Int> CreateRooms(HashSet<Vector2Int> potentialRoomPositions)
    {
        HashSet<Vector2Int> roomPositions = new HashSet<Vector2Int>();
        roomToCreateCount = Mathf.RoundToInt(potentialRoomPositions.Count * roomPercent);
        if (roomToCreateCount > 15)
        {
            roomToCreateCount = 15;
        }

        List<Vector2Int> roomsToCreate = potentialRoomPositions.OrderBy(x => Guid.NewGuid()).Take(roomToCreateCount).ToList();
        ClearRoomData();
        foreach (var roomPosition in roomsToCreate)
        {
            var roomFloor = RunRandomWalk(randomWalkParameters, roomPosition);
            roomGuidPositions.Add(roomPosition);
            SaveRoomData(roomPosition , roomFloor);
            roomPositions.UnionWith(roomFloor);
        }
        return roomPositions;
    }

    private void SaveRoomData(Vector2Int roomPosition, HashSet<Vector2Int> roomFloor)
    {
        roomsDictionary[roomPosition] = roomFloor;
    }

    private void ClearRoomData()
    {
        roomGuidPositions.Clear();
        roomsDictionary.Clear();
    }

    private List<List<Vector2Int>> CreateCorridors(HashSet<Vector2Int> floorPositions, HashSet<Vector2Int> potentialRoomPositions)
    {
        var currentPosition = startPosition;
        potentialRoomPositions.Add(currentPosition);
        List<List<Vector2Int>> corritors = new List<List<Vector2Int>>();

        for (int i = 0; i < corridorCount; i++)
        {
            var corridor = ProceduralGenerationAlgorithms.RandomWalkCorridor(currentPosition, corridorLength);
            corritors.Add(corridor);
            currentPosition = corridor[corridor.Count - 1];
            potentialRoomPositions.Add(currentPosition);
            floorPositions.UnionWith(corridor);
            corridorTilePositions.UnionWith(corridor);
        }
        corridorPositions = new HashSet<Vector2Int>(floorPositions);
        return corritors;
    }
}