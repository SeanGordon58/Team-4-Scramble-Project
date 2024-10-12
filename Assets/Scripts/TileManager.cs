using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    // The number of players in the game
    public int numberOfPlayers = 2;

    // The initial number of tiles each player receives
    public int initialTilesPerPlayer = 7;

    // A dictionary representing the tile bag with letter scores
    private Dictionary<char, int> tileBag;

    // A list of all available tiles in the game
    private List<Tile> availableTiles;

    // A list of all players in the game
    public List<Player> players;

    // The index of the current player
    public int currentPlayerIndex = 0;

    // Total number of tiles placed on the board
    public int TotalTilesPlaced { get; private set; } = 0;

    // Total number of turns made in the game
    public int TotalTurnsMade { get; private set; } = 0;

    // The prefab used to create tile GameObjects
    public GameObject tilePrefab;

    // Reference to the BoardManager script
    public BoardManager boardManager; // Reference to BoardManager

    // A set containing all valid words from the dictionary
    private HashSet<string> validWords;

    // The UI container for displaying player scores
    public GameObject scoreContainer;

    // Initializes the game with the specified number of players and their names
    public void InitializeGame(int numberOfPlayers, List<string> playerNames)
    {
        boardManager.enabled = true; // Enable the board manager
        scoreContainer.gameObject.SetActive(true); // Show the score container
        this.numberOfPlayers = numberOfPlayers; // Set the number of players
        InitializeTileBag(); // Initialize the tile bag with letters and counts
        CreatePlayers(playerNames); // Create player objects
        DistributeInitialTiles(); // Give initial tiles to each player
        LoadDictionary(); // Load the valid words dictionary
    }

    // Initializes the tile bag with letter tiles and their counts
    private void InitializeTileBag()
    {
        // Initialize tile bag with letter scores
        tileBag = new Dictionary<char, int>
        {
            {'A', 1}, {'B', 3}, {'C', 3}, {'D', 2}, {'E', 1},
            {'F', 4}, {'G', 2}, {'H', 4}, {'I', 1}, {'J', 8},
            {'K', 5}, {'L', 1}, {'M', 3}, {'N', 1}, {'O', 1},
            {'P', 3}, {'Q', 10}, {'R', 1}, {'S', 1}, {'T', 1},
            {'U', 1}, {'V', 4}, {'W', 4}, {'X', 8}, {'Y', 4},
            {'Z', 10}
        };

        // Initialize the list of available tiles
        availableTiles = new List<Tile>();

        // Add tiles to the bag with specific counts
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

    // Adds a specific number of tiles of a given letter to the bag
    private void AddTilesToBag(char letter, int count)
    {
        for (int i = 0; i < count; i++)
        {
            // Instantiate the tile prefab
            GameObject tileObject = Instantiate(tilePrefab);
            tileObject.SetActive(false); // We don't need it active in the scene yet

            // Get the Tile component and set properties
            Tile tileComponent = tileObject.GetComponent<Tile>();
            tileComponent.TileType = ""; // Set tile type to empty
            tileComponent.Letter = letter; // Assign the letter
            tileComponent.PointValue = tileBag[letter]; // Assign the point value from tileBag
            tileComponent.IsOccupied = false; // Mark as not occupied
            tileComponent.PlacedThisTurn = false; // Not placed yet

            // Add the tile component to the available tiles list
            availableTiles.Add(tileComponent);
        }
    }

    // Creates player objects and assigns names to them
    private void CreatePlayers(List<string> playerNames)
    {
        players = new List<Player>(); // Initialize the list of players
        for (int i = 0; i < numberOfPlayers; i++)
        {
            Player newPlayer = new Player(); // Create a new player
            newPlayer.Name = playerNames[i]; // Assign name from the list
            players.Add(newPlayer); // Add the player to the list
        }
    }

    // Distributes initial tiles to each player
    private void DistributeInitialTiles()
    {
        foreach (var player in players)
        {
            for (int j = 0; j < initialTilesPerPlayer; j++)
            {
                Tile tile = DrawRandomTile(); // Draw a random tile
                if (tile != null)
                {
                    player.AddTile(tile); // Add the tile to the player's hand
                }
            }
        }
    }

    // Draws a random tile from the available tiles
    public Tile DrawRandomTile()
    {
        if (availableTiles.Count == 0)
        {
            Debug.LogError("No more tiles left in the bag!");
            return null;
        }

        int randomIndex = Random.Range(0, availableTiles.Count); // Get a random index
        Tile drawnTile = availableTiles[randomIndex]; // Get the tile at that index
        availableTiles.RemoveAt(randomIndex); // Remove the tile from the bag

        // Activate the tile GameObject if necessary
        drawnTile.gameObject.SetActive(true);

        return drawnTile; // Return the drawn tile
    }

    // Ends the current player's turn and moves to the next player
    public void EndTurn()
    {
        TotalTurnsMade++; // Increment the total turns made
        currentPlayerIndex = (currentPlayerIndex + 1) % numberOfPlayers; // Move to the next player
    }

    // Gets the current player object
    public Player GetCurrentPlayer()
    {
        return players[currentPlayerIndex];
    }

    // Loads the valid words dictionary from a text file
    public void LoadDictionary()
    {
        TextAsset dictionaryText = Resources.Load<TextAsset>("dictionary"); // Load the dictionary text file
        if (dictionaryText != null)
        {
            // Split the text into words, removing empty entries
            string[] words = dictionaryText.text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
            // Create a hash set of words for quick lookup
            validWords = new HashSet<string>(words, System.StringComparer.OrdinalIgnoreCase);
            Debug.Log($"Loaded {validWords.Count} words into the dictionary.");
        }
        else
        {
            Debug.LogError("Failed to load dictionary file. Ensure 'dictionary.txt' is placed in the 'Resources' folder.");
            validWords = new HashSet<string>(); // Initialize as empty set
        }
    }

    // Finalizes the current player's turn, checking for valid words and updating scores
    public void FinalizeTurn()
    {
        // Get the tiles placed this turn from the board manager
        List<Vector2Int> tilesPlacedThisTurn = boardManager.GetTilesPlacedThisTurn();
        // Get the current state of the board tiles
        Tile[,] boardTilesData = boardManager.GetBoardTilesData();

        // Get the list of words formed during this turn
        List<WordInfo> wordsFormed = GetWordsFormed(tilesPlacedThisTurn, boardTilesData);

        if (wordsFormed.Count == 0)
        {
            Debug.Log("No valid words formed.");

            // Revert tile colors
            foreach (Vector2Int position in tilesPlacedThisTurn)
            {
                Tile tile = boardTilesData[position.x, position.y];
                tile.RevertColor();  // Revert the color to the original
            }

            // Reset the tiles placed this turn
            boardManager.ResetTilesPlacedThisTurn();
        }
        else
        {
            Player currentPlayer = GetCurrentPlayer(); // Get the current player

            foreach (WordInfo wordInfo in wordsFormed)
            {
                string word = wordInfo.Word.ToUpper(); // Convert word to uppercase

                if (validWords.Contains(word))
                {
                    int wordScore = CalculateWordScore(wordInfo); // Calculate the word's score
                    currentPlayer.Score += wordScore; // Add to player's score
                    Debug.Log($"Word '{word}' is valid and scored {wordScore} points.");
                }
                else
                {
                    Debug.Log($"Word '{word}' is invalid.");

                    // Revert tile colors if the word is invalid
                    foreach (Vector2Int position in tilesPlacedThisTurn)
                    {
                        Tile tile = boardTilesData[position.x, position.y];
                        tile.RevertColor();  // Revert the color to the original
                    }

                    boardManager.ResetTilesPlacedThisTurn();
                    return;
                }
            }

            TotalTilesPlaced += tilesPlacedThisTurn.Count; // Update total tiles placed

            // Update the player's score display
            var scoreDisplay = scoreContainer.transform.Find($"Player{currentPlayerIndex + 1}ScoreText");
            if (scoreDisplay != null)
            {
                scoreDisplay.GetComponent<TextMeshProUGUI>().text = $"{currentPlayer.Name}: {currentPlayer.Score}";
            }
            else
            {
                Debug.LogError($"Could not find score display for {currentPlayer.Name}");
            }

            // Refill the player's tiles after the turn
            RefillPlayerTiles(currentPlayer);
        }

        // End the player's turn
        EndTurn();

        // Update the UI for the next player
        boardManager.UpdateUIForCurrentPlayer();

        // Clear the list of placed tiles
        boardManager.ClearTilesPlacedThisTurn();
    }

    // Gets the words formed during this turn based on tiles placed
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
            WordInfo verticalWord = GetWord(position, Vector2Int.down, boardTilesData);
            if (verticalWord != null && !processedWords.Contains(verticalWord.Word))
            {
                wordsFormed.Add(verticalWord);
                processedWords.Add(verticalWord.Word);
            }
        }

        return wordsFormed;
    }

    // Constructs a word starting from a position in a specific direction
    WordInfo GetWord(Vector2Int startPos, Vector2Int direction, Tile[,] boardTilesData)
    {
        WordInfo wordInfo = new WordInfo();
        List<Tile> wordTiles = new List<Tile>();

        int boardSize = boardTilesData.GetLength(0);

        // Move backwards to find the start of the word
        Vector2Int pos = startPos;
        while (true)
        {
            pos -= direction;  // Move backwards
            if (!IsValidGridPosition(pos, boardSize) || !boardTilesData[pos.x, pos.y].IsOccupied)
                break;

            wordTiles.Insert(0, boardTilesData[pos.x, pos.y]); // Add tiles to the start of the list
        }

        // Reset position and move forwards to find the rest of the word
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

            pos += direction; // Move forwards
            if (!IsValidGridPosition(pos, boardSize))
                break;
        }

        // If the word consists of only one tile
        if (wordTiles.Count <= 1)
            return null;

        // Build the word string from the tiles
        string word = "";
        foreach (Tile tile in wordTiles)
        {
            word += tile.Letter;
        }

        wordInfo.Tiles = wordTiles;
        wordInfo.Word = word;

        return wordInfo;
    }

    // Checks if a grid position is valid within the board
    bool IsValidGridPosition(Vector2Int position, int boardSize)
    {
        return position.x >= 0 && position.x < boardSize && position.y >= 0 && position.y < boardSize;
    }

    // Class representing information about a formed word
    public class WordInfo
    {
        public List<Tile> Tiles = new List<Tile>(); // The tiles that make up the word
        public string Word; // The word string
    }

    // Calculates the score of a word based on tile values and multipliers
    int CalculateWordScore(WordInfo wordInfo)
    {
        int wordMultiplier = 1; // Multiplier for the word
        int wordScore = 0; // Total score for the word

        foreach (Tile tile in wordInfo.Tiles)
        {
            int letterScore = tile.PointValue; // The point value of the letter
            int letterMultiplier = 1; // Multiplier for the letter

            if (tile.PlacedThisTurn)
            {
                // Apply multipliers based on tile type
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

            wordScore += letterScore * letterMultiplier; // Add to word score
        }

        wordScore *= wordMultiplier; // Apply word multiplier
        return wordScore;
    }

    // Refills the player's hand with tiles up to the initial number
    void RefillPlayerTiles(Player player)
    {
        int tilesToDraw = initialTilesPerPlayer - player.PlayerTiles.Count; // Calculate how many tiles to draw

        for (int i = 0; i < tilesToDraw; i++)
        {
            Tile tile = DrawRandomTile(); // Draw a random tile
            if (tile != null)
            {
                player.AddTile(tile); // Add tile to player's hand
            }
            else
            {
                Debug.Log("No more tiles left to draw.");
                break;
            }
        }
    }
}
