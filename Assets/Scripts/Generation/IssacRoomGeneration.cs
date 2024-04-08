using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IssacRoomGeneration
{
    public static Vector2Int[] DIRECTIONS = new Vector2Int[4] { Vector2Int.left, Vector2Int.up, Vector2Int.right, Vector2Int.down };

    public static int[,] Generate(int gridWidth, int gridHeight, int numRooms)
    {
        var skeleton = GenerateSkeleton(gridWidth, gridHeight, numRooms);

        return GenerateGrid(skeleton);
    }

    public static int[,] GenerateGrid(int[,] skeleton)
    {
        int width = skeleton.GetLength(0);
        int height = skeleton.GetLength(1);

        // 0 - Empty | 1 - Room | 2 - Start | 3 - Nest | 4 - Finish
        int[,] grid = new int[width, height];

        int farthestDistance = 0;
        Vector2Int farthestRoom = new();

        Vector2Int start = new(width / 2, height / 2);

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (skeleton[i, j] == 1) // If we are looking at a normal room
                {
                    // If only one neighbor, then nest
                    if (GetNumNeighbors(new(i, j), skeleton) == 1)
                    {
                        grid[i, j] = 3;

                        int distance = ManhattanDistance(start, new(i, j));
                        if (distance > farthestDistance)
                        {
                            farthestRoom = new(i, j);
                            farthestDistance = distance;
                        }
                    }
                    else
                    {
                        grid[i, j] = 1;
                    }
                }
                else
                {
                    grid[i, j] = skeleton[i, j];
                }
            }
        }

        // Set farthest nest to the exit
        grid[farthestRoom.x, farthestRoom.y] = 4;

        return grid;
    }

    private static int[,] GenerateSkeleton(int rows, int cols, int numRooms)
    {
        // Start location is center of grid
        Vector2Int startLocation = new(rows / 2, cols / 2);

        // Queue of rooms to expand from
        Queue<Vector2Int> roomQueue;

        // 0 == Empty | 1 == Room
        int[,] grid;

        // Number of rooms to carve out
        int numRoomsLeft;
        do
        {
            // Init values
            grid = new int[rows, cols];
            roomQueue = new Queue<Vector2Int>();

            // Set up start
            grid[startLocation.x, startLocation.y] = 2;
            roomQueue.Enqueue(startLocation);
            numRoomsLeft = numRooms - 1;

            // Recurisvely generate grid
            BFS(roomQueue, grid, ref numRoomsLeft);

        } while (numRoomsLeft != 0);

        return grid;
    }

    private static void BFS(Queue<Vector2Int> roomQueue, int[,] grid, ref int numRoomsLeft)
    {
        if (roomQueue.Count == 0 || numRoomsLeft <= 0)
            return;

        var location = roomQueue.Dequeue();

        // Check each valid neighbor
        foreach (var neighbor in GetValidNeighbors(location, grid))
        {
            // 50% chance of exploring room
            if (Random.Range(0, 100) > 50)
            {
                // Add to queue
                numRoomsLeft--;
                grid[neighbor.x, neighbor.y] = 1;
                roomQueue.Enqueue(neighbor);
            }
        }

        // Recursively call
        BFS(roomQueue, grid, ref numRoomsLeft);
    }

    private static Vector2Int[] GetValidNeighbors(Vector2Int location, int[,] grid)
    {
        List<Vector2Int> validNeighbors = new List<Vector2Int>();

        foreach (var direction in DIRECTIONS)
        {
            var newLocation = location + direction;

            // Skip if out of bounds
            if (OutOfBounds(newLocation, grid))
            {
                continue;
            }

            bool validNeighbor = true;
            foreach (var neighborDirection in DIRECTIONS)
            {
                var newNewLocation = newLocation + neighborDirection;

                // Skip if out of bounds or same as start
                if (OutOfBounds(newNewLocation, grid) || newNewLocation == location)
                {
                    continue;
                }

                // If neighbor has another neighbor that isn't original
                if (grid[newNewLocation.x, newNewLocation.y] != 0)
                {
                    // Then invalid location
                    validNeighbor = false;
                    break;
                }
            }

            // Add to valid neighbors
            if (validNeighbor)
            {
                validNeighbors.Add(newLocation);
            }
        }

        return validNeighbors.ToArray();
    }

    private static int GetNumNeighbors(Vector2Int location, int[,] grid)
    {
        int count = 0;
        foreach (var direction in DIRECTIONS)
        {
            var newLocation = location + direction;

            if (OutOfBounds(newLocation, grid))
                continue;

            // If not empty
            if (grid[newLocation.x, newLocation.y] != 0)
                count++;
        }

        return count;
    }

    public static bool OutOfBounds(Vector2Int location, int[,] grid)
    {
        return location.x < 0 || location.y < 0 || location.x >= grid.GetLength(0) || location.y >= grid.GetLength(1);
    }

    public static int ManhattanDistance(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }
}
