using UnityEngine;

/// <summary>
/// Static class to persist selection data between scenes
/// </summary>
public static class SelectionData
{
    public static CharacterData SelectedCharacter;
    public static int SelectedGameMode; // 0:1P, 1:2P Split-Screen, 2:PvAI
    public static int SelectedTrack;
    
    // Additional game session data
    public static CharacterData Player2Character; // For 2-player mode
    public static bool IsSplitScreen => SelectedGameMode == 1;
    public static bool IsSinglePlayer => SelectedGameMode == 0;
    public static bool IsPlayerVsAI => SelectedGameMode == 2;
    
    // Session settings
    public static int NumberOfLaps = 3;
    public static float GameStartTime;
    public static bool GameInProgress = false;
    
    /// <summary>
    /// Reset all selection data
    /// </summary>
    public static void Reset() 
    {
        SelectedCharacter = null;
        Player2Character = null;
        SelectedGameMode = 0;
        SelectedTrack = 0;
        NumberOfLaps = 3;
        GameInProgress = false;
    }
    
    /// <summary>
    /// Save current selections to PlayerPrefs for quick replay
    /// </summary>
    public static void SaveToPlayerPrefs() 
    {
        if (SelectedCharacter != null) 
        {
            UnityEngine.PlayerPrefs.SetString("LastCharacter", SelectedCharacter.characterName);
        }
        
        if (Player2Character != null) 
        {
            UnityEngine.PlayerPrefs.SetString("LastPlayer2Character", Player2Character.characterName);
        }
        
        UnityEngine.PlayerPrefs.SetInt("LastGameMode", SelectedGameMode);
        UnityEngine.PlayerPrefs.SetInt("LastTrack", SelectedTrack);
        UnityEngine.PlayerPrefs.SetInt("LastNumberOfLaps", NumberOfLaps);
        UnityEngine.PlayerPrefs.Save();
    }
    
    /// <summary>
    /// Load previous selections from PlayerPrefs
    /// </summary>
    public static bool LoadFromPlayerPrefs() 
    {
        if (!UnityEngine.PlayerPrefs.HasKey("LastCharacter")) 
        {
            return false;
        }
        
        string characterName = UnityEngine.PlayerPrefs.GetString("LastCharacter");
        SelectedCharacter = UnityEngine.Resources.Load<CharacterData>($"Characters/{characterName}");
        
        if (UnityEngine.PlayerPrefs.HasKey("LastPlayer2Character")) 
        {
            string player2Name = UnityEngine.PlayerPrefs.GetString("LastPlayer2Character");
            Player2Character = UnityEngine.Resources.Load<CharacterData>($"Characters/{player2Name}");
        }
        
        SelectedGameMode = UnityEngine.PlayerPrefs.GetInt("LastGameMode", 0);
        SelectedTrack = UnityEngine.PlayerPrefs.GetInt("LastTrack", 0);
        NumberOfLaps = UnityEngine.PlayerPrefs.GetInt("LastNumberOfLaps", 3);
        
        return SelectedCharacter != null;
    }
    
    /// <summary>
    /// Get display text for current game mode
    /// </summary>
    public static string GetGameModeText() 
    {
        switch (SelectedGameMode) 
        {
            case 0: return "1 Player";
            case 1: return "2 Player Split-Screen";
            case 2: return "Player vs AI";
            default: return "Unknown Mode";
        }
    }
    
    /// <summary>
    /// Check if current selection is valid for starting a game
    /// </summary>
    public static bool IsValidSelection() 
    {
        if (SelectedCharacter == null) return false;
        if (IsSplitScreen && Player2Character == null) return false;
        return true;
    }
}
