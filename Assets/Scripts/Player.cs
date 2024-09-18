using System.Collections.Generic;


public class Player
{
    public List<Tile> PlayerTiles { get; private set; }
    public int Score { get; private set; }

    public Player()
    {
        PlayerTiles = new List<Tile>();
    }

    public void AddTile(Tile tile)
    {
        PlayerTiles.Add(tile);
    }

    public void RemoveTile(Tile tile)
    {
        PlayerTiles.Remove(tile);
    }

    public void AddScore(int score)
    {
        Score += score;
    }
}
