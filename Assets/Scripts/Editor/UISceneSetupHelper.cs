#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem.UI;

/// <summary>
/// Editor utility to ensure UI scenes have all necessary components for proper interaction
/// </summary>
public class UISceneSetupHelper : EditorWindow
{
    [MenuItem("TimeRiders/Setup UI/Check Scene Requirements")]
    public static void CheckSceneRequirements()
    {
        bool hasIssues = false;
        string report = "=== UI Scene Requirements Check ===\n\n";
        
        // Check for EventSystem
        EventSystem eventSystem = FindFirstObjectByType<EventSystem>();
        if (eventSystem == null)
        {
            report += "❌ MISSING: EventSystem - Required for UI input handling\n";
            hasIssues = true;
        }
        else
        {
            report += "✅ EventSystem found\n";
            
            // Check for input modules
            InputSystemUIInputModule inputModule = eventSystem.GetComponent<InputSystemUIInputModule>();
            
            if (inputModule == null)
            {
                report += "⚠️  WARNING: No InputSystemUIInputModule (new input system UI)\n";
            }
            else
            {
                report += "✅ InputSystemUIInputModule found\n";
            }
        }
        
        // Check for Canvas
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            report += "❌ MISSING: Canvas - Required for UI rendering\n";
            hasIssues = true;
        }
        else
        {
            report += "✅ Canvas found\n";
            
            // Check Canvas settings
            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                report += "✅ Canvas render mode: Screen Space - Overlay (good for UI)\n";
            }
            else
            {
                report += $"⚠️  Canvas render mode: {canvas.renderMode} (may need adjustment)\n";
            }
            
            // Check for GraphicRaycaster
            GraphicRaycaster raycaster = canvas.GetComponent<GraphicRaycaster>();
            if (raycaster == null)
            {
                report += "❌ MISSING: GraphicRaycaster on Canvas - Required for UI interaction\n";
                hasIssues = true;
            }
            else
            {
                report += "✅ GraphicRaycaster found on Canvas\n";
            }
        }
        
        // Check buttons
        Button[] buttons = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        report += $"\n📱 Found {buttons.Length} buttons in scene:\n";
        
        int interactableButtons = 0;
        int raycastTargetButtons = 0;
        
        foreach (Button button in buttons)
        {
            string buttonStatus = $"  • {button.name}: ";
            
            if (button.interactable)
            {
                buttonStatus += "Interactable ✅";
                interactableButtons++;
            }
            else
            {
                buttonStatus += "NOT Interactable ❌";
            }
            
            Image buttonImage = button.GetComponent<Image>();
            if (buttonImage != null && buttonImage.raycastTarget)
            {
                buttonStatus += ", Raycast Target ✅";
                raycastTargetButtons++;
            }
            else
            {
                buttonStatus += ", NO Raycast Target ❌";
            }
            
            buttonStatus += $", Active: {button.gameObject.activeInHierarchy}";
            report += buttonStatus + "\n";
        }
        
        report += $"\nSummary: {interactableButtons}/{buttons.Length} buttons interactable, {raycastTargetButtons}/{buttons.Length} have raycast targets\n";
        
        // Final recommendation
        if (hasIssues)
        {
            report += "\n🔧 RECOMMENDATION: Run 'Fix Scene Requirements' to automatically fix issues.";
        }
        else
        {
            report += "\n🎉 Scene looks good for UI interaction!";
        }
        
        Debug.Log(report);
        EditorUtility.DisplayDialog("UI Scene Requirements", report, "OK");
    }
    
    [MenuItem("TimeRiders/Setup UI/Fix Scene Requirements")]
    public static void FixSceneRequirements()
    {
        int fixesApplied = 0;
        string report = "=== Fixing UI Scene Requirements ===\n\n";
        
        // Ensure EventSystem exists
        EventSystem eventSystem = FindFirstObjectByType<EventSystem>();
        if (eventSystem == null)
        {
            GameObject eventSystemGO = new GameObject("EventSystem");
            eventSystem = eventSystemGO.AddComponent<EventSystem>();
            eventSystemGO.AddComponent<InputSystemUIInputModule>();
            report += "✅ Created EventSystem with new input system UI\n";
            fixesApplied++;
        }
        else
        {
            // Ensure input modules exist
            InputSystemUIInputModule inputModule = eventSystem.GetComponent<InputSystemUIInputModule>();
            if (inputModule == null)
            {
                eventSystem.gameObject.AddComponent<InputSystemUIInputModule>();
                report += "✅ Added InputSystemUIInputModule\n";
                fixesApplied++;
            }
        }
        
        // Ensure Canvas has GraphicRaycaster
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas != null)
        {
            GraphicRaycaster raycaster = canvas.GetComponent<GraphicRaycaster>();
            if (raycaster == null)
            {
                canvas.gameObject.AddComponent<GraphicRaycaster>();
                report += "✅ Added GraphicRaycaster to Canvas\n";
                fixesApplied++;
            }
        }
        
        // Fix button settings
        Button[] buttons = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        int buttonsFixed = 0;
        
        foreach (Button button in buttons)
        {
            bool buttonFixed = false;
            
            if (!button.interactable)
            {
                button.interactable = true;
                buttonFixed = true;
            }
            
            Image buttonImage = button.GetComponent<Image>();
            if (buttonImage != null && !buttonImage.raycastTarget)
            {
                buttonImage.raycastTarget = true;
                buttonFixed = true;
            }
            
            if (buttonFixed)
            {
                buttonsFixed++;
            }
        }
        
        if (buttonsFixed > 0)
        {
            report += $"✅ Fixed {buttonsFixed} buttons (enabled interactable and raycast target)\n";
            fixesApplied++;
        }
        
        // Mark scene as dirty
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        
        report += $"\n🎉 Applied {fixesApplied} fixes to the scene!";
        
        if (fixesApplied > 0)
        {
            report += "\n\nYour UI should now respond to clicks properly.";
        }
        else
        {
            report += "\n\nNo fixes were needed - scene was already properly configured.";
        }
        
        Debug.Log(report);
        EditorUtility.DisplayDialog("UI Scene Fixed", report, "OK");
    }
    
    [MenuItem("TimeRiders/Setup UI/Create Complete UI Scene")]
    public static void CreateCompleteUIScene()
    {
        // This creates a complete UI scene from scratch
        string report = "=== Creating Complete UI Scene ===\n\n";
        
        // Create EventSystem
        if (FindFirstObjectByType<EventSystem>() == null)
        {
            GameObject eventSystemGO = new GameObject("EventSystem");
            eventSystemGO.AddComponent<EventSystem>();
            eventSystemGO.AddComponent<InputSystemUIInputModule>();
            report += "✅ Created EventSystem\n";
        }
        
        // Create Canvas
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasGO = new GameObject("Canvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
            report += "✅ Created Canvas with proper components\n";
        }
        
        // Create MainMenuManager
        if (FindFirstObjectByType<MainMenuManager>() == null)
        {
            GameObject managerGO = new GameObject("MainMenuManager");
            managerGO.AddComponent<MainMenuManager>();
            report += "✅ Created MainMenuManager\n";
        }
        
        report += "\n🎉 Complete UI scene created! Add your buttons as children of the Canvas.";
        
        Debug.Log(report);
        EditorUtility.DisplayDialog("UI Scene Created", report, "OK");
    }
}
#endif
