#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

/// <summary>
/// Editor utility to set up the MotorcycleWRider prefab with character model swapping functionality
/// </summary>
public class MotorcycleSetupHelper : EditorWindow
{
    [MenuItem("TimeRiders/Setup Motorcycle/Add Character Model Swapper")]
    public static void AddCharacterModelSwapper()
    {
        // Load the MotorcycleWRider prefab
        string prefabPath = "Assets/SimpleMotorcyclePhysics/Prefabs/MotorcycleWRider.prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        
        if (prefab == null)
        {
            EditorUtility.DisplayDialog("Error", $"Could not find MotorcycleWRider prefab at {prefabPath}", "OK");
            return;
        }
        
        // Check if CharacterModelSwapper already exists
        CharacterModelSwapper existingSwapper = prefab.GetComponent<CharacterModelSwapper>();
        if (existingSwapper != null)
        {
            Debug.Log("CharacterModelSwapper already exists on MotorcycleWRider prefab");
            EditorUtility.DisplayDialog("Already Setup", "CharacterModelSwapper component already exists on the MotorcycleWRider prefab.", "OK");
            return;
        }
        
        // Create a prefab instance to modify
        GameObject prefabInstance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        
        // Add CharacterModelSwapper component
        CharacterModelSwapper swapper = prefabInstance.AddComponent<CharacterModelSwapper>();
        
        // Try to find the rider model in the prefab hierarchy
        Transform riderTransform = FindRiderInHierarchy(prefabInstance.transform);
        
        if (riderTransform != null)
        {
            swapper.currentRiderModel = riderTransform.gameObject;
            swapper.riderParent = riderTransform.parent; // Use the parent as the attachment point
            Debug.Log($"Found rider model: {riderTransform.name}");
        }
        else
        {
            Debug.LogWarning("Could not automatically find rider model in prefab hierarchy. Please assign manually.");
        }
        
        // Load default rider model
        string defaultModelPath = "Assets/SimpleMotorcyclePhysics/Models/Rider.fbx";
        GameObject defaultModel = AssetDatabase.LoadAssetAtPath<GameObject>(defaultModelPath);
        if (defaultModel != null)
        {
            swapper.defaultRiderModel = defaultModel;
            Debug.Log($"Assigned default rider model: {defaultModel.name}");
        }
        else
        {
            Debug.LogWarning($"Could not find default rider model at {defaultModelPath}");
        }
        
        // Apply changes back to prefab
        PrefabUtility.ApplyPrefabInstance(prefabInstance, InteractionMode.UserAction);
        
        // Clean up the instance
        DestroyImmediate(prefabInstance);
        
        // Save assets
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log("Successfully added CharacterModelSwapper to MotorcycleWRider prefab");
        EditorUtility.DisplayDialog("Setup Complete", 
            "CharacterModelSwapper has been added to the MotorcycleWRider prefab.\n\n" +
            "The component will automatically swap character models based on the selected character when the game starts.", 
            "OK");
    }
    
    /// <summary>
    /// Recursively search for a rider model in the hierarchy
    /// </summary>
    /// <param name="parent">Parent transform to search from</param>
    /// <returns>Transform of the rider model or null if not found</returns>
    private static Transform FindRiderInHierarchy(Transform parent)
    {
        // Look for common rider-related names
        string[] riderNames = { "rider", "character", "person", "human", "body" };
        
        // Check current transform
        string lowerName = parent.name.ToLower();
        foreach (string riderName in riderNames)
        {
            if (lowerName.Contains(riderName))
            {
                // Check if this has a renderer (likely the actual model)
                if (parent.GetComponent<Renderer>() != null || parent.GetComponentInChildren<Renderer>() != null)
                {
                    return parent;
                }
            }
        }
        
        // Recursively check children
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform found = FindRiderInHierarchy(parent.GetChild(i));
            if (found != null)
            {
                return found;
            }
        }
        
        return null;
    }
    
    [MenuItem("TimeRiders/Setup Motorcycle/Show Prefab Info")]
    public static void ShowPrefabInfo()
    {
        string prefabPath = "Assets/SimpleMotorcyclePhysics/Prefabs/MotorcycleWRider.prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        
        if (prefab == null)
        {
            EditorUtility.DisplayDialog("Error", $"Could not find MotorcycleWRider prefab at {prefabPath}", "OK");
            return;
        }
        
        Debug.Log("=== MotorcycleWRider Prefab Hierarchy ===");
        LogHierarchy(prefab.transform, 0);
        
        // Check for CharacterModelSwapper
        CharacterModelSwapper swapper = prefab.GetComponent<CharacterModelSwapper>();
        if (swapper != null)
        {
            Debug.Log(" CharacterModelSwapper component found");
            Debug.Log($"  - Current Rider Model: {(swapper.currentRiderModel ? swapper.currentRiderModel.name : "None")}");
            Debug.Log($"  - Rider Parent: {(swapper.riderParent ? swapper.riderParent.name : "None")}");
            Debug.Log($"  - Default Rider Model: {(swapper.defaultRiderModel ? swapper.defaultRiderModel.name : "None")}");
        }
        else
        {
            Debug.Log(" CharacterModelSwapper component not found");
        }
    }
    
    private static void LogHierarchy(Transform transform, int depth)
    {
        string indent = new string(' ', depth * 2);
        string components = "";
        
        Component[] comps = transform.GetComponents<Component>();
        foreach (Component comp in comps)
        {
            if (comp != transform) // Skip Transform component
            {
                components += $"[{comp.GetType().Name}] ";
            }
        }
        
        Debug.Log($"{indent}{transform.name} {components}");
        
        for (int i = 0; i < transform.childCount; i++)
        {
            LogHierarchy(transform.GetChild(i), depth + 1);
        }
    }
}
#endif
