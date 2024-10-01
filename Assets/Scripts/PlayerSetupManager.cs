using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PlayerSetupManager : MonoBehaviour
{
    public TMP_InputField numberOfPlayersInput;
    public GameObject playerNameInputPrefab;
    public Transform playerNamesContainer;
    public Button startGameButton;

    private int numberOfPlayers;
    private List<TMP_InputField> playerNameInputs = new List<TMP_InputField>();

    public TileManager tileManager; // Reference to TileManager
    public GameUIManager gameUIManager; // Reference to GameUIManager

    void Start()
    {
        // Add listener to number of players input
        numberOfPlayersInput.onValueChanged.AddListener(OnNumberOfPlayersChanged);

        // Add listener to start game button
        startGameButton.onClick.AddListener(OnStartGameClicked);
    }

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

    void OnStartGameClicked()
    {
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
