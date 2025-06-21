#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

/// <summary>
/// Editor utility to add visual effects to buttons in scenes
/// </summary>
public class ButtonEffectsHelper : EditorWindow
{
    [MenuItem("TimeRiders/Setup UI/Add Button Visual Effects")]
    public static void AddButtonEffectsToScene()
    {
        // Find all buttons in the current scene
        Button[] allButtons = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        
        if (allButtons.Length == 0)
        {
            EditorUtility.DisplayDialog("No Buttons Found", 
                "No buttons were found in the current scene. Please open a scene with UI buttons first.", 
                "OK");
            return;
        }
        
        int buttonsProcessed = 0;
        
        foreach (Button button in allButtons)
        {
            // Add ButtonVisualEffects component if it doesn't exist
            ButtonVisualEffects visualEffects = button.GetComponent<ButtonVisualEffects>();
            if (visualEffects == null)
            {
                visualEffects = button.gameObject.AddComponent<ButtonVisualEffects>();
                buttonsProcessed++;
                
                // Configure default settings
                ConfigureButtonDefaults(visualEffects, button);
                
                Debug.Log($"Added visual effects to button: {button.name}");
            }
        }
        
        // Also add ButtonStyleManager to the scene if it doesn't exist
        ButtonStyleManager styleManager = FindFirstObjectByType<ButtonStyleManager>();
        if (styleManager == null)
        {
            GameObject styleManagerGO = new GameObject("ButtonStyleManager");
            styleManager = styleManagerGO.AddComponent<ButtonStyleManager>();
            Debug.Log("Added ButtonStyleManager to scene");
        }
        
        // Apply styles to all buttons
        styleManager.ApplyStylesToAllButtons();
        
        // Mark scene as dirty
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        
        string message = $"Successfully processed {allButtons.Length} buttons:\n" +
                        $"• Added visual effects to {buttonsProcessed} buttons\n" +
                        $"• Applied consistent styling to all buttons\n" +
                        $"• Added ButtonStyleManager to scene\n\n" +
                        "Buttons now have:\n" +
                        "• Hover effects (scale + color change)\n" +
                        "• Press feedback (scale down)\n" +
                        "• Keyboard navigation support\n" +
                        "• Smooth animations";
        
        EditorUtility.DisplayDialog("Button Effects Applied", message, "OK");
    }
    
    [MenuItem("TimeRiders/Setup UI/Configure Button Colors")]
    public static void ConfigureButtonColors()
    {
        ButtonStyleManager styleManager = FindFirstObjectByType<ButtonStyleManager>();
        if (styleManager == null)
        {
            EditorUtility.DisplayDialog("No Style Manager", 
                "No ButtonStyleManager found in scene. Please run 'Add Button Visual Effects' first.", 
                "OK");
            return;
        }
        
        // Select the ButtonStyleManager in the inspector so user can configure colors
        Selection.activeGameObject = styleManager.gameObject;
        
        EditorUtility.DisplayDialog("Configure Colors", 
            "ButtonStyleManager is now selected in the Inspector.\n\n" +
            "You can customize:\n" +
            "• Normal, Hover, Pressed, Selected colors\n" +
            "• Primary button color (Start, Play)\n" +
            "• Secondary button color (Settings, Back)\n" +
            "• Danger button color (Exit, Quit)\n" +
            "• Animation settings (scale, duration)", 
            "OK");
    }
    
    [MenuItem("TimeRiders/Setup UI/Test Button Effects")]
    public static void TestButtonEffects()
    {
        if (!Application.isPlaying)
        {
            EditorUtility.DisplayDialog("Test Mode", 
                "Enter Play Mode to test button hover and click effects.\n\n" +
                "You should see:\n" +
                "• Buttons scale up on hover\n" +
                "• Buttons scale down when pressed\n" +
                "• Color changes for different states\n" +
                "• Smooth animations between states", 
                "OK");
            return;
        }
        
        Button[] buttons = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (buttons.Length > 0)
        {
            // Simulate hover effect on first button
            ButtonVisualEffects effects = buttons[0].GetComponent<ButtonVisualEffects>();
            if (effects != null)
            {
                effects.SetHoverState(true);
                Debug.Log($"Testing hover effect on button: {buttons[0].name}");
            }
        }
    }
    
    private static void ConfigureButtonDefaults(ButtonVisualEffects visualEffects, Button button)
    {
        // Set reasonable defaults for motorcycle racing game
        // These will be overridden by ButtonStyleManager, but provide fallbacks
        
        // Determine button category and set appropriate colors
        string buttonName = button.name.ToLower();
        Text buttonText = button.GetComponentInChildren<Text>();
        string buttonTextContent = buttonText != null ? buttonText.text.ToLower() : "";
        
        Color normalColor = new Color(0.2f, 0.3f, 0.5f, 1f); // Default blue
        Color hoverColor = new Color(0.3f, 0.5f, 0.8f, 1f);
        Color pressedColor = new Color(0.1f, 0.2f, 0.4f, 1f);
        Color selectedColor = new Color(0.4f, 0.6f, 0.9f, 1f);
        
        // Primary buttons (green)
        if (buttonName.Contains("start") || buttonName.Contains("play") || buttonName.Contains("continue") ||
            buttonTextContent.Contains("start") || buttonTextContent.Contains("play") || buttonTextContent.Contains("continue"))
        {
            normalColor = new Color(0.2f, 0.6f, 0.2f, 1f);
            hoverColor = new Color(0.3f, 0.8f, 0.3f, 1f);
            pressedColor = new Color(0.1f, 0.4f, 0.1f, 1f);
            selectedColor = new Color(0.4f, 0.9f, 0.4f, 1f);
        }
        // Danger buttons (red)
        else if (buttonName.Contains("exit") || buttonName.Contains("quit") ||
                 buttonTextContent.Contains("exit") || buttonTextContent.Contains("quit"))
        {
            normalColor = new Color(0.6f, 0.2f, 0.2f, 1f);
            hoverColor = new Color(0.8f, 0.3f, 0.3f, 1f);
            pressedColor = new Color(0.4f, 0.1f, 0.1f, 1f);
            selectedColor = new Color(0.9f, 0.4f, 0.4f, 1f);
        }
        
        // Apply colors to the button's Image component
        Image buttonImage = button.GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.color = normalColor;
        }
        
        // Set the colors on the visual effects component
        visualEffects.SetColors(normalColor, hoverColor, pressedColor, selectedColor);
    }
}
#endif
