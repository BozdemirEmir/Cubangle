using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class CorridorDungeonDictionaryItemPlacement : MonoBehaviour
{
    [Header("References")]
    public CorridorFirstDungeonGenerator cfdg;
    public FightPitRoom fpr;
    public PlayerRoom pr;
    public TreasureRoom tr;





    HashSet<Vector2Int> alreadySpawningPositions = new HashSet<Vector2Int>();

    private List<Vector2Int> needToRemove = new();
    Vector2Int playerRoomPos = new Vector2Int();

    public GameObject player;


    public void PlaceObject()
    {
        alreadySpawningPositions.Clear();
        SpawnPlayer();
        Vector2Int playerPos = new Vector2Int(Mathf.RoundToInt(player.transform.position.x) , Mathf.RoundToInt(player.transform.position.y));
        PlayerRoomPos(playerPos);
        foreach (var structure in pr.itemStructures)
        {
            int numberOfItem = Random.Range(structure.minNumberOfItem, structure.maxNumberOfItem);
            for (int i = 0; i < numberOfItem; i++)
            {
                Vector2Int spawnTransform = FindPlayerRoomSpawnPos(playerPos , structure.itemSO);
                Instantiate(structure.itemSO.objectPrefab, new Vector3(spawnTransform.x, spawnTransform.y, -1f), Quaternion.identity);
                
            }
            
        }
        foreach (var rooms in needToRemove)
        {
            cfdg.roomsDictionary.Remove(rooms);
            cfdg.roomGuidPositions.Remove(rooms);
        }

        HashSet<Vector2Int> treasureRooms = MakeTreasureRoom();
        foreach (var key in treasureRooms)
        {
            HashSet<Vector2Int> treasureRoomPos = cfdg.roomsDictionary[key];
            needToRemove.Add(key);
            foreach (var structure in tr.itemStructures)
            {
                int numberOfItem = Random.Range(structure.minNumberOfItem, structure.maxNumberOfItem);

                for (int i = 0; i < numberOfItem; i++)
                {
                    Vector2Int spawnTransform = FindFightPitsSpawnPos(structure.itemSO, treasureRoomPos);
                    Instantiate(structure.itemSO.objectPrefab, new Vector3(spawnTransform.x, spawnTransform.y, -1f), Quaternion.identity);
                }

            }
        }
        foreach (var rooms in needToRemove)
        {
            cfdg.roomsDictionary.Remove(rooms);
            cfdg.roomGuidPositions.Remove(rooms);
        }

        foreach (var key in cfdg.roomsDictionary.Keys)
        {
            HashSet<Vector2Int> roomPos = cfdg.roomsDictionary[key];
            foreach (var structure in fpr.itemStructures)
            {
                int numberOfItem = Random.Range(structure.minNumberOfItem, structure.maxNumberOfItem);

                for (int i = 0; i < numberOfItem; i++)
                {
                    Vector2Int spawnTransform = FindFightPitsSpawnPos(structure.itemSO, roomPos);
                    Instantiate(structure.itemSO.objectPrefab, new Vector3(spawnTransform.x, spawnTransform.y, -1f), Quaternion.identity);
                }

            }


        }

    }
    private void PlayerRoomPos(Vector2Int playerPos)
    {
        float closestRoomPointToPlayer = float.MaxValue;
        

        foreach (var keyPos in cfdg.roomsDictionary.Keys)
        {
            float distance = Vector2Int.Distance(playerPos, keyPos);

            if (distance < closestRoomPointToPlayer)
            {
                closestRoomPointToPlayer = distance;
                playerRoomPos = keyPos;
            }

        }
    }
    private Vector2Int FindPlayerRoomSpawnPos(Vector2Int playerPos , ItemSO itemSO)
    {
        Vector2Int characterRoomKey = playerRoomPos;
        
        HashSet<Vector2Int> playerRoomTilePos = cfdg.roomsDictionary[characterRoomKey];
        needToRemove.Add(characterRoomKey);
        Vector2Int spawnLoc = FindRealSpawnLocationsPlayerRoom(itemSO , playerRoomTilePos);

        return spawnLoc;

    }

    private HashSet<HashSet<Vector2Int>> FindRooms()
    {
        HashSet<HashSet<Vector2Int>> roomPositions = new HashSet<HashSet<Vector2Int>>();
        foreach (var pos in cfdg.roomGuidPositions)
        {
            if (!roomPositions.Contains(cfdg.roomsDictionary[pos]))
            {
                roomPositions.Add(cfdg.roomsDictionary[pos]);
            }
        }
        return roomPositions;
    }
    private  HashSet<Vector2Int> FindWallTileMap()
    {
        BoundsInt wallBounds = cfdg.tilemapVisualizer.wallTilemap.cellBounds;
        HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();

        foreach (var cell in wallBounds.allPositionsWithin)
        {
            if (cfdg.tilemapVisualizer.wallTilemap.HasTile(cell))
            {
                wallPositions.Add((Vector2Int)cell);
            }
        }
        return wallPositions;
    }

    private Vector2Int FindFightPitsSpawnPos(ItemSO itemSO , HashSet<Vector2Int> spawnLocationArray)
    {
        Vector2Int spawnLocation = FindRealSpawnLocationsFightPit(itemSO , spawnLocationArray);

        return spawnLocation;

    }

    private (HashSet<Vector2Int> nearWall, HashSet<Vector2Int> openSpace) FindSpawnablePositions(HashSet<Vector2Int> roomFloorTilesLocationArray)
    {
        HashSet<Vector2Int> nearWallPositons = new HashSet<Vector2Int>();
        HashSet<Vector2Int> openSpacePositons = new HashSet<Vector2Int>();
        HashSet<Vector2Int> neighbourPoisitons = new HashSet<Vector2Int>();

        var wallTiles = FindWallTileMap();

        Graph graph = new Graph(wallTiles);

        foreach (var tilePositions in roomFloorTilesLocationArray)
        {
            if (cfdg.tilemapVisualizer.floorTilemap.HasTile((Vector3Int)tilePositions))
            {
                neighbourPoisitons = graph.GetNeighbours8Direction((Vector2Int)tilePositions);
                openSpacePositons.Add((Vector2Int)tilePositions);
                if (neighbourPoisitons.Count > 0)
                {
                    nearWallPositons.Add((Vector2Int)tilePositions);
                }
            }

        }

        openSpacePositons.ExceptWith(nearWallPositons);

        return (nearWallPositons, openSpacePositons);
    }

    private Vector2Int ChooseRandomLocation(HashSet<Vector2Int> area)
    {
        int random = Random.Range(0, area.Count);
        Vector2Int spawnLoc = area.ElementAt(random);

        return spawnLoc;
    }
    private HashSet<Vector2Int> BigItemPlacement(ItemSO itemSO)
    {
        int itemArea = itemSO.size.x * itemSO.size.y;
        HashSet<Vector2Int> offsetPositions = new HashSet<Vector2Int>();

        if (itemArea > 1)
        {
            int sizeX = itemSO.size.x;
            int sizeY = itemSO.size.y;

            for (int row = -sizeX; row <= sizeX; row++)
            {
                for (int col = -sizeY; col <= sizeY; col++)
                {
                    Vector2Int offsetPosition = new Vector2Int(row, col);
                    offsetPositions.Add(offsetPosition);
                }
            }
        }
        return null;
    }
    void SpawnPlayer()
    {
        BoundsInt bounds = cfdg.tilemapVisualizer.floorTilemap.cellBounds;
        Vector3Int minYtile = Vector3Int.zero;
        foreach (var tiles in bounds.allPositionsWithin)
        {
            if (minYtile.y > tiles.y && cfdg.tilemapVisualizer.floorTilemap.HasTile(tiles))
            {
                minYtile = tiles;
            }
        }

        player.transform.position = new Vector3(minYtile.x , minYtile.y , player.transform.position.z);

    }
    

    private Vector2Int FindRealSpawnLocationsFightPit(ItemSO itemSO , HashSet<Vector2Int> spawnLocationArray)
    {
        HashSet<Vector2Int> offsetBySize = BigItemPlacement(itemSO);
        var wallTiles = FindWallTileMap();

        Graph graph = new Graph(wallTiles);


        Vector2Int spawnLoc = new Vector2Int();
        var (nearWall, openSpace) = FindSpawnablePositions(spawnLocationArray);
        PlacementType type = itemSO.placementType == PlacementType.NearWall ? PlacementType.NearWall : PlacementType.OpenSpace;

        HashSet<Vector2Int> possibleNeighbourPos = new HashSet<Vector2Int>();


        nearWall.ExceptWith(alreadySpawningPositions);
        openSpace.ExceptWith(alreadySpawningPositions);

        if (type == PlacementType.NearWall)
        {
            spawnLoc = ChooseRandomLocation(nearWall);
            alreadySpawningPositions.Add(spawnLoc);
            return spawnLoc;
        }
        else
        {
            spawnLoc = ChooseRandomLocation(openSpace);
            if (offsetBySize != null)
            {
                foreach (var offsetPos in offsetBySize)
                {
                    possibleNeighbourPos.AddRange(graph.GetNeighbours8Direction(spawnLoc + offsetPos));
                    alreadySpawningPositions.AddRange(graph.GetNeighbours8DirectionWithoutAnyCondition(spawnLoc + offsetPos));

                    HashSet<Vector2Int> addedPos = graph.GetNeighbours8DirectionWithoutAnyCondition(spawnLoc + offsetPos);

                    if (possibleNeighbourPos.Count > 0)
                    {
                        alreadySpawningPositions.ExceptWith(addedPos);
                        return FindFightPitsSpawnPos(itemSO, spawnLocationArray);
                    }

                    if (offsetPos == offsetBySize.ElementAt(offsetBySize.Count - 1) && possibleNeighbourPos.Count == 0)
                    {
                        return spawnLoc;
                    }
                }
            }
            else
            {
                alreadySpawningPositions.Add(spawnLoc);
                return spawnLoc;
            }
        }

        return FindFightPitsSpawnPos(itemSO, spawnLocationArray);
    }
    private Vector2Int FindRealSpawnLocationsPlayerRoom(ItemSO itemSO , HashSet<Vector2Int> locArray)
    {
        Vector2Int playerPos = new Vector2Int(Mathf.RoundToInt(player.transform.position.x), Mathf.RoundToInt(player.transform.position.y));
        HashSet<HashSet<Vector2Int>> roomPositions = FindRooms();
        int randomNumber = Random.Range(0, roomPositions.Count);
        HashSet<Vector2Int> offsetBySize = BigItemPlacement(itemSO);

        var wallTiles = FindWallTileMap();

        Graph graph = new Graph(wallTiles);

        Vector2Int spawnLoc = new Vector2Int();
        var (nearWall, openSpace) = FindSpawnablePositions(locArray);

        nearWall.ExceptWith(alreadySpawningPositions);
        openSpace.ExceptWith(alreadySpawningPositions);

        PlacementType type = itemSO.placementType == PlacementType.NearWall ? PlacementType.NearWall : PlacementType.OpenSpace;

        if (type == PlacementType.NearWall)
        {
            spawnLoc = ChooseRandomLocation(nearWall);
            alreadySpawningPositions.Add(spawnLoc);
            return spawnLoc;
        }
        else
        {
            spawnLoc = ChooseRandomLocation(openSpace);
            if (offsetBySize != null)
            {
                foreach (var offsetPos in offsetBySize)
                {
                    HashSet<Vector2Int> possibleNeighbourPos = graph.GetNeighbours8Direction(spawnLoc + offsetPos);
                    if (possibleNeighbourPos.Count > 0)
                    {
                        return FindPlayerRoomSpawnPos(playerPos, itemSO);
                    }

                    if (offsetPos == offsetBySize.ElementAt(offsetBySize.Count - 1) && possibleNeighbourPos.Count == 0)
                    {
                        foreach (var offsets in offsetBySize)
                        {
                            alreadySpawningPositions.Add(spawnLoc + offsets);
                        }
                        return spawnLoc;
                    }
                }
            }
            else
            {
                alreadySpawningPositions.Add(spawnLoc);
                return spawnLoc;
            }
        }

        return FindPlayerRoomSpawnPos(playerPos, itemSO);
    }

    HashSet<Vector2Int> MakeTreasureRoom()
    {
        HashSet<HashSet<Vector2Int>> treasureRoomPos = new();
        HashSet<Vector2Int> treasureRoomPosKeys = new();
        HashSet<HashSet<Vector2Int>> possibleRoomPositions = FindRooms();


        for (int i = 0; i < cfdg.numberOfTreasureRoom; i++)
        {
            int randomIndex = Random.Range(0, possibleRoomPositions.Count);
            
            var selectedRoom = possibleRoomPositions.ElementAt(randomIndex);
            treasureRoomPos.Add(selectedRoom);

            possibleRoomPositions.Remove(selectedRoom);
        }

        foreach (var key in cfdg.roomsDictionary.Keys)
        {
            foreach (var pos in treasureRoomPos)
            {
                if (cfdg.roomsDictionary[key] == pos)
                {
                    treasureRoomPosKeys.Add(key);
                    break;
                }
            }
            
        }
        return treasureRoomPosKeys;

    }

    
}
