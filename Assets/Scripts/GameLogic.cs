using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GameLogic
{
    public static void AttackUnit(UnitData attacker, UnitData victim)
    {
        victim.currentHP -= attacker.attackDamage;
        GameEvents.instance.TriggerOnUnitTakeDamage(victim);

        if (victim.currentHP <= 0)
        {
            victim.currentHP = 0;
            victim.transform = null;
            GameEvents.instance.TriggerOnUnitDie(victim);
        }
    }

    public static void AssignUnitToRoom(UnitData unitData, RoomData roomData)
    {
        if (roomData != null)
        {
            // Add to new room
            roomData.AddUnit(unitData);
            GameEvents.instance.TriggerOnUnitAssign(unitData, roomData);
        }
        else
        {
            // Remove self from current room
            unitData.roomData?.RemoveUnit(unitData);
            GameEvents.instance.TriggerOnUnitAssign(unitData, unitData.roomData);
        }

        unitData.roomData = roomData;
    }

    public static void GenerateWave(WorldData worldData, out WaveData waveData)
    {
        // Find valid rooms to spawn from
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
            //print("No valid rooms, no enemies will spawn...");
            waveData = null;
            return;
        }

        // Decide how many enemies to spawn (based on number of room discovered)
        int numEnemies = 1; //worldData.NumDiscoveredRooms * spawnMultiplier;
        int numPoints = worldData.NumDiscoveredRooms * 1;

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
}
