using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MinimapManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Tilemap minimapTilemap;
    [SerializeField] private Tilemap wallTilemap;
    [SerializeField] private TileBase blankTile;

    [Header("Settings")]
    [SerializeField] private Color wallColor;
    [SerializeField] private Color activeColor;
    [SerializeField] private Color inactiveColor;
    [SerializeField] private Color unknownColor;

    public static MinimapManager instance;
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
        minimapTilemap.ClearAllTiles();

        GameEvents.instance.OnEnterRoom += EventOnEnter;
        GameEvents.instance.OnExitRoom += EventOnExit;
        GameEvents.instance.OnDiscoverRoom += EventOnDiscover;
    }

    private void OnDestroy()
    {
        GameEvents.instance.OnEnterRoom -= EventOnEnter;
        GameEvents.instance.OnExitRoom -= EventOnExit;
        GameEvents.instance.OnDiscoverRoom -= EventOnDiscover;
    }

    private void EventOnEnter(RoomData roomData)
    {
        for (int i = 0; i < roomData.size; i++)
        {
            for (int j = 0; j < roomData.size; j++)
            {
                Vector3 position = roomData.worldPosition + new Vector3(i - roomData.size / 2f, j - roomData.size / 2f);
                Vector3Int cellPosition = minimapTilemap.WorldToCell(position);
                minimapTilemap.SetTile(cellPosition, blankTile);

                if (wallTilemap.HasTile(cellPosition))
                    minimapTilemap.SetColor(cellPosition, wallColor);
                else
                    minimapTilemap.SetColor(cellPosition, activeColor);

            }
        }
    }

    private void EventOnExit(RoomData roomData)
    {
        for (int i = 0; i < roomData.size; i++)
        {
            for (int j = 0; j < roomData.size; j++)
            {
                Vector3 position = roomData.worldPosition + new Vector3(i - roomData.size / 2f, j - roomData.size / 2f);
                Vector3Int cellPosition = minimapTilemap.WorldToCell(position);
                minimapTilemap.SetTile(cellPosition, blankTile);

                if (wallTilemap.HasTile(cellPosition))
                    minimapTilemap.SetColor(cellPosition, wallColor);
                else
                    minimapTilemap.SetColor(cellPosition, inactiveColor);
            }
        }
    }

    private void EventOnDiscover(RoomData roomData)
    {
        // Gray out room
        foreach (var room in roomData.adjacents)
        {
            if (room.isDiscovered)
                continue;

            for (int i = 0; i < room.size; i++)
            {
                for (int j = 0; j < room.size; j++)
                {
                    Vector3 position = room.worldPosition + new Vector3(i - room.size / 2f, j - room.size / 2f);
                    Vector3Int cellPosition = minimapTilemap.WorldToCell(position);
                    minimapTilemap.SetTile(cellPosition, blankTile);

                    if (wallTilemap.HasTile(cellPosition))
                        minimapTilemap.SetColor(cellPosition, wallColor);
                    else
                        minimapTilemap.SetColor(cellPosition, unknownColor);
                }
            }
        }

        // As if entered
        EventOnExit(roomData);
    }
}
