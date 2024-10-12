using System.Collections.Generic;

public class Player
{
    // List of tiles in the player's hand
    public List<Tile> PlayerTiles { get; private set; }

    // The player's current score
    public int Score;

    // The player's name
    public string Name;

    public Player()
    {
        PlayerTiles = new List<Tile>(); // Initialize the player's tiles list
        Score = 0; // Initialize score to zero
        Name = "Player"; // Default name
    }

    // Optional constructor to set the name directly
    public Player(string name) : this()
    {
        Name = name;
    }

    // Adds a tile to the player's hand
    public void AddTile(Tile tile)
    {
        PlayerTiles.Add(tile);
    }

    // Removes a tile from the player's hand
    public void RemoveTile(Tile tile)
    {
        PlayerTiles.Remove(tile);
    }

    // Adds to the player's score
    public void AddScore(int score)
    {
        Score += score;
    }
}
