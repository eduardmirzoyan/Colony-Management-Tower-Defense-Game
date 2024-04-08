using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldData
{
    public List<RoomData> rooms;
    public RoomData currentRoom;

    public WorldData()
    {
        rooms = new List<RoomData>();
        currentRoom = null;
    }

    public void AddRoom(RoomData room)
    {
        rooms.Add(room);
    }
}
