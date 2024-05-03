using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WaveData
{
    public int numEnemies;
    public int numKilled;
    public bool IsCompleted { get { return numKilled >= numEnemies; } }
    public Dictionary<RoomData, List<EnemyType>> spawnRoomTable;

    public WaveData()
    {
        numEnemies = 0;
        spawnRoomTable = new();
    }

    public WaveData(Dictionary<RoomData, List<EnemyType>> spawnRoomTable)
    {
        this.spawnRoomTable = spawnRoomTable;

        numEnemies = 0;
        foreach (var list in spawnRoomTable.Values)
            numEnemies += list.Count;
    }

    public override string ToString()
    {
        string roomText = "";
        foreach (RoomData room in spawnRoomTable.Keys)
            roomText += $"{room}, ";

        return $"Wave; NUMENEMIES={numEnemies}; NUMPOINTS={spawnRoomTable.Count}; SPAWNPOINTS=[{roomText}]";
    }
}
