using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public int numberOfPlayers = 2;
    public int initialTilesPerPlayer = 7;

    private Dictionary<char, int> tileBag;
    private List<Tile> availableTiles;
    public List<Player> players;

    public int currentPlayerIndex = 0;
    public int TotalTilesPlaced { get; private set; } = 0;
    public int TotalTurnsMade { get; private set; } = 0;

    public GameObject tilePrefab;
    public BoardManager boardManager; // Reference to BoardManager

    private HashSet<string> validWords;

    public void InitializeGame()
    {
        InitializeTileBag();
        CreatePlayers();
        DistributeInitialTiles();
        LoadDictionary();
    }

    private void InitializeTileBag()
    {
        tileBag = new Dictionary<char, int>
        {
            {'A', 1}, {'B', 3}, {'C', 3}, {'D', 2}, {'E', 1},
            {'F', 4}, {'G', 2}, {'H', 4}, {'I', 1}, {'J', 8},
            {'K', 5}, {'L', 1}, {'M', 3}, {'N', 1}, {'O', 1},
            {'P', 3}, {'Q', 10}, {'R', 1}, {'S', 1}, {'T', 1},
            {'U', 1}, {'V', 4}, {'W', 4}, {'X', 8}, {'Y', 4},
            {'Z', 10}
        };

        availableTiles = new List<Tile>();

        // Add tiles to the bag
        AddTilesToBag('A', 9);
        AddTilesToBag('B', 2);
        AddTilesToBag('C', 2);
        AddTilesToBag('D', 4);
        AddTilesToBag('E', 12);
        AddTilesToBag('F', 2);
        AddTilesToBag('G', 3);
        AddTilesToBag('H', 2);
        AddTilesToBag('I', 9);
        AddTilesToBag('J', 1);
        AddTilesToBag('K', 1);
        AddTilesToBag('L', 4);
        AddTilesToBag('M', 2);
        AddTilesToBag('N', 6);
        AddTilesToBag('O', 8);
        AddTilesToBag('P', 2);
        AddTilesToBag('Q', 1);
        AddTilesToBag('R', 6);
        AddTilesToBag('S', 4);
        AddTilesToBag('T', 6);
        AddTilesToBag('U', 4);
        AddTilesToBag('V', 2);
        AddTilesToBag('W', 2);
        AddTilesToBag('X', 1);
        AddTilesToBag('Y', 2);
        AddTilesToBag('Z', 1);
    }

    private void AddTilesToBag(char letter, int count)
    {
        for (int i = 0; i < count; i++)
        {
            // Instantiate the tile prefab
            GameObject tileObject = Instantiate(tilePrefab);
            tileObject.SetActive(false); // We don't need it active in the scene yet

            // Get the Tile component and set properties
            Tile tileComponent = tileObject.GetComponent<Tile>();
            tileComponent.TileType = "";
            tileComponent.Letter = letter;
            tileComponent.PointValue = tileBag[letter];
            tileComponent.IsOccupied = false;
            tileComponent.PlacedThisTurn = false;

            // Add the tile component to the available tiles list
            availableTiles.Add(tileComponent);
        }
    }

    private void CreatePlayers()
    {
        players = new List<Player>();
        for (int i = 0; i < numberOfPlayers; i++)
            players.Add(new Player());
    }

    private void DistributeInitialTiles()
    {
        foreach (var player in players)
        {
            for (int j = 0; j < initialTilesPerPlayer; j++)
            {
                Tile tile = DrawRandomTile();
                if (tile != null)
                {
                    player.AddTile(tile);
                }
            }
        }
    }

    public Tile DrawRandomTile()
    {
        if (availableTiles.Count == 0)
        {
            Debug.LogError("No more tiles left in the bag!");
            return null;
        }

        int randomIndex = Random.Range(0, availableTiles.Count);
        Tile drawnTile = availableTiles[randomIndex];
        availableTiles.RemoveAt(randomIndex);

        // Activate the tile GameObject if necessary
        drawnTile.gameObject.SetActive(true);

        return drawnTile;
    }

    public void IncrementTotalTilesPlaced()
    {
        TotalTilesPlaced++;
    }

    public void EndTurn()
    {
        TotalTurnsMade++;
        currentPlayerIndex = (currentPlayerIndex + 1) % numberOfPlayers;
    }

    public Player GetCurrentPlayer()
    {
        return players[currentPlayerIndex];
    }

    public void LoadDictionary()
    {
        TextAsset dictionaryText = Resources.Load<TextAsset>("dictionary");
        if (dictionaryText != null)
        {
            string[] words = dictionaryText.text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
            validWords = new HashSet<string>(words, System.StringComparer.OrdinalIgnoreCase);
            Debug.Log($"Loaded {validWords.Count} words into the dictionary.");
        }
        else
        {
            Debug.LogError("Failed to load dictionary file. Ensure 'dictionary.txt' is placed in the 'Resources' folder.");
            validWords = new HashSet<string>();
        }
    }

    public void FinalizeTurn()
    {
        List<Vector2Int> tilesPlacedThisTurn = boardManager.GetTilesPlacedThisTurn();
        Tile[,] boardTilesData = boardManager.GetBoardTilesData();

        List<WordInfo> wordsFormed = GetWordsFormed(tilesPlacedThisTurn, boardTilesData);

        if (wordsFormed.Count == 0)
        {
            Debug.Log("No valid words formed.");
            boardManager.ResetTilesPlacedThisTurn();
        }
        else
        {
            int totalScore = 0;

            foreach (WordInfo wordInfo in wordsFormed)
            {
                string word = wordInfo.Word.ToUpper();

                if (validWords.Contains(word))
                {
                    int wordScore = CalculateWordScore(wordInfo);
                    totalScore += wordScore;
                    Debug.Log($"Word '{word}' is valid and scored {wordScore} points.");
                }
                else
                {
                    Debug.Log($"Word '{word}' is invalid.");
                    boardManager.ResetTilesPlacedThisTurn();
                    return;
                }
            }

            // Add total score to current player
            Player currentPlayer = GetCurrentPlayer();
            currentPlayer.AddScore(totalScore);
            Debug.Log($"Player {currentPlayerIndex + 1} earned {totalScore} points this turn. Total score: {currentPlayer.Score}");

            // Refill player's hand
            RefillPlayerTiles(currentPlayer);
        }

        // End the player's turn
        EndTurn();

        // Update UI for the next player
        boardManager.UpdateUIForCurrentPlayer();

        // Clear the list of placed tiles
        boardManager.ClearTilesPlacedThisTurn();
    }

    // Get words formed during this turn
    List<WordInfo> GetWordsFormed(List<Vector2Int> tilesPlacedThisTurn, Tile[,] boardTilesData)
    {
        List<WordInfo> wordsFormed = new List<WordInfo>();
        HashSet<string> processedWords = new HashSet<string>();

        foreach (Vector2Int position in tilesPlacedThisTurn)
        {
            // Check horizontally (left to right)
            WordInfo horizontalWord = GetWord(position, Vector2Int.right, boardTilesData);
            if (horizontalWord != null && !processedWords.Contains(horizontalWord.Word))
            {
                wordsFormed.Add(horizontalWord);
                processedWords.Add(horizontalWord.Word);
            }

            // Check vertically (top to bottom)
            WordInfo verticalWord = GetWord(position, Vector2Int.down, boardTilesData); // Changed from Vector2Int.up to Vector2Int.down
            if (verticalWord != null && !processedWords.Contains(verticalWord.Word))
            {
                wordsFormed.Add(verticalWord);
                processedWords.Add(verticalWord.Word);
            }
        }

        return wordsFormed;
    }

    WordInfo GetWord(Vector2Int startPos, Vector2Int direction, Tile[,] boardTilesData)
    {
        WordInfo wordInfo = new WordInfo();
        List<Tile> wordTiles = new List<Tile>();

        int boardSize = boardTilesData.GetLength(0);

        // Move left or up (based on
        // Move left or up (based on the direction) to find the start of the word
        Vector2Int pos = startPos;
        while (true)
        {
            pos -= direction;  // Move backwards to find the start of the word
            if (!IsValidGridPosition(pos, boardSize) || !boardTilesData[pos.x, pos.y].IsOccupied)
                break;

            wordTiles.Insert(0, boardTilesData[pos.x, pos.y]); // Add tiles to the start of the list
        }

        // Reset position and now move right or down (based on the direction) to find the rest of the word
        pos = startPos;
        while (true)
        {
            if (boardTilesData[pos.x, pos.y].IsOccupied)
            {
                wordTiles.Add(boardTilesData[pos.x, pos.y]); // Add tiles to the end of the list
            }
            else
            {
                break;
            }

            pos += direction; // Move forwards to continue forming the word
            if (!IsValidGridPosition(pos, boardSize))
                break;
        }

        // If the word consists of only one tile and wasn't part of a longer word
        if (wordTiles.Count <= 1)
            return null;

        // Build the word string
        string word = "";
        foreach (Tile tile in wordTiles)
        {
            word += tile.Letter;
        }

        wordInfo.Tiles = wordTiles;
        wordInfo.Word = word;

        return wordInfo;
    }

    bool IsValidGridPosition(Vector2Int position, int boardSize)
    {
        return position.x >= 0 && position.x < boardSize && position.y >= 0 && position.y < boardSize;
    }

    public class WordInfo
    {
        public List<Tile> Tiles = new List<Tile>();
        public string Word;
    }

    int CalculateWordScore(WordInfo wordInfo)
    {
        int wordMultiplier = 1;
        int wordScore = 0;

        foreach (Tile tile in wordInfo.Tiles)
        {
            int letterScore = tile.PointValue;
            int letterMultiplier = 1;

            if (tile.PlacedThisTurn)
            {
                switch (tile.TileType)
                {
                    case "DWS":
                        wordMultiplier *= 2;
                        break;
                    case "TWS":
                        wordMultiplier *= 3;
                        break;
                    case "DLS":
                        letterMultiplier *= 2;
                        break;
                    case "TLS":
                        letterMultiplier *= 3;
                        break;
                }
            }

            wordScore += letterScore * letterMultiplier;
        }

        wordScore *= wordMultiplier;
        return wordScore;
    }

    void RefillPlayerTiles(Player player)
    {
        int tilesToDraw = initialTilesPerPlayer - player.PlayerTiles.Count;

        for (int i = 0; i < tilesToDraw; i++)
        {
            Tile tile = DrawRandomTile();
            if (tile != null)
            {
                player.AddTile(tile);
            }
            else
            {
                Debug.Log("No more tiles left to draw.");
                break;
            }
        }
    }
}
