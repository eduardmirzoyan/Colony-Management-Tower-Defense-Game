using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public const float BASE_MOVE_SPEED = 1;

    private enum GameState { Expand, Prepare, Defend }

    [Header("Debug")]
    [SerializeField] private WorldData worldData;
    [SerializeField] private GameState gameState;
    [SerializeField] private float spawnDelay;
    [SerializeField] private int spawnMultiplier;

    [SerializeField] private WaveData waveData;
    [SerializeField] private bool printLogs;

    public static GameManager instance;
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

    private void Start()
    {
        TransitionManager.instance.Initialize();

        // Initialize world
        WorldManager.instance.GenerateWorld(out worldData);
        SpawnManager.instance.Initialize(worldData);
        InitializeSpawners();
        waveData = null;

        DebugManager.instance.Initialize(worldData);

        StartCoroutine(DelayedStart());
    }

    private IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(0.1f);

        DiscoverRoom(worldData.playerData.roomData);
        // EnterRoom(worldData.playerData.roomData);

        GameEvents.instance.TriggerOnEnterRoom(worldData.playerData.roomData);

        GameEvents.instance.TriggerOnGenrateWorld(worldData);

        yield return new WaitForSeconds(1f);

        TransitionManager.instance.OpenScene();
    }

    public void EnterRoom(RoomData roomData)
    {
        //var playerRoom = worldData.playerData.roomData;

        // If entering same room, then do nothing
        if (worldData.playerData.roomData == roomData) return;

        // Leave previous room
        if (worldData.playerData.roomData != null)
            GameEvents.instance.TriggerOnExitRoom(worldData.playerData.roomData);

        // Enter room
        worldData.playerData.roomData = roomData;
        GameEvents.instance.TriggerOnEnterRoom(roomData);
    }

    public void DiscoverRoom(RoomData roomData)
    {
        if (printLogs) print("Enter prepare state");

        // Discover room
        if (!roomData.isDiscovered)
            roomData.isDiscovered = true;

        // Calculate wave
        SetupWave(worldData, out waveData);

        // Now player needs to prepare for wave
        gameState = GameState.Prepare;

        GameEvents.instance.TriggerOnStatePrepare(waveData);
        GameEvents.instance.TriggerOnDiscoverRoom(roomData);

        if (waveData is null)
        {
            // Finish wave
            CompleteWave();
            return;
        }
    }

    public void StartWave()
    {
        if (printLogs) print("Starting Wave: " + waveData);

        // Start spawning
        StartCoroutine(SpawnEnemiesOverTime(waveData, spawnDelay));

        // Now player needs defend against wave
        gameState = GameState.Defend;

        GameEvents.instance.TriggerOnStateDefend();
    }

    public void WaveReduced()
    {
        if (waveData == null) return;

        waveData.numKilled++;
        if (waveData.IsCompleted)
            CompleteWave();
    }

    public void CompleteWave()
    {
        if (printLogs) print("Wave Complete");

        // Do any cleanup
        waveData = null;

        // Now player can attempt to expand again
        gameState = GameState.Expand;

        GameEvents.instance.TriggerOnStateExpand();
    }

    public void GameWin()
    {
        // Win game!
        if (printLogs) print("You win!");

        // Load new level
        TransitionManager.instance.ReloadScene();
    }

    public void GameLose()
    {
        // Win game!
        if (printLogs) print("You Lose!");

        // Load new level
        TransitionManager.instance.ReloadScene();
    }

    #region Helpers

    private void InitializeSpawners()
    {
        FollowerSpawnerHandler[] followerSpawners = (FollowerSpawnerHandler[])FindObjectsOfType(typeof(FollowerSpawnerHandler));
        foreach (var spawner in followerSpawners)
        {
            spawner.Initialize(worldData.playerData.roomData);
        }
    }

    private void SetupWave(WorldData worldData, out WaveData waveData)
    {
        // Find valid rooms to spawn from
        Dictionary<RoomData, int> roomPointAllocationTable = new();
        foreach (var room in worldData.rooms)
        {
            // Each discovered room's non-discovered adjacent rooms are valid rooms
            if (room.isDiscovered)
            {
                // Nest rooms are valid
                if (room.roomType == RoomType.Nest)
                {
                    roomPointAllocationTable[room] = 0;
                }
                // Standard rooms need to be checked
                else if (room.roomType == RoomType.Standard)
                {
                    foreach (var adjacent in room.adjacents)
                        if (!adjacent.isDiscovered)
                            roomPointAllocationTable[adjacent] = 0;
                }
            }
        }

        if (roomPointAllocationTable.Count == 0)
        {
            print("No valid rooms, no enemies will spawn...");
            waveData = null;
            return;
        }

        // Decide how many enemies to spawn (based on number of room discovered)
        int numEnemies = 1; //worldData.NumDiscoveredRooms * spawnMultiplier;
        int numPoints = worldData.NumDiscoveredRooms * spawnMultiplier;

        // Distribute them randomly to each valid room
        int numSpawnRooms = roomPointAllocationTable.Count;
        for (int i = 0; i < numEnemies; i++)
        {
            int randomIndex = Random.Range(0, numSpawnRooms);
            RoomData randomRoom = roomPointAllocationTable.ElementAt(randomIndex).Key;
            roomPointAllocationTable[randomRoom]++; // Increment number to spawn here
        }

        // Convert points to a random enemy type of equivalent value
        Dictionary<RoomData, List<EnemyType>> spawnRoomTable = new();
        for (int i = 0; i < roomPointAllocationTable.Keys.Count; i++)
        {
            var room = roomPointAllocationTable.Keys.ElementAt(i);

            spawnRoomTable[room] = new();

            // Randomly decide on enemy type
            EnemyType randomType = (EnemyType)Random.Range(0, 2); // FIXME

            // Get value of enemy
            int enemyValue = EnemyManager.instance.EnemyValueTable[randomType];

            // Use all points towards enemy
            while (roomPointAllocationTable[room] >= enemyValue)
            {
                spawnRoomTable[room].Add(randomType);
                roomPointAllocationTable[room] -= enemyValue;
            }
        }

        // Create new wave
        waveData = new WaveData(spawnRoomTable);
    }

    private IEnumerator SpawnEnemiesOverTime(WaveData waveData, float spawnDelay)
    {
        int numEnemiesToSpawn = waveData.numEnemies;

        // Spawn 1 enemy from each location over time
        while (numEnemiesToSpawn > 0)
        {
            for (int i = 0; i < waveData.spawnRoomTable.Count; i++)
            {
                var entry = waveData.spawnRoomTable.ElementAt(i);
                if (entry.Value.Count > 0)
                {
                    // Spawn enemy
                    // EnemyManager.instance.SpawnNormal(entry.Key);
                    EnemyManager.instance.Spawn(entry.Value[0], entry.Key);


                    // Decrement
                    entry.Value.RemoveAt(0);
                    //waveData.spawnRoomTable[entry.Key]--;
                    numEnemiesToSpawn--;
                }
            }

            yield return new WaitForSeconds(spawnDelay);
        }
    }

    #endregion
}
