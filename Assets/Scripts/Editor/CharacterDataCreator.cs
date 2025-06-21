#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// Editor utility to create CharacterData ScriptableObjects for all Time Riders characters
/// </summary>
public class CharacterDataCreator : EditorWindow
{
    private static GameObject defaultRiderModel;
    
    [MenuItem("TimeRiders/Create Character Data/Create All Characters")]
    public static void CreateAllCharacters()
    {
        LoadDefaultRiderModel();
        
        CreateOliviaRodrigo();
        CreatePink();
        CreateTaylorSwift();
        CreateBillieEilish();
        CreateLizzo();
        CreateClaire();
        CreateMaddy();
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log("All Time Riders character data created successfully!");
        EditorUtility.DisplayDialog("Characters Created", "All 7 Time Riders characters have been created in Assets/Resources/Characters/ with default rider models assigned.", "OK");
    }
    
    /// <summary>
    /// Load the default rider model from the SimpleMotorcyclePhysics folder
    /// </summary>
    private static void LoadDefaultRiderModel()
    {
        // Try to load the Rider.fbx model
        string riderModelPath = "Assets/SimpleMotorcyclePhysics/Models/Rider.fbx";
        defaultRiderModel = AssetDatabase.LoadAssetAtPath<GameObject>(riderModelPath);
        
        if (defaultRiderModel == null)
        {
            Debug.LogWarning($"CharacterDataCreator: Could not find default rider model at {riderModelPath}. Characters will be created without model assignments.");
        }
        else
        {
            Debug.Log($"CharacterDataCreator: Loaded default rider model from {riderModelPath}");
        }
    }
    
    [MenuItem("TimeRiders/Create Character Data/Individual Characters/Olivia Rodrigo")]
    public static void CreateOliviaRodrigo()
    {
        var character = CreateCharacterData(
            "OliviaRodrigo",
            "Olivia Rodrigo",
            "Pop sensation with incredible speed and agility. Known for her emotional depth and lightning-fast reflexes on the track.",
            85 + 5, 80, 75, // Speed bonus (+5)
            "Speed Rush - Temporary speed boost when taking damage"
        );
        SaveCharacterAsset(character, "OliviaRodrigo");
    }
    
    [MenuItem("TimeRiders/Create Character Data/Individual Characters/P!nk")]
    public static void CreatePink()
    {
        var character = CreateCharacterData(
            "Pink",
            "P!nk",
            "Rock star with incredible durability and aerial prowess. Her acrobatic background gives her superior jumping ability and resilience.",
            75, 80 + 20, 70 + 10, // Health bonus (+20), Jump bonus (+10)
            "Acrobatic Recovery - Reduced fall damage and faster health regeneration"
        );
        SaveCharacterAsset(character, "Pink");
    }
    
    [MenuItem("TimeRiders/Create Character Data/Individual Characters/Taylor Swift")]
    public static void CreateTaylorSwift()
    {
        var character = CreateCharacterData(
            "TaylorSwift",
            "Taylor Swift",
            "The ultimate all-around performer with perfectly balanced skills. Her versatility makes her adaptable to any racing situation.",
            80 + 3, 80 + 3, 80 + 3, // Balanced stats (+3 all)
            "Adaptive Performance - Gains small bonuses based on track conditions"
        );
        SaveCharacterAsset(character, "TaylorSwift");
    }
    
    [MenuItem("TimeRiders/Create Character Data/Individual Characters/Billie Eilish")]
    public static void CreateBillieEilish()
    {
        var character = CreateCharacterData(
            "BillieEilish",
            "Billie Eilish",
            "Mysterious and agile rider with stealth capabilities. Her unique style allows for unexpected maneuvers and superior jumping ability.",
            78 + 2, 75, 72 + 8, // Stealth bonus (+2 speed, +8 jump)
            "Shadow Movement - Brief invisibility after taking damage, reduced collision detection"
        );
        SaveCharacterAsset(character, "BillieEilish");
    }
    
    [MenuItem("TimeRiders/Create Character Data/Individual Characters/Lizzo")]
    public static void CreateLizzo()
    {
        var character = CreateCharacterData(
            "Lizzo",
            "Lizzo",
            "Powerhouse performer with incredible durability and resilience. Trades some speed for exceptional survivability and confidence.",
            73 - 2, 85 + 30, 70, // Durability (+30 health, -2 speed)
            "Confidence Boost - Immunity to knockdown effects, reduced damage from all sources"
        );
        SaveCharacterAsset(character, "Lizzo");
    }
    
    [MenuItem("TimeRiders/Create Character Data/Individual Characters/Claire")]
    public static void CreateClaire()
    {
        var character = CreateCharacterData(
            "Claire",
            "Claire",
            "Acrobatic specialist with exceptional jumping ability and speed. Her gymnastic background allows for incredible aerial maneuvers but at the cost of durability.",
            75 + 8, 75 - 5, 65 + 15, // Acrobatic (+15 jump, +8 speed, -5 health)
            "Aerial Mastery - Extended air time, ability to perform mid-air corrections"
        );
        SaveCharacterAsset(character, "Claire");
    }
    
    [MenuItem("TimeRiders/Create Character Data/Individual Characters/Maddy")]
    public static void CreateMaddy()
    {
        var character = CreateCharacterData(
            "Maddy",
            "Maddy",
            "The complete package with solid improvements across all areas. A reliable choice for riders who want consistent performance in every aspect.",
            80 + 5, 80 + 5, 80 + 5, // All-rounder (+5 all stats)
            "Versatile Performance - Small bonuses to all stats that increase over time during races"
        );
        SaveCharacterAsset(character, "Maddy");
    }
    
    private static CharacterData CreateCharacterData(
        string assetName,
        string characterName,
        string description,
        int speed, int health, int jump,
        string abilityDescription)
    {
        var character = ScriptableObject.CreateInstance<CharacterData>();
        character.name = assetName;
        character.characterName = characterName;
        character.description = description;
        character.characterModel = defaultRiderModel; // Assign default model
        character.speed = speed;
        character.health = health;
        character.jump = jump;
        character.abilityDescription = abilityDescription;
        
        return character;
    }
    
    private static void SaveCharacterAsset(CharacterData character, string fileName)
    {
        // Ensure the Resources/Characters directory exists
        string directoryPath = "Assets/Resources/Characters";
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        
        string assetPath = $"{directoryPath}/{fileName}.asset";
        
        // Check if asset already exists and remove it
        if (File.Exists(assetPath))
        {
            AssetDatabase.DeleteAsset(assetPath);
            Debug.Log($"Removed existing character asset: {assetPath}");
        }
        
        AssetDatabase.CreateAsset(character, assetPath);
        Debug.Log($"Created character asset: {assetPath}");
    }
}
#endif
