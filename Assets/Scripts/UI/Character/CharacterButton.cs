using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterButton : MonoBehaviour
{
    [Header("UI References")]
    public Image characterImage;
    public TextMeshProUGUI characterNameText;
    public Button button;
    
    [Header("Character Data")]
    public CharacterData characterData;
    
    private CharacterSelector selector;
    
    void Start()
    {
        selector = FindFirstObjectByType<CharacterSelector>();
        
        if (characterData)
        {
            SetupButton();
        }
        
        button.onClick.AddListener(OnButtonClick);
    }
    
    void SetupButton()
    {
        characterImage.sprite = characterData.characterPortrait;
        characterNameText.text = characterData.characterName;
    }
    
    void OnButtonClick()
    {
        if (selector)
        {
            selector.SelectCharacter(characterData);
        }
    }
}