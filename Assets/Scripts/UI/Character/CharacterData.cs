using UnityEngine;

/// <summary>
/// Stores character selection data and stats for the Character Selection scene.
/// </summary>
[CreateAssetMenu(fileName = "CharacterData", menuName = "TimeRiders/Character Data", order = 0)]
public class CharacterData : ScriptableObject
{
    [Header("General Info")]
    public string characterName;
    [TextArea]
    public string description;
    public Sprite portrait;

    [Header("3D Model")]
    [Tooltip("Character model prefab or GameObject to use in the motorcycle rider")]
    public GameObject characterModel;

    [Header("Stats")]
    public int speed;
    public int health;
    public int jump;

    [Header("Ability Summary")]
    public string abilityDescription;
}
