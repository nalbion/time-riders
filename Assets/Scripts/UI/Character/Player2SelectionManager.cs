using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages Player 2 character selection for split-screen mode
/// </summary>
public class Player2SelectionManager : MonoBehaviour 
{
    [Header("UI References")]
    [SerializeField] private GameObject player2Panel;
    [SerializeField] private Character3DCarousel player2Carousel;
    [SerializeField] private Image player2Portrait;
    [SerializeField] private Text player2NameText;
    [SerializeField] private Text player2StatsText;
    [SerializeField] private Button player2LeftButton;
    [SerializeField] private Button player2RightButton;
    
    [Header("Comparison Display")]
    [SerializeField] private GameObject comparisonPanel;
    [SerializeField] private Text player1NameDisplay;
    [SerializeField] private Text player2NameDisplay;
    [SerializeField] private Slider player1SpeedBar;
    [SerializeField] private Slider player1HealthBar;
    [SerializeField] private Slider player1JumpBar;
    [SerializeField] private Slider player2SpeedBar;
    [SerializeField] private Slider player2HealthBar;
    [SerializeField] private Slider player2JumpBar;
    
    [Header("Navigation")]
    [SerializeField] private Button continueButton;
    [SerializeField] private Button backButton;
    
    [Header("Character Data")]
    [SerializeField] private List<CharacterData> availableCharacters;
    
    private int player2CharacterIndex = 0;
    
    private void Start() 
    {
        // Only show if we're in 2-player mode
        if (!SelectionData.IsSplitScreen) 
        {
            gameObject.SetActive(false);
            return;
        }
        
        SetupUI();
        InitializePlayer2Selection();
        UpdateDisplay();
    }
    
    private void SetupUI() 
    {
        player2Panel.SetActive(true);
        comparisonPanel.SetActive(true);
        
        // Setup button listeners
        continueButton.onClick.AddListener(ContinueToTrackSelection);
        backButton.onClick.AddListener(GoBackToCharacterSelection);
        player2LeftButton.onClick.AddListener(() => ChangePlayer2Character(-1));
        player2RightButton.onClick.AddListener(() => ChangePlayer2Character(1));
        
        // Setup carousel if available
        if (player2Carousel != null) 
        {
            player2Carousel.OnCharacterChanged += OnPlayer2CharacterChanged;
        }
    }
    
    private void InitializePlayer2Selection() 
    {
        // Start with a different character than Player 1
        player2CharacterIndex = (GetPlayer1CharacterIndex() + 1) % availableCharacters.Count;
        
        // Set initial Player 2 character
        if (player2Carousel != null) 
        {
            player2Carousel.SetCharacterIndex(player2CharacterIndex);
        }
    }
    
    private int GetPlayer1CharacterIndex() 
    {
        if (SelectionData.SelectedCharacter == null) return 0;
        
        for (int i = 0; i < availableCharacters.Count; i++) 
        {
            if (availableCharacters[i] == SelectionData.SelectedCharacter) 
            {
                return i;
            }
        }
        return 0;
    }
    
    private void OnPlayer2CharacterChanged(int index, CharacterData characterData) 
    {
        player2CharacterIndex = index;
        UpdatePlayer2Display();
        UpdateComparison();
    }
    
    private void ChangePlayer2Character(int direction) 
    {
        if (player2Carousel != null) 
        {
            player2Carousel.RotateToCharacter(direction);
        }
        else 
        {
            // Fallback for button navigation
            player2CharacterIndex = (player2CharacterIndex + direction + availableCharacters.Count) % availableCharacters.Count;
            UpdatePlayer2Display();
            UpdateComparison();
        }
    }
    
    private void UpdateDisplay() 
    {
        UpdatePlayer2Display();
        UpdateComparison();
    }
    
    private void UpdatePlayer2Display() 
    {
        if (player2CharacterIndex < 0 || player2CharacterIndex >= availableCharacters.Count) return;
        
        CharacterData data = availableCharacters[player2CharacterIndex];
        
        // Update Player 2 UI
        if (player2Portrait != null) player2Portrait.sprite = data.portrait;
        if (player2NameText != null) player2NameText.text = data.characterName;
        if (player2StatsText != null) 
        {
            player2StatsText.text = $"Speed: {data.speed}\nHealth: {data.health}\nJump: {data.jump}\n\n{data.abilityDescription}";
        }
    }
    
    private void UpdateComparison() 
    {
        // Update Player 1 comparison display
        if (SelectionData.SelectedCharacter != null) 
        {
            if (player1NameDisplay != null) player1NameDisplay.text = $"P1: {SelectionData.SelectedCharacter.characterName}";
            UpdateStatBar(player1SpeedBar, SelectionData.SelectedCharacter.speed);
            UpdateStatBar(player1HealthBar, SelectionData.SelectedCharacter.health);
            UpdateStatBar(player1JumpBar, SelectionData.SelectedCharacter.jump);
        }
        
        // Update Player 2 comparison display
        if (player2CharacterIndex >= 0 && player2CharacterIndex < availableCharacters.Count) 
        {
            CharacterData player2Data = availableCharacters[player2CharacterIndex];
            if (player2NameDisplay != null) player2NameDisplay.text = $"P2: {player2Data.characterName}";
            UpdateStatBar(player2SpeedBar, player2Data.speed);
            UpdateStatBar(player2HealthBar, player2Data.health);
            UpdateStatBar(player2JumpBar, player2Data.jump);
        }
    }
    
    private void UpdateStatBar(Slider slider, int statValue) 
    {
        if (slider != null) 
        {
            slider.value = statValue / 100f; // Assuming max stat is 100
        }
    }
    
    private void ContinueToTrackSelection() 
    {
        // Store Player 2 selection
        if (player2CharacterIndex >= 0 && player2CharacterIndex < availableCharacters.Count) 
        {
            SelectionData.Player2Character = availableCharacters[player2CharacterIndex];
        }
        
        // Validate selection
        if (!SelectionData.IsValidSelection()) 
        {
            Debug.LogWarning("Invalid character selection for 2-player mode!");
            return;
        }
        
        // Continue to course selection
        SceneManager.LoadScene("CourseSelection");
    }
    
    private void GoBackToCharacterSelection() 
    {
        SceneManager.LoadScene("CharacterSelect");
    }
    
    private void OnDestroy() 
    {
        // Unsubscribe from events
        if (player2Carousel != null) 
        {
            player2Carousel.OnCharacterChanged -= OnPlayer2CharacterChanged;
        }
    }
    
    /// <summary>
    /// Static method to check if Player 2 selection is needed
    /// </summary>
    public static bool IsPlayer2SelectionNeeded() 
    {
        return SelectionData.IsSplitScreen && SelectionData.Player2Character == null;
    }
}
