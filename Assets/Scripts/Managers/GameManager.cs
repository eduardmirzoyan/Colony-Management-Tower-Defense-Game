using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private enum GameState { Expand, Prepare, Defend }

    [Header("Debug")]
    [SerializeField] private WorldData worldData;
    [SerializeField] private GameState gameState;
    [SerializeField] private float spawnDelay;
    [SerializeField] private int spawnMultiplier;

    [SerializeField] private WaveData waveData;

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
        EnterRoom(worldData.playerData.roomData);

        GameEvents.instance.TriggerOnGenrateWorld(worldData);

        yield return new WaitForSeconds(1f);

        TransitionManager.instance.OpenScene();
    }

    public void EnterRoom(RoomData roomData)
    {
        // Leave previous room
        if (worldData.playerData.roomData != null)
            GameEvents.instance.TriggerOnExitRoom(worldData.playerData.roomData);

        // Check if room was already discovered
        if (!roomData.isDiscovered)
            roomData.isDiscovered = true;

        // Enter room
        worldData.playerData.roomData = roomData;
        GameEvents.instance.TriggerOnEnterRoom(roomData);
    }

    public void DiscoverRoom(RoomData roomData)
    {
        // if (gameState != GameState.Expand) throw new System.Exception($"ATTEMPTING EXPANSION WHEN GAME IS IN STATE: {gameState}");

        print("Begin to prepare...");

        // Discover room
        if (!roomData.isDiscovered)
            roomData.isDiscovered = true;

        // Calculate wave
        SetupWave(out waveData);
        if (waveData is null)
        {
            // TODO
            // Finish wave
            // CompleteWave();
            // return;
        }

        // Now player needs to prepare for wave
        gameState = GameState.Prepare;

        GameEvents.instance.TriggerOnStatePrepare();
        GameEvents.instance.TriggerOnDiscoverRoom(roomData);
    }

    public void StartWave()
    {
        // if (gameState != GameState.Expand) throw new System.Exception($"ATTEMPTING WAVE START WHEN GAME IS IN STATE: {gameState}");

        print("Starting Wave...");
        print(waveData);

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
        // if (gameState != GameState.Expand) throw new System.Exception($"ATTEMPTING WAVE END WHEN GAME IS IN STATE: {gameState}");

        print("Wave Complete...");

        // Do any cleanup
        waveData = null;

        // Now player can attempt to expand again
        gameState = GameState.Expand;

        GameEvents.instance.TriggerOnStateExpand();
    }

    public void GameWin()
    {
        // Win game!
        print("You win!");

        // Load new level
        TransitionManager.instance.ReloadScene();
    }

    public void GameLose()
    {
        // Win game!
        print("You Lose!");

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

    private void SetupWave(out WaveData waveData)
    {
        // Decide how many enemies to spawn (based on number of room discovered)
        int numEnemies = worldData.NumDiscoveredRooms * spawnMultiplier;

        // Find valid rooms to generate
        Dictionary<RoomData, int> spawnRoomTable = new();
        foreach (var room in worldData.rooms)
        {
            // Each discovered room's non-discovered adjacent rooms are valid rooms
            if (room.isDiscovered)
            {
                // Nest rooms are valid
                if (room.roomType == RoomType.Nest)
                {
                    spawnRoomTable[room] = 0;
                }
                // Standard rooms need to be checked
                else if (room.roomType == RoomType.Standard)
                {
                    foreach (var adjacent in room.adjacents)
                        if (!adjacent.isDiscovered)
                            spawnRoomTable[adjacent] = 0;
                }
            }
        }

        if (spawnRoomTable.Count == 0)
        {
            print("No valid rooms, no enemies will spawn...");
            waveData = null;
            return;
        }

        // Distribute them randomly to each valid room
        int numSpawnRooms = spawnRoomTable.Count;
        for (int i = 0; i < numEnemies; i++)
        {
            int randomIndex = Random.Range(0, numSpawnRooms);
            RoomData randomRoom = spawnRoomTable.ElementAt(randomIndex).Key;
            spawnRoomTable[randomRoom]++; // Increment number to spawn here
        }

        // Create new wave
        waveData = new WaveData(numEnemies, spawnRoomTable);
    }

    private IEnumerator SpawnEnemiesOverTime(WaveData waveData, float spawnDelay)
    {
        print("Started spawning enemies...");

        int numEnemiesToSpawn = waveData.numEnemies;

        // Spawn 1 enemy from each location over time
        while (numEnemiesToSpawn > 0)
        {
            for (int i = 0; i < waveData.spawnRoomTable.Count; i++)
            {
                var entry = waveData.spawnRoomTable.ElementAt(i);
                if (entry.Value > 0)
                {
                    // Spawn enemy
                    EnemyManager.instance.Spawn(entry.Key);

                    // Decrement
                    waveData.spawnRoomTable[entry.Key]--;
                    numEnemiesToSpawn--;
                }
            }

            yield return new WaitForSeconds(spawnDelay);
        }

        print("Finished spawning enemies...");
    }

    #endregion
}
