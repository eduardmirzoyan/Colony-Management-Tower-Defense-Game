using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldData
{
    public List<RoomData> rooms;
    public UnitData playerData;
    public int NumDiscoveredRooms
    {
        get
        {
            if (rooms == null) return 0;

            int count = 0;
            foreach (RoomData room in rooms)
                if (room.isDiscovered)
                    count++;

            return count;
        }
    }

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

    public void Trace()
    {
        foreach (var room in rooms)
        {
            Debug.Log($"{room}=Adj {room.adjacents.Count}");
        }
    }
}
