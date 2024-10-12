using UnityEngine;
using TMPro;

public class BoardTile : MonoBehaviour
{
    // The type of the tile (e.g., DWS, TWS, etc.)
    public string TileType;

    // Text component to display the tile's letter or type
    public TextMeshProUGUI Text;

    // Main square renderer
    public SpriteRenderer SquareRenderer;

    // Accent renderer for special tiles
    public SpriteRenderer AccentSquareRenderer;

    // Whether the tile is occupied
    public bool IsOccupied = false;

    // Whether the tile was placed this turn
    public bool PlacedThisTurn = false;
}
