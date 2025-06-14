using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    public float raceTimeLimit = 300f; // 5 minutes
    public int maxPlayers = 2;
    
    [Header("UI References")]
    public GameObject mainMenuUI;
    public GameObject gameUI;
    public GameObject resultsUI;
    public GameObject characterSelectUI;
    
    [Header("Character Selection")]
    public CharacterButton[] characterButtons;
    public TMP_InputField player1NameInput;
    public TMP_InputField player2NameInput;
    
    [Header("Game UI")]
    public TextMeshProUGUI timerText;
    public Slider healthBar;
    public TextMeshProUGUI speedText;
    public GameObject confettiEffect;
    
    [Header("Results UI")]
    public TextMeshProUGUI finalTimeText;
    public Transform leaderboardParent;
    public GameObject leaderboardEntryPrefab;
    
    [Header("Audio")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioClip backgroundMusic;
    public AudioClip engineSound;
    public AudioClip crashSound;
    
    private GameState currentState = GameState.MainMenu;

    /// <summary>
    /// Public property to expose current game state for tests
    /// </summary>
    public GameState CurrentState => currentState;
    private float currentRaceTime;
    private bool isRaceActive;
    private List<PlayerController> players = new List<PlayerController>();
    private LeaderboardManager leaderboard;
    
    public enum GameState
    {
        MainMenu,
        CharacterSelect,
        Racing,
        Results
    }
    
    void Start()
    {
        leaderboard = GetComponent<LeaderboardManager>();
        SetGameState(GameState.MainMenu);
        
        // Play background music
        if (musicSource && backgroundMusic)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.Play();
        }

        // Auto-assign procedural UI fields if missing
        if (timerText == null)
            timerText = FindTextByName("Timer");
        if (speedText == null)
            speedText = FindTextByName("Speed");
        if (gameUI == null)
            gameUI = GameObject.Find("HUD");
    }
    
    private TextMeshProUGUI FindTextByName(string name)
    {
        var texts = GameObject.FindObjectsOfType<TMPro.TextMeshProUGUI>();
        foreach (var t in texts)
        {
            if (t.gameObject.name == name)
                return t;
        }
        return null;
    }
    
    void Update()
    {
        if (isRaceActive)
        {
            UpdateRaceTimer();
            UpdateUI();
        }
        
        HandleInput();
    }
    
    void HandleInput()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (currentState == GameState.Racing)
            {
                PauseGame();
            }
            else if (currentState != GameState.MainMenu)
            {
                SetGameState(GameState.MainMenu);
            }
        }
    }
    
    public void SetGameState(GameState newState)
    {
        currentState = newState;
        
        // Hide all UI panels
        if (mainMenuUI != null) mainMenuUI.SetActive(false);
        if (gameUI != null) gameUI.SetActive(false);
        if (resultsUI != null) resultsUI.SetActive(false);
        if (characterSelectUI != null) characterSelectUI.SetActive(false);
        
        // Show appropriate UI
        switch (newState)
        {
            case GameState.MainMenu:
                if (mainMenuUI != null) mainMenuUI.SetActive(true);
                break;
            case GameState.CharacterSelect:
                if (characterSelectUI != null) characterSelectUI.SetActive(true);
                break;
            case GameState.Racing:
                if (gameUI != null) gameUI.SetActive(true);
                break;
            case GameState.Results:
                if (resultsUI != null) resultsUI.SetActive(true);
                break;
        }
    }
    
    public void StartGame()
    {
        SetGameState(GameState.CharacterSelect);
    }
    
    public void BeginRace()
    {
        SetGameState(GameState.Racing);
        currentRaceTime = 0f;
        isRaceActive = true;
        
        // Initialize players at start line
        foreach (var player in players)
        {
            player.ResetToStartPosition();
        }
    }
    
    public void EndRace()
    {
        isRaceActive = false;
        
        // Show confetti effect
        if (confettiEffect)
        {
            confettiEffect.SetActive(true);
        }
        
        // Save score and show results
        string playerName = player1NameInput.text;
        if (string.IsNullOrEmpty(playerName)) playerName = "Anonymous";
        
        leaderboard.AddScore(playerName, currentRaceTime);
        ShowResults();
    }
    
    void UpdateRaceTimer()
    {
        currentRaceTime += Time.deltaTime;
        
        if (currentRaceTime >= raceTimeLimit)
        {
            EndRace();
        }
    }
    
    void UpdateUI()
    {
        // Update timer
        float remainingTime = raceTimeLimit - currentRaceTime;
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        if (timerText != null)
        {
            timerText.text = $"{minutes:00}:{seconds:00}";
        }
        else
        {
            GameLogger.Warning("GameManager", "timerText is not assigned.");
        }
        
        // Update player health, speed, and motor input if we have players
        if (players.Count > 0)
        {
            var player = players[0]; // Primary player
            if (healthBar != null)
            {
                healthBar.value = player.GetHealthPercentage();
            }
            else
            {
                GameLogger.Warning("GameManager", "healthBar is not assigned.");
            }
            if (speedText != null)
            {
                speedText.text = $"{player.GetSpeed():F0} km/h\nMotor: {player.GetMotorInput():F2}";
            }
            else
            {
                GameLogger.Warning("GameManager", "speedText is not assigned.");
            }
        }
    }
    
    void ShowResults()
    {
        finalTimeText.text = $"Final Time: {FormatTime(currentRaceTime)}";
        
        // Clear existing leaderboard entries
        foreach (Transform child in leaderboardParent)
        {
            Destroy(child.gameObject);
        }
        
        // Display top 10 scores
        var topScores = leaderboard.GetTopScores(10);
        for (int i = 0; i < topScores.Count; i++)
        {
            GameObject entry = Instantiate(leaderboardEntryPrefab, leaderboardParent);
            var entryText = entry.GetComponent<TextMeshProUGUI>();
            entryText.text = $"{i + 1}. {topScores[i].playerName} - {FormatTime(topScores[i].time)}";
        }
        
        SetGameState(GameState.Results);
    }
    
    string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        float seconds = time % 60;
        return $"{minutes:00}:{seconds:00.00}";
    }
    
    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource && clip)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
    
    void PauseGame()
    {
        Time.timeScale = Time.timeScale == 0 ? 1 : 0;
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }
}