using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages consistent button styling across the UI
/// </summary>
public class ButtonStyleManager : MonoBehaviour
{
    [Header("Button Style Settings")]
    [SerializeField] private bool autoApplyToAllButtons = true;
    [SerializeField] private bool applyOnStart = true;
    
    [Header("Visual Effect Settings")]
    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private float pressScale = 0.95f;
    [SerializeField] private float animationDuration = 0.2f;
    
    [Header("Color Scheme")]
    [SerializeField] private Color normalColor = new Color(0.2f, 0.3f, 0.5f, 1f); // Dark blue
    [SerializeField] private Color hoverColor = new Color(0.3f, 0.5f, 0.8f, 1f); // Bright blue
    [SerializeField] private Color pressedColor = new Color(0.1f, 0.2f, 0.4f, 1f); // Darker blue
    [SerializeField] private Color selectedColor = new Color(0.4f, 0.6f, 0.9f, 1f); // Light blue
    
    [Header("Button Categories")]
    [SerializeField] private Color primaryButtonColor = new Color(0.2f, 0.6f, 0.2f, 1f); // Green for primary actions
    [SerializeField] private Color secondaryButtonColor = new Color(0.6f, 0.6f, 0.2f, 1f); // Yellow for secondary actions
    [SerializeField] private Color dangerButtonColor = new Color(0.6f, 0.2f, 0.2f, 1f); // Red for destructive actions
    
    private void Start()
    {
        if (applyOnStart && autoApplyToAllButtons)
        {
            ApplyStylesToAllButtons();
        }
    }
    
    /// <summary>
    /// Apply visual effects and styling to all buttons in the scene
    /// </summary>
    [ContextMenu("Apply Styles to All Buttons")]
    public void ApplyStylesToAllButtons()
    {
        Button[] allButtons = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        
        int activeButtons = 0;
        int inactiveButtons = 0;
        
        foreach (Button button in allButtons)
        {
            if (button.gameObject.activeInHierarchy)
            {
                activeButtons++;
            }
            else
            {
                inactiveButtons++;
            }
            
            ApplyStyleToButton(button);
        }
        
        Debug.Log($"Applied styles to {allButtons.Length} buttons ({activeButtons} active, {inactiveButtons} inactive)");
    }
    
    /// <summary>
    /// Apply visual effects to a specific button
    /// </summary>
    /// <param name="button">The button to style</param>
    public void ApplyStyleToButton(Button button)
    {
        if (button == null) return;
        
        // Add ButtonVisualEffects component if it doesn't exist
        ButtonVisualEffects visualEffects = button.GetComponent<ButtonVisualEffects>();
        if (visualEffects == null)
        {
            visualEffects = button.gameObject.AddComponent<ButtonVisualEffects>();
        }
        
        // Determine button category and apply appropriate styling
        ButtonCategory category = DetermineButtonCategory(button);
        ApplyButtonCategory(button, category);
        
        string status = button.gameObject.activeInHierarchy ? "active" : "inactive";
        Debug.Log($"Applied {category} style to {status} button: {button.name}");
    }
    
    private ButtonCategory DetermineButtonCategory(Button button)
    {
        string buttonName = button.name.ToLower();
        Text buttonText = button.GetComponentInChildren<Text>();
        string buttonTextContent = buttonText != null ? buttonText.text.ToLower() : "";
        
        // Primary buttons (main actions)
        if (buttonName.Contains("start") || buttonName.Contains("play") || buttonName.Contains("continue") ||
            buttonTextContent.Contains("start") || buttonTextContent.Contains("play") || buttonTextContent.Contains("continue"))
        {
            return ButtonCategory.Primary;
        }
        // Secondary buttons (navigation, settings)
        else if (buttonName.Contains("settings") || buttonName.Contains("options") || buttonName.Contains("back") ||
                 buttonTextContent.Contains("settings") || buttonTextContent.Contains("options") || buttonTextContent.Contains("back"))
        {
            return ButtonCategory.Secondary;
        }
        // Danger buttons (exit, quit, delete)
        else if (buttonName.Contains("exit") || buttonName.Contains("quit") || buttonName.Contains("delete") ||
                 buttonTextContent.Contains("exit") || buttonTextContent.Contains("quit") || buttonTextContent.Contains("delete"))
        {
            return ButtonCategory.Danger;
        }
        else
        {
            return ButtonCategory.Primary; // Default to primary if no match found
        }
    }
    
    private void ConfigureButtonVisualEffects(ButtonVisualEffects visualEffects, Button button)
    {
        // Use reflection to set private fields (since they're SerializeField)
        var fields = typeof(ButtonVisualEffects).GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        foreach (var field in fields)
        {
            switch (field.Name)
            {
                case "hoverScale":
                    field.SetValue(visualEffects, hoverScale);
                    break;
                case "pressScale":
                    field.SetValue(visualEffects, pressScale);
                    break;
                case "animationDuration":
                    field.SetValue(visualEffects, animationDuration);
                    break;
                case "enableScaleEffect":
                    field.SetValue(visualEffects, true);
                    break;
                case "enableColorEffect":
                    field.SetValue(visualEffects, true);
                    break;
                case "enableShadowEffect":
                    field.SetValue(visualEffects, true);
                    break;
                case "enableSoundEffect":
                    field.SetValue(visualEffects, true);
                    break;
            }
        }
    }
    
    private void SetButtonColors(Button button, ButtonVisualEffects visualEffects)
    {
        Color baseColor = normalColor;
        
        // Determine button type based on name or text content
        string buttonName = button.name.ToLower();
        Text buttonText = button.GetComponentInChildren<Text>();
        string buttonTextContent = buttonText != null ? buttonText.text.ToLower() : "";
        
        // Primary buttons (main actions)
        if (buttonName.Contains("start") || buttonName.Contains("play") || buttonName.Contains("continue") ||
            buttonTextContent.Contains("start") || buttonTextContent.Contains("play") || buttonTextContent.Contains("continue"))
        {
            baseColor = primaryButtonColor;
        }
        // Secondary buttons (navigation, settings)
        else if (buttonName.Contains("settings") || buttonName.Contains("options") || buttonName.Contains("back") ||
                 buttonTextContent.Contains("settings") || buttonTextContent.Contains("options") || buttonTextContent.Contains("back"))
        {
            baseColor = secondaryButtonColor;
        }
        // Danger buttons (exit, quit, delete)
        else if (buttonName.Contains("exit") || buttonName.Contains("quit") || buttonName.Contains("delete") ||
                 buttonTextContent.Contains("exit") || buttonTextContent.Contains("quit") || buttonTextContent.Contains("delete"))
        {
            baseColor = dangerButtonColor;
        }
        
        // Calculate hover, pressed, and selected colors based on the base color
        Color hover = LightenColor(baseColor, 0.3f);
        Color pressed = DarkenColor(baseColor, 0.3f);
        Color selected = LightenColor(baseColor, 0.2f);
        
        // Apply colors to the visual effects component
        visualEffects.SetColors(baseColor, hover, pressed, selected);
        
        // Also set the button's Image component color
        Image buttonImage = button.GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.color = baseColor;
        }
    }
    
    private Color LightenColor(Color color, float amount)
    {
        return Color.Lerp(color, Color.white, amount);
    }
    
    private Color DarkenColor(Color color, float amount)
    {
        return Color.Lerp(color, Color.black, amount);
    }
    
    /// <summary>
    /// Apply specific style to a button by category
    /// </summary>
    public void ApplyButtonCategory(Button button, ButtonCategory category)
    {
        if (button == null) return;
        
        ButtonVisualEffects visualEffects = button.GetComponent<ButtonVisualEffects>();
        if (visualEffects == null)
        {
            visualEffects = button.gameObject.AddComponent<ButtonVisualEffects>();
        }
        
        Color baseColor = normalColor;
        switch (category)
        {
            case ButtonCategory.Primary:
                baseColor = primaryButtonColor;
                break;
            case ButtonCategory.Secondary:
                baseColor = secondaryButtonColor;
                break;
            case ButtonCategory.Danger:
                baseColor = dangerButtonColor;
                break;
        }
        
        Color hover = LightenColor(baseColor, 0.3f);
        Color pressed = DarkenColor(baseColor, 0.3f);
        Color selected = LightenColor(baseColor, 0.2f);
        
        visualEffects.SetColors(baseColor, hover, pressed, selected);
        
        Image buttonImage = button.GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.color = baseColor;
        }
    }
}

public enum ButtonCategory
{
    Primary,    // Main actions (Start, Play, Continue)
    Secondary,  // Navigation (Settings, Back, Options)
    Danger      // Destructive actions (Exit, Quit, Delete)
}
