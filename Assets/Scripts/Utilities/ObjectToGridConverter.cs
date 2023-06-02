using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectToGridConverter : MonoBehaviour
{
    private GameObject _river;
    private MeshFilter _riverMeshFilter;

    private GameObject _leftRiverBank;
    private MeshFilter _leftRiverBankMeshFilter;
    private GameObject _rightRiverBank;
    private MeshFilter _rightRiverBankMeshFilter;
    public MeshObject[] MeshObjects;

    [System.Serializable]
    public struct MeshObject
    {
        public GameObject MeshHolder;
        public string MeshName;
        public MeshObject(GameObject meshHolder, string meshName)
        {
            MeshHolder = meshHolder;
            MeshName = meshName;
        }
    }
    void Start()
    {
        
    }

    private void AssignComponents()
    {
        _river = GameObject.Find("River");
        _riverMeshFilter = _river.GetComponent<MeshFilter>();

        _rightRiverBank = GameObject.Find("RightRiverSide");
        _rightRiverBankMeshFilter = _rightRiverBank.GetComponent<MeshFilter>();

        _leftRiverBank = GameObject.Find("LeftRiverSide");
        _leftRiverBankMeshFilter = _leftRiverBank.GetComponent<MeshFilter>();
    }

    public void IntegrateRiverMesh()
    {
        AssignComponents();
       
        IntegrateMesh(_leftRiverBankMeshFilter, Tile.TileType.Ground, _leftRiverBank);
        IntegrateMesh(_rightRiverBankMeshFilter, Tile.TileType.Ground, _rightRiverBank);
         IntegrateMesh(_riverMeshFilter, Tile.TileType.Water, _river);
    }

    public void IntegrateMeshByName(string targetMeshName, Tile.TileType tileType)
    {
        foreach (MeshObject meshObject in MeshObjects)
        {
            if (meshObject.MeshName == targetMeshName)
            {
                MeshFilter meshFilter = meshObject.MeshHolder.GetComponent<MeshFilter>();
                GameObject meshHolder = meshObject.MeshHolder;

                if (meshFilter != null)
                {
                    IntegrateMesh(meshFilter, tileType, meshHolder);
                }
                else
                {
                    Debug.LogWarning("MeshFilter component not found on the MeshHolder GameObject.");
                }

                // If you want to integrate only the first matching mesh, break the loop after calling IntegrateMesh()
                break;
            }
        }
    }

    public void IntegrateMesh(MeshFilter meshFilter, Tile.TileType tileType, GameObject meshHolder)
    {
        // Create a temporary mesh collider for intersection tests
        MeshCollider meshCollider = meshHolder.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = meshFilter.sharedMesh;

        // Iterate through grid cells
        for (int x = 0; x < GridManager.Instance.GridWidth; x++)
        {
            for (int y = 0; y < GridManager.Instance.GridHeight; y++)
            {
                Vector3 gridWorldPos = GridSpaceToWorldSpace(new Vector2Int(x, y));

                // Check if the grid cell is inside the mesh using a mesh collider intersection test
                if (IsPointInsideMesh(gridWorldPos, meshCollider))
                {
                    GridManager.Instance.TileGrid[x, y] = new Tile(tileType, 0);
                }
            }
        }

        // Remove the temporary mesh collider
        Destroy(meshCollider);
    }


    bool IsPointInsideMesh(Vector3 point, MeshCollider meshCollider)
    {
        Vector3 rayDirection = Vector3.down;
        Vector3 rayOrigin = point + new Vector3(0, 0.01f, 0);
        int intersectionCount = 0;
        float rayDistance = Mathf.Infinity;
        //Debug.DrawRay(rayOrigin, rayDirection * 100, Color.red, 5.0f);
        while (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, rayDistance))
        {
            if (hit.collider == meshCollider)
            {

                intersectionCount++;
                rayDistance = hit.distance + 0.01f;
            }
            else
            {
                break;
            }

            rayOrigin = hit.point + rayDirection * 0.01f;
        }

        return intersectionCount % 2 == 1;

    }

    void Update()
    {

    }

    Vector2Int WorldSpaceToGridSpace(Vector3 worldPosition)
    {
        float tileSize = 1.0f; // Adjust this value based on the size of your tiles
        int x = Mathf.RoundToInt(worldPosition.x / tileSize);
        int y = Mathf.RoundToInt(worldPosition.z / tileSize);
        return new Vector2Int(x, y);
    }

    Vector3 GridSpaceToWorldSpace(Vector2Int gridPosition)
    {
        float tileSize = 1.0f;
        return new Vector3(gridPosition.x * tileSize, 0, gridPosition.y * tileSize);
    }
}

