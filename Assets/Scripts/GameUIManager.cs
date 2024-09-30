using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUIManager : MonoBehaviour
{
    // References to UI elements
    public GameObject tileButtonPrefab; // Prefab for the tile button UI
    public Transform tilePanel;
    public Button endTurnButton;        // Reference to the End Turn button

    public TileManager tileManager;   // Reference to the Board Manager

    private Tile selectedTile;    // The currently selected tile for placement

    void Start()
    {
        // Add listener for the End Turn button
        endTurnButton.onClick.AddListener(OnEndTurnButtonClicked);
    }

    // Method called when the End Turn button is clicked
    void OnEndTurnButtonClicked()
    {
        tileManager.FinalizeTurn();
    }

    // Populate the player's tiles in the UI
    public void DisplayPlayerTiles(Player player)
    {
        // Clear the existing buttons
        foreach (Transform child in tilePanel)
        {
            Destroy(child.gameObject);
        }

        // Create a button for each tile in the player's hand
        foreach (Tile tile in player.PlayerTiles)
        {
            GameObject tileButton = Instantiate(tileButtonPrefab, tilePanel, false);
            tileButton.transform.localScale = Vector3.one;

            // Get the TextMeshProUGUI components from the tileButton prefab
            TextMeshProUGUI[] textComponents = tileButton.GetComponentsInChildren<TextMeshProUGUI>();
            if (textComponents.Length >= 2)
            {
                // Assuming the first is Letter Text and the second is Score Text
                textComponents[0].text = tile.Letter.ToString();
                textComponents[1].text = tile.PointValue.ToString();
            }
            else
            {
                Debug.LogError("Could not find TextMeshPro components in tileButton prefab.");
            }

            Button button = tileButton.GetComponent<Button>();
            Tile tileRef = tile; // Local copy for the lambda

            // Add click event to select the tile
            button.onClick.AddListener(() => SelectTile(tileRef));
        }
    }

    // Called when the player selects a tile
    private void SelectTile(Tile tile)
    {
        selectedTile = tile;
        Debug.Log($"Selected tile: {tile.Letter}");
    }

    // Returns the selected tile for placement
    public Tile GetSelectedTile()
    {
        return selectedTile;
    }

    // Clear the selected tile after placement
    public void ClearSelectedTile()
    {
        selectedTile = null;
    }
}
