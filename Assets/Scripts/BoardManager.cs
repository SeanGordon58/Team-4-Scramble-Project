using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    // Prefab for creating board tile GameObjects
    public GameObject tilePrefab;

    // The size of the board (e.g., 15x15)
    public int boardSize = 15;

    // Reference to the TileManager
    public TileManager tileManager;

    // Reference to the GameUIManager
    public GameUIManager gameUIManager;

    // 2D array to store the visual tiles on the board
    private GameObject[,] boardTiles;

    // 2D array to store the data for each tile on the board
    private Tile[,] boardTilesData;

    // List of positions where tiles were placed this turn
    public List<Vector2Int> tilesPlacedThisTurn = new List<Vector2Int>();

    private void Start()
    {
        InitializeBoardLayout(); // Initialize the board
        UpdateUIForCurrentPlayer(); // Update UI for the current player
    }

    // Initializes the board layout and assigns special tiles
    void InitializeBoardLayout()
    {
        boardTiles = new GameObject[boardSize, boardSize]; // Initialize visual tiles array
        boardTilesData = new Tile[boardSize, boardSize];   // Initialize data tiles array

        for (int y = 0; y < boardSize; y++)
        {
            for (int x = 0; x < boardSize; x++)
            {
                // Instantiate the tile prefab
                GameObject newTileObject = Instantiate(tilePrefab, new Vector3(x, y, 0f), Quaternion.identity, transform);
                newTileObject.name = $"{x}x{y}";

                // Get the Tile component
                Tile tileComponent = newTileObject.GetComponent<Tile>();

                // Initialize the tile as needed
                tileComponent.TileType = "";
                tileComponent.Letter = '\0';
                tileComponent.PointValue = 0;
                tileComponent.IsOccupied = false;
                tileComponent.PlacedThisTurn = false;

                // Store references
                boardTiles[x, y] = newTileObject;
                boardTilesData[x, y] = tileComponent;
            }
        }

        // Assign special tiles (e.g., DWS, TWS, etc.)
        AssignSpecialTiles();
    }

    // Assigns special tiles to the board based on standard positions
    void AssignSpecialTiles()
    {
        // Triple Word Score (TWS) positions
        int[,] TWSPositions = new int[,]
        {
            {0, 0}, {0, 7}, {0, 14},
            {7, 0}, {7, 14},
            {14, 0}, {14, 7}, {14, 14}
        };

        // Double Word Score (DWS) positions
        int[,] DWSPositions = new int[,]
        {
            {1, 1}, {2, 2}, {3, 3}, {4, 4},
            {10, 10}, {11, 11}, {12, 12}, {13, 13},
            {1, 13}, {2, 12}, {3, 11}, {4, 10},
            {10, 4}, {11, 3}, {12, 2}, {13, 1}
        };

        // Triple Letter Score (TLS) positions
        int[,] TLSPositions = new int[,]
        {
            {5, 1}, {9, 1},
            {1, 5}, {5, 5}, {9, 5}, {13, 5},
            {5, 9}, {9, 9},
            {1, 9}, {5, 13}, {9, 13}, {13, 9}
        };

        // Double Letter Score (DLS) positions
        int[,] DLSPositions = new int[,]
        {
            {3, 0}, {11, 0},
            {6, 2}, {8, 2},
            {0, 3}, {7, 3}, {14, 3},
            {2, 6}, {6, 6}, {8, 6}, {12, 6},
            {3, 7}, {11, 7},
            {2, 8}, {6, 8}, {8, 8}, {12, 8},
            {0, 11}, {7, 11}, {14, 11},
            {6, 12}, {8, 12},
            {3, 14}, {11, 14}
        };

        // Set TWS tiles
        for (int i = 0; i < TWSPositions.GetLength(0); i++)
        {
            int x = TWSPositions[i, 0];
            int y = TWSPositions[i, 1];
            SetSpecialTile(x, y, "TWS");
        }

        // Set DWS tiles
        for (int i = 0; i < DWSPositions.GetLength(0); i++)
        {
            int x = DWSPositions[i, 0];
            int y = DWSPositions[i, 1];
            SetSpecialTile(x, y, "DWS");
        }

        // Set TLS tiles
        for (int i = 0; i < TLSPositions.GetLength(0); i++)
        {
            int x = TLSPositions[i, 0];
            int y = TLSPositions[i, 1];
            SetSpecialTile(x, y, "TLS");
        }

        // Set DLS tiles
        for (int i = 0; i < DLSPositions.GetLength(0); i++)
        {
            int x = DLSPositions[i, 0];
            int y = DLSPositions[i, 1];
            SetSpecialTile(x, y, "DLS");
        }

        // Set the center tile
        SetSpecialTile(7, 7, "Center");
    }

    // Sets a special tile at a given position
    void SetSpecialTile(int x, int y, string tileType)
    {
        Tile tile = boardTilesData[x, y];
        if (tile != null)
        {
            tile.TileType = tileType;
            tile.UpdateVisuals();
        }
        else
        {
            Debug.LogError($"Tile at position ({x}, {y}) is null.");
        }
    }

    // Updates the UI for the current player
    public void UpdateUIForCurrentPlayer()
    {
        Player currentPlayer = tileManager.GetCurrentPlayer();

        gameUIManager.DisplayUIForPlayer(currentPlayer);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int gridPosition = new Vector2Int(Mathf.RoundToInt(mouseWorldPosition.x), Mathf.RoundToInt(mouseWorldPosition.y));

            Debug.Log($"Mouse World Position: {mouseWorldPosition}, Grid Position: {gridPosition}");

            if (IsValidGridPosition(gridPosition))
            {
                PlaceTileOnBoard(gridPosition);
            }
        }
    }

    // Checks if a grid position is valid within the board
    bool IsValidGridPosition(Vector2Int position)
    {
        return position.x >= 0 && position.x < boardSize && position.y >= 0 && position.y < boardSize;
    }

    // Attempts to place a tile on the board at the specified grid position
    void PlaceTileOnBoard(Vector2Int gridPosition)
    {
        Player currentPlayer = tileManager.GetCurrentPlayer();
        Tile selectedTile = gameUIManager.GetSelectedTile();

        if (selectedTile == null)
        {
            Debug.Log("No tile selected.");
            return;
        }

        Tile boardTileComponent = boardTiles[gridPosition.x, gridPosition.y].GetComponent<Tile>();

        if (IsValidMove(gridPosition))
        {
            if (boardTileComponent.IsOccupied)
            {
                Debug.Log("Tile is already occupied.");
                return;
            }

            // Update the board tile data
            boardTileComponent.Letter = selectedTile.Letter;
            boardTileComponent.PointValue = selectedTile.PointValue;
            boardTileComponent.IsOccupied = true;
            boardTileComponent.PlacedThisTurn = true;

            // Change the tile color to indicate it was placed
            boardTileComponent.ChangeColor(Color.green); // Use any color you prefer

            // Update the visual representation
            boardTileComponent.UpdateVisuals();

            // Keep track of the placed tile positions
            tilesPlacedThisTurn.Add(gridPosition);

            // Remove the tile from the player's hand
            currentPlayer.RemoveTile(selectedTile);

            // Destroy the selected tile's GameObject
            Destroy(selectedTile.gameObject);

            // Clear the selected tile from UI
            gameUIManager.ClearSelectedTile();

            // Update UI for the current player
            gameUIManager.DisplayUIForPlayer(currentPlayer);

            Debug.Log($"Placed tile '{boardTileComponent.Letter}' at position {gridPosition.x}, {gridPosition.y}");
        }
        else
        {
            Debug.Log("Invalid move.");
        }
    }

    // Checks if placing a tile at the specified position is a valid move
    bool IsValidMove(Vector2Int position)
    {
        Tile boardTileData = boardTilesData[position.x, position.y];

        // Check if the tile is already occupied
        if (boardTileData.IsOccupied)
        {
            Debug.Log("Tile is already occupied.");
            return false;
        }

        // Special case for the first tile placement
        if (tilesPlacedThisTurn.Count + tileManager.TotalTilesPlaced == 0)
        {
            // The first tile must be placed in the center (adjust coordinates as necessary)
            if (position == new Vector2Int(7, 7)) // Center tile
            {
                return true;
            }
            else
            {
                Debug.Log("First tile must be placed at the center.");
                return false;
            }
        }

        // Check if the tile is adjacent to an existing tile
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        foreach (var dir in directions)
        {
            Vector2Int adjacentPos = position + dir;
            if (IsValidGridPosition(adjacentPos))
            {
                if (boardTilesData[adjacentPos.x, adjacentPos.y].IsOccupied)
                {
                    return true; // It's adjacent to an occupied tile, so the move is valid
                }
            }
        }

        Debug.Log("Move must be adjacent to an existing tile.");
        return false;
    }

    // Resets the tiles placed this turn, returning them to the player's hand
    public void ResetTilesPlacedThisTurn()
    {
        Player currentPlayer = tileManager.GetCurrentPlayer();

        foreach (Vector2Int position in tilesPlacedThisTurn)
        {
            GameObject boardTileObject = boardTiles[position.x, position.y];
            Tile boardTileComponent = boardTileObject.GetComponent<Tile>();

            if (boardTileComponent != null && boardTileComponent.IsOccupied)
            {
                // Create a new tile for the player's hand
                GameObject tileObject = Instantiate(tilePrefab);
                Tile tileComponent = tileObject.GetComponent<Tile>();

                // Set properties from the board tile
                tileComponent.TileType = "";
                tileComponent.Letter = boardTileComponent.Letter;
                tileComponent.PointValue = boardTileComponent.PointValue;
                tileComponent.IsOccupied = false;
                tileComponent.PlacedThisTurn = false;

                // Add the tile back to the player's hand
                currentPlayer.AddTile(tileComponent);

                // Fully reset the board tile's state
                boardTileComponent.Letter = '\0';
                boardTileComponent.PointValue = 0;
                boardTileComponent.IsOccupied = false;
                boardTileComponent.PlacedThisTurn = false;
                boardTileComponent.RevertColor();  // Revert the color to the original

                // Update the visual representation of the board tile
                boardTileComponent.UpdateVisuals();
            }
        }

        // Clear the list of tiles placed this turn
        tilesPlacedThisTurn.Clear();

        // Update the UI for the current player's hand
        gameUIManager.DisplayUIForPlayer(currentPlayer);
    }

    // Expose necessary data and methods to TileManager
    public List<Vector2Int> GetTilesPlacedThisTurn()
    {
        return tilesPlacedThisTurn;
    }

    public Tile[,] GetBoardTilesData()
    {
        return boardTilesData;
    }

    public void ClearTilesPlacedThisTurn()
    {
        tilesPlacedThisTurn.Clear();
    }
}
