using UnityEngine;
using TMPro;

public class CharacterSelector : MonoBehaviour
{
    [Header("Selected Character Display")]
    public TextMeshProUGUI selectedCharacterText;
    public UnityEngine.UI.Image selectedCharacterImage;
    
    private CharacterData selectedCharacter;
    private GameManager gameManager;
    
    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }
    
    public void SelectCharacter(CharacterData character)
    {
        selectedCharacter = character;
        
        if (selectedCharacterText)
        {
            selectedCharacterText.text = $"Selected: {character.characterName}";
        }
        
        if (selectedCharacterImage)
        {
            selectedCharacterImage.sprite = character.characterPortrait;
        }
    }
    
    public void ConfirmSelection()
    {
        if (selectedCharacter && gameManager)
        {
            // Pass selected character to game manager and start race
            gameManager.BeginRace();
        }
    }
    
    public CharacterData GetSelectedCharacter()
    {
        return selectedCharacter;
    }
}