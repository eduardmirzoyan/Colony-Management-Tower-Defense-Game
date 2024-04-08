using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomType { None, Standard, Start, Nest, End }

[System.Serializable]
public class RoomData
{
    public Vector2Int gridPosition;
    public int size;
    public Vector2 worldPosition;
    public RoomType roomType;
    public bool isDiscovered;

    public RoomData(int i, int j, int size, RoomType roomType)
    {
        gridPosition = new Vector2Int(i, j);
        this.size = size;
        worldPosition = new(i * size + size / 2f, j * size + size / 2f);
        this.roomType = roomType;
        isDiscovered = false;
    }

    public override string ToString()
    {
        return $"Room [{gridPosition}]";
    }
}
