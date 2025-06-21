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
    
    private void Start() 
    {
        SetupUI();
        LoadPreviousGameData();
    }
    
    private void SetupUI() 
    {
        // Setup button listeners
        startRaceButton.onClick.AddListener(StartRaceWithPreviousConfig);
        newGameButton.onClick.AddListener(StartNewGame);
        settingsButton.onClick.AddListener(OpenSettings);
        exitButton.onClick.AddListener(ExitGame);
        
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
        SceneManager.LoadScene("CharacterSelect");
    }
    
    private void OpenSettings() 
    {
        // TODO: Implement settings screen
        Debug.Log("Settings menu - TODO: Implement");
    }
    
    private void ExitGame() 
    {
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
}
