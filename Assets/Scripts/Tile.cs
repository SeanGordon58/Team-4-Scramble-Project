using UnityEngine;
using TMPro;

public class Tile : MonoBehaviour
{
    // The type of the tile (e.g., DWS, TWS, etc.)
    public string TileType;

    // The letter assigned to this tile
    public char Letter;

    // The point value of the letter
    public int PointValue;

    // Whether the tile is occupied by a letter
    public bool IsOccupied;

    // Whether the tile was placed this turn
    public bool PlacedThisTurn;

    // UI components
    public TextMeshProUGUI Text; // Reference to the text component displaying the letter or tile type
    public SpriteRenderer SquareRenderer; // The main square renderer
    public SpriteRenderer AccentSquareRenderer; // The accent renderer for special tiles

    private Color originalColor; // The original color of the tile

    void Awake()
    {
        // Cache the original color of the tile
        originalColor = SquareRenderer.color;
    }

    // Updates the color of the tile
    public void ChangeColor(Color newColor)
    {
        SquareRenderer.color = newColor;
    }

    // Reverts the tile color to the original
    public void RevertColor()
    {
        SquareRenderer.color = originalColor;
    }

    // Updates the visual representation of the tile based on its state
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
            Text.color = GetTileColor(TileType); // Set text color based on tile type
            AccentSquareRenderer.enabled = true;
            AccentSquareRenderer.color = GetTileColor(TileType); // Set accent color
        }
        else
        {
            // Empty tile
            Text.text = "";
            AccentSquareRenderer.enabled = false;
        }
    }

    // Returns the color associated with a special tile type
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
