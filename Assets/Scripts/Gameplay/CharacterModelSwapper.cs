using UnityEngine;

/// <summary>
/// Component that handles dynamic swapping of character models in the motorcycle rider prefab.
/// This allows different characters to use different 3D models while keeping the same motorcycle physics.
/// </summary>
public class CharacterModelSwapper : MonoBehaviour
{
    [Header("Model Configuration")]
    [Tooltip("The current rider model GameObject that will be replaced")]
    public GameObject currentRiderModel;
    
    [Tooltip("Parent transform where the rider model should be attached")]
    public Transform riderParent;
    
    [Header("Default Model")]
    [Tooltip("Default rider model to use if no character is selected")]
    public GameObject defaultRiderModel;
    
    private GameObject activeRiderInstance;
    private CharacterData currentCharacter;
    
    void Start()
    {
        // Try to get character from SelectionData
        InitializeCharacterModel();
    }
    
    /// <summary>
    /// Initialize the character model based on current selection
    /// </summary>
    public void InitializeCharacterModel()
    {
        CharacterData selectedCharacter = GetSelectedCharacter();
        
        if (selectedCharacter != null)
        {
            SwapToCharacterModel(selectedCharacter);
        }
        else
        {
            // Use default model if no character selected
            SwapToDefaultModel();
        }
    }
    
    /// <summary>
    /// Swap to a specific character's model
    /// </summary>
    /// <param name="characterData">Character data containing the model to swap to</param>
    public void SwapToCharacterModel(CharacterData characterData)
    {
        if (characterData == null)
        {
            Debug.LogWarning("CharacterModelSwapper: No character data provided, using default model");
            SwapToDefaultModel();
            return;
        }
        
        currentCharacter = characterData;
        
        // Use character's model if available, otherwise use default
        GameObject modelToUse = characterData.characterModel != null ? characterData.characterModel : defaultRiderModel;
        
        if (modelToUse != null)
        {
            SwapModel(modelToUse);
            Debug.Log($"CharacterModelSwapper: Swapped to {characterData.characterName} model");
        }
        else
        {
            Debug.LogWarning($"CharacterModelSwapper: No model found for {characterData.characterName}, keeping current model");
        }
    }
    
    /// <summary>
    /// Swap to the default rider model
    /// </summary>
    public void SwapToDefaultModel()
    {
        if (defaultRiderModel != null)
        {
            SwapModel(defaultRiderModel);
            Debug.Log("CharacterModelSwapper: Using default rider model");
        }
        else
        {
            Debug.LogWarning("CharacterModelSwapper: No default rider model assigned");
        }
    }
    
    /// <summary>
    /// Perform the actual model swap
    /// </summary>
    /// <param name="newModel">The new model to instantiate</param>
    private void SwapModel(GameObject newModel)
    {
        // Remove existing rider instance if it exists
        if (activeRiderInstance != null)
        {
            DestroyImmediate(activeRiderInstance);
        }
        
        // Determine parent transform
        Transform parent = riderParent != null ? riderParent : transform;
        
        // Instantiate new model
        activeRiderInstance = Instantiate(newModel, parent);
        
        // Reset transform to match parent
        activeRiderInstance.transform.localPosition = Vector3.zero;
        activeRiderInstance.transform.localRotation = Quaternion.identity;
        activeRiderInstance.transform.localScale = Vector3.one;
        
        // Disable the original rider model if it exists
        if (currentRiderModel != null && currentRiderModel != activeRiderInstance)
        {
            currentRiderModel.SetActive(false);
        }
    }
    
    /// <summary>
    /// Get the currently selected character from SelectionData
    /// </summary>
    /// <returns>Selected character data or null if none selected</returns>
    private CharacterData GetSelectedCharacter()
    {
        // Try to get from SelectionData static class
        if (SelectionData.SelectedCharacter != null)
        {
            return SelectionData.SelectedCharacter;
        }
        
        // Fallback: try to load from PlayerPrefs
        string characterName = PlayerPrefs.GetString("SelectedCharacter", "");
        if (!string.IsNullOrEmpty(characterName))
        {
            return LoadCharacterByName(characterName);
        }
        
        return null;
    }
    
    /// <summary>
    /// Load a character by name from Resources
    /// </summary>
    /// <param name="characterName">Name of the character to load</param>
    /// <returns>CharacterData or null if not found</returns>
    private CharacterData LoadCharacterByName(string characterName)
    {
        CharacterData[] allCharacters = Resources.LoadAll<CharacterData>("Characters");
        
        foreach (CharacterData character in allCharacters)
        {
            if (character.characterName == characterName)
            {
                return character;
            }
        }
        
        Debug.LogWarning($"CharacterModelSwapper: Character '{characterName}' not found in Resources/Characters");
        return null;
    }
    
    /// <summary>
    /// Get the current character data
    /// </summary>
    /// <returns>Current character or null</returns>
    public CharacterData GetCurrentCharacter()
    {
        return currentCharacter;
    }
    
    /// <summary>
    /// Force a model refresh (useful for editor or runtime changes)
    /// </summary>
    [ContextMenu("Refresh Character Model")]
    public void RefreshModel()
    {
        InitializeCharacterModel();
    }
}
