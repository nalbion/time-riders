using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Time Riders/Character")]
public class CharacterData : ScriptableObject
{
    public string characterName;
    public Sprite characterPortrait;
    public GameObject characterPrefab;
    public string description;
    
    [Header("Special Abilities")]
    public float speedBonus = 0f;
    public float healthBonus = 0f;
    public float jumpBonus = 0f;
    
    [Header("Character Specific")]
    public bool hasUniqueRidingStyle = false;
    public string uniqueStyleDescription;
}