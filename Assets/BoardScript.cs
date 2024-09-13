using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardScript : MonoBehaviour
{
    public GameObject TilePrefab;

    private void Awake()
    {
        SetupCamera();
    }

    void Start()
    {
        DistributeBoardTiles();
        DistributeGridCoordinates();
        DistributeSpecialTiles();
    }

    void SetupCamera()
    {
        Camera.main.transform.position = new Vector3(11, 11, Camera.main.transform.position.z);
        Camera.main.orthographicSize = 8;
    }

    void DistributeBoardTiles()
    {
        for (int i = 0; i < 256; i++)
        {
            float y = i / 16;
            float x = i % 16;
            string tileName = x + "x" + y;

            GameObject newTile = Instantiate(TilePrefab, new Vector3(x + 3, y + 3), Quaternion.identity);
            newTile.name = tileName;
        }
    }

    void DistributeGridCoordinates()
    {
        for (int i = 0; i < 256; i++)
        {
            float tileY = i / 16;
            float tileX = i % 16;

            GameObject tile = GameObject.Find((tileX) + "x" + (tileY));
            if (tile != null)
            {
                TileReference tileReference = tile.GetComponent<TileReference>();

                if (tileX == 0 && tileY != 15)
                {
                    tileReference.Text.text = (15 - tileY).ToString();
                }
                else if (tileX != 0 && tileY == 15)
                {
                    tileReference.Text.text = ((char)(tileX + 64)).ToString();
                }

                if (tileX == 0 || tileY == 15)
                {
                    tileReference.SquareRenderer.GetComponent<SpriteRenderer>().enabled = false;
                    tileReference.Text.color = new Color(0, 0, 0);
                }
            }
        }
    }

    void DistributeSpecialTiles()
    {
        // Positions for Double Word Score
        int[,] dwsPositions = new int[,] {
        {0, 0}, {7, 0}, {14, 0}, {0, 7}, {14, 7}, {0, 14}, {7, 14}, {14, 14}
    };

        // Positions for Triple Word Score
        int[,] twsPositions = new int[,] {
        {0, 0}, {0, 14}, {14, 0}, {14, 14}
    };

        // Positions for Double Letter Score
        int[,] dlsPositions = new int[,] {
        {3, 0}, {11, 0}, {6, 2}, {8, 2}, {0, 3}, {14, 3}, {2, 6}, {6, 6},
        {8, 6}, {12, 6}, {3, 7}, {11, 7}, {2, 8}, {6, 8}, {8, 8}, {12, 8},
        {0, 11}, {14, 11}, {6, 12}, {8, 12}, {3, 14}, {11, 14}
    };

        // Positions for Triple Letter Score
        int[,] tlsPositions = new int[,] {
        {5, 1}, {9, 1}, {1, 5}, {5, 5}, {9, 5}, {13, 5}, {1, 9}, {5, 9},
        {9, 9}, {13, 9}, {5, 13}, {9, 13}
    };

        for (int i = 0; i < dwsPositions.GetLength(0); i++)
        {
            int x = dwsPositions[i, 0] + 1;
            int y = dwsPositions[i, 1];
            PlaceSpecialTile(x, y, "DWS");
        }

        for (int i = 0; i < twsPositions.GetLength(0); i++)
        {
            int x = twsPositions[i, 0] + 1;
            int y = twsPositions[i, 1];
            PlaceSpecialTile(x, y, "TWS");
        }

        for (int i = 0; i < dlsPositions.GetLength(0); i++)
        {
            int x = dlsPositions[i, 0] + 1;
            int y = dlsPositions[i, 1];
            PlaceSpecialTile(x, y, "DLS");
        }

        for (int i = 0; i < tlsPositions.GetLength(0); i++)
        {
            int x = tlsPositions[i, 0] + 1;
            int y = tlsPositions[i, 1];
            PlaceSpecialTile(x, y, "TLS");
        }
    }


    void PlaceSpecialTile(int x, int y, string tileType)
    {
        GameObject tile = GameObject.Find(x + "x" + y);
        if (tile != null)
        {
            TileReference tileReference = tile.GetComponent<TileReference>();
            tileReference.TileType = tileType;
            tileReference.Text.text = tileReference.TileType;
            tileReference.Text.color = GetTileColor(tileType);
            tileReference.AccentSquareRenderer.GetComponent<SpriteRenderer>().enabled = true;
            tileReference.AccentSquareRenderer.GetComponent<SpriteRenderer>().color = GetTileColor(tileType);
            Debug.Log($"Placed {tileType} at {x}x{y}");
        }
    }

    Color GetTileColor(string tileType)
    {
        switch (tileType)
        {
            case "DWS": return Color.red;   // Double Word Score - Red
            case "TWS": return Color.blue;  // Triple Word Score - Blue
            case "DLS": return Color.cyan;  // Double Letter Score - Cyan
            case "TLS": return Color.green; // Triple Letter Score - Green
            default: return Color.white;    // Default color for other tiles
        }
    }
}
