using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private WorldData worldData;

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
        // Create world
        WorldManager.instance.GenerateWorld(out worldData);

        StartCoroutine(DelayedStart());
    }

    private IEnumerator DelayedStart()
    {
        TransitionManager.instance.Initialize();

        yield return new WaitForSeconds(0.1f);

        DiscoverRoom(worldData.currentRoom);
        EnterRoom(worldData.currentRoom);

        yield return new WaitForSeconds(1f);

        TransitionManager.instance.OpenScene();
    }

    public void EnterRoom(RoomData roomData)
    {
        // Leave previous room
        if (worldData.currentRoom != null)
            GameEvents.instance.TriggerOnExitRoom(worldData.currentRoom);

        // Check if room was already discovered
        if (!roomData.isDiscovered)
            roomData.isDiscovered = true;

        // Enter room
        worldData.currentRoom = roomData;
        GameEvents.instance.TriggerOnEnterRoom(roomData);
    }

    public void DiscoverRoom(RoomData roomData)
    {
        // Discover room
        if (!roomData.isDiscovered)
            roomData.isDiscovered = true;

        GameEvents.instance.TriggerOnDiscoverRoom(roomData);
    }

    public void ExitMap()
    {
        // Win game!
        print("You win!");

        // Load new level
        TransitionManager.instance.ReloadScene();
    }
}