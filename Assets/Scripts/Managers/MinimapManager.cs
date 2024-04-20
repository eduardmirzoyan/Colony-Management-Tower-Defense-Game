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

    [Header("Walls")]
    [SerializeField] private Color wallColor;

    [Header("Standard room")]
    [SerializeField] private Color defaultActiveColor;
    [SerializeField] private Color defaultInactiveColor;

    [Header("Base room")]
    [SerializeField] private Color startActiveColor;
    [SerializeField] private Color startInactiveColor;

    [Header("Nest room")]
    [SerializeField] private Color nestActiveColor;
    [SerializeField] private Color nestInactiveColor;

    [Header("Exit room")]
    [SerializeField] private Color endActiveColor;
    [SerializeField] private Color endInactiveColor;


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
                {
                    minimapTilemap.SetColor(cellPosition, wallColor);
                    continue;
                }

                Color color = roomData.roomType switch
                {
                    RoomType.Start => startActiveColor,
                    RoomType.Nest => nestActiveColor,
                    RoomType.End => endActiveColor,
                    _ => defaultActiveColor
                };
                minimapTilemap.SetColor(cellPosition, color);
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
                {
                    minimapTilemap.SetColor(cellPosition, wallColor);
                    continue;
                }

                Color color = roomData.roomType switch
                {
                    RoomType.Start => startInactiveColor,
                    RoomType.Nest => nestInactiveColor,
                    RoomType.End => endInactiveColor,
                    _ => defaultInactiveColor
                };
                minimapTilemap.SetColor(cellPosition, color);
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
                    {
                        minimapTilemap.SetColor(cellPosition, wallColor);
                        continue;
                    }

                    Color color = room.roomType switch
                    {
                        RoomType.Start => startInactiveColor,
                        RoomType.Nest => nestInactiveColor,
                        RoomType.End => endInactiveColor,
                        _ => defaultInactiveColor
                    };
                    minimapTilemap.SetColor(cellPosition, color);
                }
            }
        }

        // As if entered
        EventOnExit(roomData);
    }
}
