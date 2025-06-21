using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages character, mode, and track selection for the Character Selection scene.
/// </summary>
public class CharacterSelectionManager : MonoBehaviour
{
    [Header("3D Carousel")]
    [SerializeField] private Character3DCarousel character3DCarousel;
    
    [Header("UI References")]
    public List<CharacterData> availableCharacters;
    public Image characterPortrait;
    public Text characterNameText;
    public Text abilityText;
    public Text statsText;
    public Dropdown gameModeDropdown;
    public Button continueButton;
    public Button backButton;
    
    [Header("Character Stats Display")]
    [SerializeField] private Slider speedSlider;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider jumpSlider;
    [SerializeField] private Text speedValueText;
    [SerializeField] private Text healthValueText;
    [SerializeField] private Text jumpValueText;
    
    private int selectedCharacterIndex = 0;
    private int selectedGameModeIndex = 0;

    private void Start()
    {
        SetupUI();
        SetupCarousel();
        PopulateGameModes();
        UpdateCharacterDisplay();
    }
    
    /// <summary>
    /// Detect if we're running on a mobile platform at runtime
    /// True for native Android/iOS OR WebGL with touch and tilt sensor support
    /// </summary>
    // Removed DetectMobilePlatform method
    
    private void SetupUI() 
    {
        // Setup button listeners
        continueButton.onClick.AddListener(ContinueToTrackSelection);
        backButton.onClick.AddListener(GoBackToMainMenu);
        
        // Setup dropdown listener
        gameModeDropdown.onValueChanged.AddListener(OnGameModeChanged);
    }
    
    private void SetupCarousel() 
    {
        if (character3DCarousel != null) 
        {
            // Subscribe to character change events
            character3DCarousel.OnCharacterChanged += OnCarouselCharacterChanged;
        }
        else 
        {
            Debug.LogWarning("Character3DCarousel not assigned! Falling back to button navigation.");
        }
    }
    
    private void OnCarouselCharacterChanged(int index, CharacterData characterData) 
    {
        selectedCharacterIndex = index;
        UpdateCharacterDisplay();
    }

    public void NextCharacter()
    {
        if (character3DCarousel != null) 
        {
            character3DCarousel.RotateToCharacter(1);
        }
        else 
        {
            // Fallback for button navigation
            selectedCharacterIndex = (selectedCharacterIndex + 1) % availableCharacters.Count;
            UpdateCharacterDisplay();
        }
    }

    public void PreviousCharacter()
    {
        if (character3DCarousel != null) 
        {
            character3DCarousel.RotateToCharacter(-1);
        }
        else 
        {
            // Fallback for button navigation
            selectedCharacterIndex = (selectedCharacterIndex - 1 + availableCharacters.Count) % availableCharacters.Count;
            UpdateCharacterDisplay();
        }
    }

    void UpdateCharacterDisplay()
    {
        if (selectedCharacterIndex < 0 || selectedCharacterIndex >= availableCharacters.Count) return;
        
        var data = availableCharacters[selectedCharacterIndex];
        
        // Update portrait and text
        if (characterPortrait != null) characterPortrait.sprite = data.portrait;
        if (characterNameText != null) characterNameText.text = data.characterName;
        if (abilityText != null) abilityText.text = data.abilityDescription;
        
        // Update stats text
        if (statsText != null) 
        {
            statsText.text = $"Speed: {data.speed}\nHealth: {data.health}\nJump: {data.jump}";
        }
        
        // Update stat sliders
        UpdateStatSliders(data);
    }
    
    private void UpdateStatSliders(CharacterData data) 
    {
        // Assuming max stat value is 100 for slider calculation
        float maxStat = 100f;
        
        if (speedSlider != null) 
        {
            speedSlider.value = data.speed / maxStat;
            if (speedValueText != null) speedValueText.text = data.speed.ToString();
        }
        
        if (healthSlider != null) 
        {
            healthSlider.value = data.health / maxStat;
            if (healthValueText != null) healthValueText.text = data.health.ToString();
        }
        
        if (jumpSlider != null) 
        {
            jumpSlider.value = data.jump / maxStat;
            if (jumpValueText != null) jumpValueText.text = data.jump.ToString();
        }
    }

    void PopulateGameModes()
    {
        gameModeDropdown.ClearOptions();
        
        // Add single player mode
        gameModeDropdown.options.Add(new Dropdown.OptionData("Single Player"));
        
        // Only add 2-player mode on desktop platforms
        if (PlatformDetector.SupportsSplitScreen)
        {
            gameModeDropdown.options.Add(new Dropdown.OptionData("2-Player Split Screen"));
        }
        
        gameModeDropdown.RefreshShownValue();
    }

    public void OnGameModeChanged(int index)
    {
        selectedGameModeIndex = index;
        
        // Adjust index for mobile builds where 2-player is skipped
        if (!PlatformDetector.SupportsSplitScreen && index >= 1) 
        {
            selectedGameModeIndex = index + 1; // Skip the 2-player mode index
        }
    }

    private void ContinueToTrackSelection()
    {
        // Store selections for next scene
        SelectionData.SelectedCharacter = availableCharacters[selectedCharacterIndex];
        SelectionData.SelectedGameMode = selectedGameModeIndex;
        
        // Load course selection scene
        SceneManager.LoadScene("CourseSelection");
    }
    
    private void GoBackToMainMenu() 
    {
        SceneManager.LoadScene("MainMenu");
    }

    // Legacy method for direct race start (keeping for compatibility)
    public void OnStartButton()
    {
        // Store selections for next scene
        SelectionData.SelectedCharacter = availableCharacters[selectedCharacterIndex];
        SelectionData.SelectedGameMode = selectedGameModeIndex;
        SelectionData.SelectedTrack = 0; // Default track

        string sceneName = "SimpleSetup"; // Default to simple setup
        SceneManager.LoadScene($"Assets/SimpleMotorcyclePhysics/Scenes/{sceneName}.unity");
    }
    
    private void OnDestroy() 
    {
        // Unsubscribe from events
        if (character3DCarousel != null) 
        {
            character3DCarousel.OnCharacterChanged -= OnCarouselCharacterChanged;
        }
    }
}
