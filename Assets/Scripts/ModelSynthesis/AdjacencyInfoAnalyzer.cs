using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ModelTile
{
    public GameObject gameObject;
    public string type;
}
public class AdjacencyInfoAnalyzer : MonoBehaviour
{
    
    // Assuming you have an enum for directions like this
    public enum Direction { North, East, South, West }

    public ModelTile[] tiles = new ModelTile[16];

    Dictionary<string, Dictionary<Direction, HashSet<string>>> adjacencyDict = new Dictionary<string, Dictionary<Direction, HashSet<string>>>();

    void Start()
    {
        AnalyzeAdjacency();
    }

   void AnalyzeAdjacency()
{
    for (int i = 0; i < tiles.Length; i++)
    {
        string tileType = tiles[i].type;

        // Initialize the dictionaries for this tile type
        if (!adjacencyDict.ContainsKey(tileType))
        {
            adjacencyDict[tileType] = new Dictionary<Direction, HashSet<string>>();
            foreach (Direction dir in System.Enum.GetValues(typeof(Direction)))
            {
                adjacencyDict[tileType][dir] = new HashSet<string>();
            }
        }

        // Check each direction
        foreach (Direction dir in System.Enum.GetValues(typeof(Direction)))
        {
            int adjacentIndex = GetAdjacentIndex(i, dir);
            if (adjacentIndex >= 0 && adjacentIndex < tiles.Length)
            {
                string adjacentTileType = tiles[adjacentIndex].type;
                adjacencyDict[tileType][dir].Add(adjacentTileType);
            }
        }
    }
}


    // Given an index in the 1D array and a direction, returns the index of the tile in that direction
    int GetAdjacentIndex(int index, Direction dir)
    {
        int x = index % 4;
        int y = index / 4;

        switch (dir)
        {
            case Direction.North:
                y++;
                break;
            case Direction.East:
                x++;
                break;
            case Direction.South:
                y--;
                break;
            case Direction.West:
                x--;
                break;
        }

        if (x < 0 || x >= 4 || y < 0 || y >= 4) return -1; // out of bounds

        return y * 4 + x;
    }
}
