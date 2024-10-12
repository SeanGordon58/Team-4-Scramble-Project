using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class GameUIManager : MonoBehaviour
{
    // Prefab for the tile button UI
    public GameObject tileButtonPrefab;

    // Panel to hold tile buttons
    public Transform tilePanel;

    // Reference to the End Turn button
    public Button endTurnButton;

    // Reference to the End Game button
    public Button endGameButton;

    // Reference to the Restart button
    public Button restartButton;

    // Text displaying the current player
    public TextMeshProUGUI PlayerIndicator;

    public TileManager tileManager; // Reference to the Tile Manager

    private Tile selectedTile; // The currently selected tile for placement

    // Initializes the game UI elements
    public void InitializeUI()
    {
        endTurnButton.gameObject.SetActive(true);
        endGameButton.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);
        tilePanel.gameObject.SetActive(true);
        PlayerIndicator.gameObject.SetActive(true);
    }

    void Start()
    {
        // Add listener for the End Turn button
        endTurnButton.onClick.AddListener(OnEndTurnButtonClicked);
        // Add listener for the End Game button
        endGameButton.onClick.AddListener(OnEndGameButtonClicked);
        // Add listener for the Restart button
        restartButton.onClick.AddListener(OnRestartButtonClicked);
    }

    // Method called when the Restart button is clicked
    void OnRestartButtonClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reloads the current scene
    }

    // Method called when the End Game button is clicked
    void OnEndGameButtonClicked()
    {
        Application.Quit(); // Quits the application
    }

    // Method called when the End Turn button is clicked
    void OnEndTurnButtonClicked()
    {
        tileManager.FinalizeTurn(); // Finalizes the turn in the tile manager
    }

    // Populates the player's tiles in the UI
    public void DisplayUIForPlayer(Player player)
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

        PlayerIndicator.text = $"Current Player: {player.Name}";
    }

    // Called when the player selects a tile
    private void SelectTile(Tile tile)
    {
        selectedTile = tile; // Set the selected tile
        Debug.Log($"Selected tile: {tile.Letter}");
    }

    // Returns the selected tile for placement
    public Tile GetSelectedTile()
    {
        return selectedTile;
    }

    // Clears the selected tile after placement
    public void ClearSelectedTile()
    {
        selectedTile = null;
    }
}
