using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using NavMeshPlus.Components;

public class WorldManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Tilemap floorTilemap;
    [SerializeField] private Tilemap wallTilemap;
    [SerializeField] private TileBase floorTile;
    [SerializeField] private TileBase wallTile;
    [SerializeField] private NavMeshSurface allyNavMesh;
    [SerializeField] private NavMeshSurface enemyNavMesh;
    [SerializeField] private Transform followersParent;
    [SerializeField] private Transform markersParent;
    [SerializeField] private Transform enemiesParent;

    [Header("Prefabs")]
    [SerializeField] private GameObject roomPrefab;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject basePrefab;
    [SerializeField] private GameObject swordsmanGenPrefab;
    [SerializeField] private GameObject archerGenPrefab;
    [SerializeField] private GameObject enemyGenPrefab;
    [SerializeField] private GameObject finishMarkerPrefab;

    [Header("Data")]
    [SerializeField] private UnitData playerData;
    [SerializeField] private UnitData baseData;

    [Header("Settings")]
    [SerializeField] private int gridWidth;
    [SerializeField] private int gridHeight;
    [SerializeField] private int roomWidth;
    [SerializeField] private int roomHeight;
    [SerializeField] private int numRooms;

    public static WorldManager instance;
    private void Awake()
    {
        // Singleton Logic
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public void GenerateWorld(out WorldData worldData)
    {
        var grid = IssacRoomGeneration.Generate(gridWidth, gridHeight, numRooms);

        worldData = RenderWorld(grid);

        BakeNavMesh();
    }

    private WorldData RenderWorld(int[,] grid)
    {
        // Clear world first
        wallTilemap.ClearAllTiles();
        floorTilemap.ClearAllTiles();

        WorldData worldData = new();
        Dictionary<Vector2Int, RoomData> adjacencyTable = new();

        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                RoomType roomType = (RoomType)grid[i, j];

                if (roomType == RoomType.None)
                {
                    RenderEmpty(i, j);
                    continue;
                }

                // Store data
                RoomData roomData = new(i, j, roomWidth, roomType);
                Instantiate(roomPrefab, wallTilemap.transform).GetComponent<RoomHandler>().Initialize(roomData);
                worldData.AddRoom(roomData);

                // Determine adjacency
                Vector2Int location = new(i, j);
                foreach (var direction in IssacRoomGeneration.DIRECTIONS)
                {
                    Vector2Int newLocation = location + direction;
                    if (adjacencyTable.TryGetValue(newLocation, out RoomData adjacentRoom))
                    {
                        roomData.AddAdjacent(adjacentRoom);
                        adjacentRoom.AddAdjacent(roomData);
                    }
                }
                adjacencyTable.Add(location, roomData);

                // Handle visuals
                RenderRoom(i, j);
                RenderPathway(i, j, grid);
                switch (roomType)
                {
                    case RoomType.Start:
                        InstantiateStartRoom(i, j, roomData, ref worldData);
                        break;
                    case RoomType.Nest:
                        InstantiateNestRoom(i, j);
                        break;
                    case RoomType.End:
                        InstantiateFinishRoom(i, j);
                        break;
                }
            }
        }

        return worldData;
    }

    private void RenderEmpty(int i, int j)
    {
        // Fill with walls
        for (int x = 0; x < roomWidth; x++)
        {
            for (int y = 0; y < roomHeight; y++)
            {
                wallTilemap.SetTile(new(i * roomWidth + x, j * roomHeight + y), wallTile);
            }
        }
    }

    private void RenderRoom(int i, int j)
    {
        // Fill in room
        for (int x = 0; x < roomWidth; x++)
        {
            for (int y = 0; y < roomHeight; y++)
            {
                // Set perimeter to wall
                if (x == 0 || y == 0 || x == roomWidth - 1 || y == roomHeight - 1)
                    wallTilemap.SetTile(new(i * roomWidth + x, j * roomHeight + y), wallTile);
                else
                    floorTilemap.SetTile(new(i * roomWidth + x, j * roomHeight + y), floorTile);
            }
        }
    }

    private void RenderPathway(int i, int j, int[,] grid)
    {
        // Check neighbors
        foreach (var direction in IssacRoomGeneration.DIRECTIONS)
        {
            // Make sure in bounds
            Vector2Int neighbor = new Vector2Int(i, j) + direction;
            if (IssacRoomGeneration.OutOfBounds(neighbor, grid))
                continue;

            // If neighbor not a room, then skip
            if (grid[neighbor.x, neighbor.y] == 0)
                continue;

            // Handle each neighbor
            if (direction == Vector2Int.up)
            {
                for (int x = 3; x < 7; x++)
                {
                    Vector3Int position = new(i * roomWidth + x, j * roomHeight + (roomHeight - 1));
                    wallTilemap.SetTile(position, null);
                    floorTilemap.SetTile(position, floorTile);
                }
            }
            else if (direction == Vector2Int.down)
            {
                for (int x = 3; x < 7; x++)
                {
                    Vector3Int position = new(i * roomWidth + x, j * roomHeight);
                    wallTilemap.SetTile(position, null);
                    floorTilemap.SetTile(position, floorTile);
                }
            }
            else if (direction == Vector2Int.left)
            {
                for (int y = 3; y < 7; y++)
                {
                    Vector3Int position = new(i * roomWidth, j * roomHeight + y);
                    wallTilemap.SetTile(position, null);
                    floorTilemap.SetTile(position, floorTile);
                }
            }
            else if (direction == Vector2Int.right)
            {
                for (int y = 3; y < 7; y++)
                {
                    Vector3Int position = new(i * roomWidth + (roomWidth - 1), j * roomHeight + y);
                    wallTilemap.SetTile(position, null);
                    floorTilemap.SetTile(position, floorTile);
                }
            }
        }
    }

    private void InstantiateStartRoom(int i, int j, RoomData startRoom, ref WorldData worldData)
    {
        // Spawn base in the middle of room
        Vector3 center = new(i * roomWidth + roomWidth / 2, j * roomHeight + roomHeight / 2);
        var baseHandler = Instantiate(basePrefab, center, Quaternion.identity, markersParent).GetComponent<BaseHandler>();
        var copy = baseData.Copy();
        copy.Initialize(baseHandler.transform, startRoom);
        baseHandler.Initialize(copy);
        worldData.AssignBase(copy);


        // Spawn player in bottom middle of room
        Vector3 bottomMiddle = new(center.x, center.y - roomHeight / 4);
        var playerHandler = Instantiate(playerPrefab, bottomMiddle, Quaternion.identity, followersParent).GetComponent<PlayerHandler>();
        copy = playerData.Copy();
        copy.Initialize(playerHandler.transform, startRoom);
        playerHandler.Initialize(copy);
        worldData.AssignPlayer(copy);


        // Spawn swordsman generator in top left corner
        Vector3 topLeft = new(center.x - roomWidth / 4, center.y + roomHeight / 4);
        Instantiate(swordsmanGenPrefab, topLeft, Quaternion.identity, markersParent);

        // Spawn archer generator in top right corner
        Vector3 topRight = new(center.x + roomWidth / 4, center.y + roomHeight / 4);
        Instantiate(archerGenPrefab, topRight, Quaternion.identity, markersParent);
    }

    private void InstantiateNestRoom(int i, int j)
    {
        // Spawn spawner in the middle of room
        Vector3 center = new(i * roomWidth + roomWidth / 2, j * roomHeight + roomHeight / 2);
        Instantiate(enemyGenPrefab, center, Quaternion.identity, markersParent);
    }

    private void InstantiateFinishRoom(int i, int j)
    {
        // Spawn base in the middle of room
        Vector3 center = new(i * roomWidth + roomWidth / 2, j * roomHeight + roomHeight / 2);
        Instantiate(finishMarkerPrefab, center, Quaternion.identity, markersParent);
    }

    public void BakeNavMesh()
    {
        allyNavMesh.BuildNavMesh();
        enemyNavMesh.BuildNavMesh();
    }
}
