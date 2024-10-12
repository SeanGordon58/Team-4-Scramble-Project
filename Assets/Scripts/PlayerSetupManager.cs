using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PlayerSetupManager : MonoBehaviour
{
    // Input field for the number of players
    public TMP_InputField numberOfPlayersInput;

    // Prefab for player name input fields
    public GameObject playerNameInputPrefab;

    // Container to hold player name input fields
    public Transform playerNamesContainer;

    // Button to start the game
    public Button startGameButton;

    private int numberOfPlayers; // The number of players
    private List<TMP_InputField> playerNameInputs = new List<TMP_InputField>(); // List of player name inputs

    public TileManager tileManager; // Reference to TileManager
    public GameUIManager gameUIManager; // Reference to GameUIManager

    void Start()
    {
        // Add listener to number of players input
        numberOfPlayersInput.onValueChanged.AddListener(OnNumberOfPlayersChanged);

        // Add listener to start game button
        startGameButton.onClick.AddListener(OnStartGameClicked);
    }

    // Called when the number of players input changes
    void OnNumberOfPlayersChanged(string value)
    {
        // Clear existing inputs
        foreach (var input in playerNameInputs)
        {
            Destroy(input.gameObject);
        }
        playerNameInputs.Clear();

        // Parse number of players
        if (int.TryParse(value, out numberOfPlayers))
        {
            // Limit the number of players if necessary
            numberOfPlayers = Mathf.Clamp(numberOfPlayers, 1, 4); // For example, limit to 4 players

            // Create input fields for player names
            for (int i = 0; i < numberOfPlayers; i++)
            {
                GameObject inputObj = Instantiate(playerNameInputPrefab, playerNamesContainer);
                TMP_InputField inputField = inputObj.GetComponent<TMP_InputField>();
                inputField.placeholder.GetComponent<TextMeshProUGUI>().text = $"Player {i + 1} Name";
                playerNameInputs.Add(inputField);
            }
        }
    }

    // Called when the Start Game button is clicked
    void OnStartGameClicked()
    {
        // Ensure valid number of players
        if (playerNameInputs.Count < 2 || playerNameInputs.Count > 4)
        {
            return;
        }

        List<string> playerNames = new List<string>();
        foreach (var input in playerNameInputs)
        {
            string name = input.text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                name = $"Player {playerNames.Count + 1}"; // Default name if none provided
            }
            playerNames.Add(name);
        }

        // Pass the player info to TileManager
        tileManager.InitializeGame(numberOfPlayers, playerNames);
        gameUIManager.InitializeUI();

        // Hide the player setup panel
        playerNamesContainer.gameObject.SetActive(false);
        numberOfPlayersInput.gameObject.SetActive(false);
        startGameButton.gameObject.SetActive(false);
    }
}
