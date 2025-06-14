using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Quickly creates and initializes basic UI elements (canvas, start button, HUD) at runtime.
/// </summary>
public class QuickUISetup : MonoBehaviour {
    /// <summary>
    /// Creates a Start button and instructions label under the given parent transform.
    /// </summary>
    /// <param name="parent">Parent transform (usually the Canvas)</param>
    public void CreateStartButton(Transform parent) {
        GameManager gm = FindFirstObjectByType<GameManager>();
        // Prevent duplicate button if already exists
        if (gm && gm.mainMenuUI != null)
            return;

        GameObject buttonObj = CreateButtonObject(parent);
        TextMeshProUGUI text = AddButtonLabel(buttonObj, "START RACE");
        Button button = buttonObj.GetComponent<Button>();
        WireButtonEvents(button, buttonObj);

        // Assign to GameManager.mainMenuUI for state management
        if (gm) {
            gm.mainMenuUI = buttonObj;
        }
    }

    private GameObject CreateButtonObject(Transform parent) {
        GameObject buttonObj = new GameObject("StartButton");
        buttonObj.transform.SetParent(parent);
        
        RectTransform rect = buttonObj.AddComponent<RectTransform>();
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(200, 50);
        
        Button button = buttonObj.AddComponent<Button>();
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = Color.green;
        buttonObj.AddComponent<CanvasRenderer>();
        return buttonObj;
    }

    private TextMeshProUGUI AddButtonLabel(GameObject buttonObj, string labelText) {
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform);
        
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchoredPosition = Vector2.zero;
        textRect.sizeDelta = buttonObj.GetComponent<RectTransform>().sizeDelta;
        
        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = labelText;
        text.fontSize = 18;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.Center;
        return text;
    }

    private void WireButtonEvents(Button button, GameObject buttonObj) {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => {
            GameLogger.Info("QuickUISetup", "START RACE button clicked");
            buttonObj.SetActive(false); // Hide immediately on click
            var gameManager = FindFirstObjectByType<GameManager>();
            if (gameManager) {
                GameLogger.Info("QuickUISetup", "GameManager found, calling BeginRace()");
                gameManager.BeginRace();
            } else {
                GameLogger.Warning("QuickUISetup", "GameManager not found when clicking START RACE");
            }
        });
    }

    /// <summary>
    /// Unity Start method. Sets up basic UI on scene start.
    /// </summary>
    void Start() {
        SetupBasicUI();
    }
    
    /// <summary>
    /// Sets up the EventSystem, Canvas, Start button, and HUD.
    /// </summary>
    void SetupBasicUI() {
        // Ensure EventSystem exists for UI interaction
        var eventSystem = FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>();
        if (eventSystem == null) {
            GameObject esObj = new GameObject("EventSystem");
            var es = esObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
            esObj.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
#else
            esObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
#endif
        }

        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null) {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }
        
        // Create simple start button
        CreateStartButton(canvas.transform);
        
        // Create basic HUD
        CreateHUD(canvas.transform);
    }
    
    
    /// <summary>
    /// Creates a basic HUD (timer, speed) and wires it to the GameManager.
    /// </summary>
    /// <param name="parent">Parent transform (Canvas)</param>
    void CreateHUD(Transform parent) {
        GameObject hudObj = new GameObject("HUD");
        hudObj.transform.SetParent(parent);
        hudObj.SetActive(false); // Hidden initially
        
        // Timer
        // var timer = CreateHUDElement(hudObj.transform, "Timer", "05:00", new Vector2(-300, 250));
        // // Speed
        // var speed = CreateHUDElement(hudObj.transform, "Speed", "0 km/h", new Vector2(300, 250));
        // // Health bar would go here

        // // Wire up to GameManager, retry if not found
        // StartCoroutine(AssignHUDToGameManagerWhenReady(hudObj, timer, speed));
    }

    /// <summary>
    /// Coroutine that waits for GameManager and assigns HUD references.
    /// </summary>
    /// <param name="hudObj">HUD GameObject</param>
    /// <param name="timer">Timer TextMeshProUGUI</param>
    /// <param name="speed">Speed TextMeshProUGUI</param>
    System.Collections.IEnumerator AssignHUDToGameManagerWhenReady(GameObject hudObj, TMPro.TextMeshProUGUI timer, TMPro.TextMeshProUGUI speed) {
        GameManager gm = null;
        while ((gm = FindFirstObjectByType<GameManager>()) == null) {
            yield return null; // Wait a frame
        }
        gm.gameUI = hudObj;
        gm.timerText = timer;
        gm.speedText = speed;
    }
    
    /// <summary>
    /// Creates a HUD text element (e.g., timer or speed) at the given position.
    /// </summary>
    /// <param name="parent">Parent transform (HUD)</param>
    /// <param name="name">Element name</param>
    /// <param name="text">Initial text</param>
    /// <param name="position">Anchored position</param>
    /// <returns>TextMeshProUGUI component for the element</returns>
    TextMeshProUGUI CreateHUDElement(Transform parent, string name, string text, Vector2 position) {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent);
        
        RectTransform rect = textObj.AddComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(200, 50);
        
        TextMeshProUGUI textComp = textObj.AddComponent<TextMeshProUGUI>();
        textComp.text = text;
        textComp.fontSize = 24;
        textComp.color = Color.white;
        textComp.alignment = TextAlignmentOptions.Center;
        
        // Add outline for better visibility
        textComp.fontMaterial = Resources.Load<Material>("Fonts & Materials/LiberationSans SDF - Outline");
        return textComp;
    }
}
