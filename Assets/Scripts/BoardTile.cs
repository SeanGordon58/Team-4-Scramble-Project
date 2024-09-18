using UnityEngine;
using TMPro;

public class BoardTile : MonoBehaviour
{
    public string TileType; // Can be used to store special tiles (DWS, TWS, etc.)
    public TextMeshProUGUI Text;
    public SpriteRenderer SquareRenderer;
    public SpriteRenderer AccentSquareRenderer;

    public bool IsOccupied = false;
    public bool PlacedThisTurn = false;
}
