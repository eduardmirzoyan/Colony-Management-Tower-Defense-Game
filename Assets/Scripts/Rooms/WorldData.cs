using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldData
{
    public List<RoomData> rooms;
    public UnitData playerData;

    public WorldData()
    {
        rooms = new List<RoomData>();
        playerData = null;
    }

    public void AssignPlayer(UnitData playerData)
    {
        this.playerData = playerData;
    }

    public void AddRoom(RoomData room)
    {
        rooms.Add(room);
    }
}
