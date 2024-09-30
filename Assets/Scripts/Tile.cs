using UnityEngine;
using TMPro;

public class Tile : MonoBehaviour
{
    public string TileType;     // Special tile types like DWS, TWS, etc.
    public char Letter;         // The letter placed on this tile, if any
    public int PointValue;      // The point value of the letter
    public bool IsOccupied;     // Whether the tile is occupied by a letter
    public bool PlacedThisTurn; // Whether the tile was placed this turn

    // UI Components
    public TextMeshProUGUI Text;
    public SpriteRenderer SquareRenderer;
    public SpriteRenderer AccentSquareRenderer;

    private Color originalColor;

    void Awake()
    {
        // Cache the original color of the tile
        originalColor = SquareRenderer.color;
    }

    // Update the color of the tile
    public void ChangeColor(Color newColor)
    {
        SquareRenderer.color = newColor;
    }

    // Revert to the original color
    public void RevertColor()
    {
        SquareRenderer.color = originalColor;
    }

    public void UpdateVisuals()
    {
        if (IsOccupied)
        {
            // Display the letter placed on this tile
            Text.text = Letter.ToString();
            Text.color = Color.black; // Ensure the text is visible
            AccentSquareRenderer.enabled = false; // Hide special tile accent if any
        }
        else if (!string.IsNullOrEmpty(TileType))
        {
            // Display the special tile type
            Text.text = TileType;
            Text.color = GetTileColor(TileType);
            AccentSquareRenderer.enabled = true;
            AccentSquareRenderer.color = GetTileColor(TileType);
        }
        else
        {
            // Empty tile
            Text.text = "";
            AccentSquareRenderer.enabled = false;
        }
    }

    Color GetTileColor(string tileType)
    {
        switch (tileType)
        {
            case "DWS": return Color.red;
            case "TWS": return Color.magenta;
            case "DLS": return Color.cyan;
            case "TLS": return Color.green;
            case "Center": return Color.yellow;
            default: return Color.white;
        }
    }
}
