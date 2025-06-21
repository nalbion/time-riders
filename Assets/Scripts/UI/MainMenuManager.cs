using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the main menu/launch screen with quick replay and new game options
/// </summary>
public class MainMenuManager : MonoBehaviour 
{
    [Header("UI References")]
    [SerializeField] private Button startRaceButton;
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitButton;
    
    [Header("Quick Play Panel")]
    [SerializeField] private GameObject quickPlayPanel;
    [SerializeField] private Text lastCharacterText;
    [SerializeField] private Text lastModeText;
    [SerializeField] private Text lastTrackText;
    [SerializeField] private Image lastCharacterPortrait;
    
    private void Awake()
    {
        AutoAssignButtonReferences();
        ApplyButtonStyling();
    }
    
    private void Start() 
    {
        Debug.Log("MainMenuManager Start() called");
        SetupUI();
        LoadPreviousGameData();
    }
    
    private void SetupUI() 
    {
        Debug.Log("MainMenuManager SetupUI() called");
        DebugButtonReferences();
        
        // Setup button listeners with null checks
        if (startRaceButton != null)
        {
            startRaceButton.onClick.AddListener(StartRaceWithPreviousConfig);
            Debug.Log("Added listener to startRaceButton");
        }
        else
            Debug.LogError("startRaceButton is not assigned in the Inspector!");
            
        if (newGameButton != null)
        {
            newGameButton.onClick.AddListener(StartNewGame);
            Debug.Log("Added listener to newGameButton");
        }
        else
            Debug.LogError("newGameButton is not assigned in the Inspector!");
            
        if (settingsButton != null)
        {
            settingsButton.onClick.AddListener(OpenSettings);
            Debug.Log("Added listener to settingsButton");
        }
        else
            Debug.LogError("settingsButton is not assigned in the Inspector!");
            
        if (exitButton != null)
        {
            exitButton.onClick.AddListener(ExitGame);
            Debug.Log("Added listener to exitButton");
        }
        else
            Debug.LogError("exitButton is not assigned in the Inspector!");
        
        Debug.Log("SetupUI() completed - all button listeners should be active now");
        
        // Configure UI based on mobile detection
        if (PlatformDetector.IsMobilePlatform) 
        {
            Debug.Log("Configuring UI for mobile - limiting to single player mode");
            // Additional mobile-specific UI adjustments can be made here
        }
        else 
        {
            Debug.Log("Configuring UI for desktop - enabling all features including split-screen");
        }
    }
    
    private void LoadPreviousGameData() 
    {
        // Check if we have previous game data
        bool hasPreviousGame = PlayerPrefs.HasKey("LastCharacter") && 
                               PlayerPrefs.HasKey("LastGameMode") && 
                               PlayerPrefs.HasKey("LastTrack");
        
        quickPlayPanel.SetActive(hasPreviousGame);
        
        if (hasPreviousGame) 
        {
            string lastCharacterName = PlayerPrefs.GetString("LastCharacter");
            int lastGameMode = PlayerPrefs.GetInt("LastGameMode");
            int lastTrack = PlayerPrefs.GetInt("LastTrack");
            
            // Update UI with previous selections
            lastCharacterText.text = lastCharacterName;
            lastModeText.text = GetGameModeText(lastGameMode);
            lastTrackText.text = GetTrackText(lastTrack);
            
            // Load character portrait if available
            LoadCharacterPortrait(lastCharacterName);
        }
    }
    
    private void LoadCharacterPortrait(string characterName) 
    {
        // Find the character data by name from the available characters
        var characterData = Resources.Load<CharacterData>($"Characters/{characterName}");
        if (characterData != null && characterData.portrait != null) 
        {
            lastCharacterPortrait.sprite = characterData.portrait;
        }
    }
    
    private string GetGameModeText(int modeIndex) 
    {
        switch (modeIndex) 
        {
            case 0: return "1 Player";
            case 1: return "2 Player Split-Screen";
            case 2: return "Player vs AI";
            default: return "Unknown";
        }
    }
    
    private string GetTrackText(int trackIndex) 
    {
        switch (trackIndex) 
        {
            case 0: return "Simple Track";
            case 1: return "Ragdoll Track";
            default: return "Unknown Track";
        }
    }
    
    private void StartRaceWithPreviousConfig() 
    {
        Debug.Log("START RACE button clicked!");
        
        // Load previous selections into SelectionData
        if (PlayerPrefs.HasKey("LastCharacter")) 
        {
            string characterName = PlayerPrefs.GetString("LastCharacter");
            var characterData = Resources.Load<CharacterData>($"Characters/{characterName}");
            SelectionData.SelectedCharacter = characterData;
            SelectionData.SelectedGameMode = PlayerPrefs.GetInt("LastGameMode");
            SelectionData.SelectedTrack = PlayerPrefs.GetInt("LastTrack");
            
            // Start the race directly
            LoadRaceScene();
        }
        else 
        {
            StartNewGame();
        }
    }
    
    private void StartNewGame() 
    {
        Debug.Log("NEW GAME button clicked!");
        
        // Clear any previous selection data
        SelectionData.SelectedCharacter = null;
        SelectionData.SelectedTrack = 0;
        SelectionData.SelectedGameMode = 0; // Single player
        
        // Load character selection scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("CharacterSelection");
    }
    
    private void OpenSettings() 
    {
        Debug.Log("SETTINGS button clicked!");
        // TODO: Implement settings menu
        // For now, just log that it was clicked
    }
    
    private void ExitGame() 
    {
        Debug.Log("EXIT button clicked!");
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    private void LoadRaceScene() 
    {
        int trackIndex = SelectionData.SelectedTrack;
        string sceneName = trackIndex == 0 ? "SimpleSetup" : "RagdollSetup";
        SceneManager.LoadScene($"Assets/SimpleMotorcyclePhysics/Scenes/{sceneName}.unity");
    }
    
    private void ApplyButtonStyling() 
    {
        // Get or create ButtonStyleManager
        ButtonStyleManager styleManager = FindFirstObjectByType<ButtonStyleManager>();
        if (styleManager == null)
        {
            GameObject styleManagerGO = new GameObject("ButtonStyleManager");
            styleManager = styleManagerGO.AddComponent<ButtonStyleManager>();
        }
        
        // Apply styling to each button with appropriate categories
        if (startRaceButton != null)
            styleManager.ApplyButtonCategory(startRaceButton, ButtonCategory.Primary);
        if (newGameButton != null)
            styleManager.ApplyButtonCategory(newGameButton, ButtonCategory.Primary);
        if (settingsButton != null)
            styleManager.ApplyButtonCategory(settingsButton, ButtonCategory.Secondary);
        if (exitButton != null)
            styleManager.ApplyButtonCategory(exitButton, ButtonCategory.Danger);
            
        Debug.Log("Applied button visual effects to MainMenu buttons");
    }
    
    private void AutoAssignButtonReferences()
    {
        Debug.Log("Auto-assigning button references...");
        
        // Find all buttons in the scene
        Button[] allButtons = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        
        foreach (Button button in allButtons)
        {
            Text buttonText = button.GetComponentInChildren<Text>();
            if (buttonText == null) continue;
            
            string textContent = buttonText.text.ToLower();
            string buttonName = button.name.ToLower();
            
            // Match button text content or object name to assign references
            if ((textContent.Contains("new game") || buttonName.Contains("newgame")) && newGameButton == null)
            {
                newGameButton = button;
                Debug.Log($"Auto-assigned NEW GAME button: {button.name}");
            }
            else if ((textContent.Contains("start") || textContent.Contains("race") || buttonName.Contains("start")) && startRaceButton == null)
            {
                startRaceButton = button;
                Debug.Log($"Auto-assigned START RACE button: {button.name}");
            }
            else if ((textContent.Contains("settings") || textContent.Contains("options") || buttonName.Contains("settings")) && settingsButton == null)
            {
                settingsButton = button;
                Debug.Log($"Auto-assigned SETTINGS button: {button.name}");
            }
            else if ((textContent.Contains("exit") || textContent.Contains("quit") || buttonName.Contains("exit")) && exitButton == null)
            {
                exitButton = button;
                Debug.Log($"Auto-assigned EXIT button: {button.name}");
            }
        }
    }
    
    private void DebugButtonReferences()
    {
        Debug.Log("=== Button Reference Debug ===");
        Debug.Log($"startRaceButton: {(startRaceButton != null ? startRaceButton.name : "NULL")}");
        Debug.Log($"newGameButton: {(newGameButton != null ? newGameButton.name : "NULL")}");
        Debug.Log($"settingsButton: {(settingsButton != null ? settingsButton.name : "NULL")}");
        Debug.Log($"exitButton: {(exitButton != null ? exitButton.name : "NULL")}");
        
        // Also check how many buttons are in the scene
        Button[] allButtons = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        Debug.Log($"Total buttons found in scene: {allButtons.Length}");
        
        foreach (Button button in allButtons)
        {
            Text buttonText = button.GetComponentInChildren<Text>();
            string textContent = buttonText != null ? buttonText.text : "No Text";
            Debug.Log($"Button: {button.name} | Text: '{textContent}' | Active: {button.gameObject.activeInHierarchy}");
        }
    }
    
    // Test method to manually trigger button clicks for debugging
    [ContextMenu("Test Button Clicks")]
    private void TestButtonClicks()
    {
        Debug.Log("=== Testing Button Clicks ===");
        
        if (newGameButton != null)
        {
            Debug.Log("Manually triggering NEW GAME button...");
            newGameButton.onClick.Invoke();
        }
        else
        {
            Debug.LogError("Cannot test NEW GAME button - reference is null!");
        }
        
        if (startRaceButton != null)
        {
            Debug.Log("Manually triggering START RACE button...");
            startRaceButton.onClick.Invoke();
        }
        else
        {
            Debug.LogError("Cannot test START RACE button - reference is null!");
        }
    }
}
