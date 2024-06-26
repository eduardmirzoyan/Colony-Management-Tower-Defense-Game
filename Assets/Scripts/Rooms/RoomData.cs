using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomType { None, Standard, Start, Nest, End }

[System.Serializable]
public class RoomData
{
    public Vector2Int gridPosition;
    public int size;
    public Vector3 worldPosition;
    public RoomType roomType;
    public bool isDiscovered;
    public readonly List<RoomData> adjacents;
    public List<UnitData> allies;

    public RoomData(int i, int j, int size, RoomType roomType)
    {
        gridPosition = new Vector2Int(i, j);
        this.size = size;
        worldPosition = new(i * size + size / 2f, j * size + size / 2f);
        this.roomType = roomType;
        isDiscovered = false;
        adjacents = new();
        allies = new();
    }

    public void AddAdjacent(RoomData roomData)
    {
        adjacents.Add(roomData);
    }

    public void AddUnit(UnitData unitData)
    {
        allies.Add(unitData);
    }

    public void RemoveUnit(UnitData unitData)
    {
        allies.Remove(unitData);
    }

    public override string ToString()
    {
        return $"{roomType} Room ({gridPosition.x}, {gridPosition.y})";
    }
}
