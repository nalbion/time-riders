using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuickUISetup : MonoBehaviour
{
    void Start()
    {
        SetupBasicUI();
    }
    
    void SetupBasicUI()
    {
        // Ensure EventSystem exists for UI interaction
        var eventSystem = FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>();
        if (eventSystem == null)
        {
            GameObject esObj = new GameObject("EventSystem");
            var es = esObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            // Use InputSystemUIInputModule for the new Input System
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
            esObj.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
#else
            esObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
#endif
        }

        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
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
    
    void CreateStartButton(Transform parent)
    {
        GameManager gm = FindFirstObjectByType<GameManager>();
        // Prevent duplicate button if already exists
        if (gm && gm.mainMenuUI != null)
            return;

        GameObject buttonObj = new GameObject("StartButton");
        buttonObj.transform.SetParent(parent);
        
        RectTransform rect = buttonObj.AddComponent<RectTransform>();
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(200, 50);
        
        Button button = buttonObj.AddComponent<Button>();
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = Color.green;
        
        // Add text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform);
        
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchoredPosition = Vector2.zero;
        textRect.sizeDelta = rect.sizeDelta;
        
        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = "START RACE";
        text.fontSize = 18;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.Center;
        
        // Assign to GameManager.mainMenuUI for state management
        if (gm)
        {
            gm.mainMenuUI = buttonObj;
        }

        // Add button functionality
        button.onClick.AddListener(() => {
            Debug.Log("START RACE button clicked");
            buttonObj.SetActive(false); // Hide immediately on click
            var gameManager = FindFirstObjectByType<GameManager>();
            if (gameManager)
            {
                Debug.Log("GameManager found, calling BeginRace()");
                gameManager.BeginRace();
            }
            else
            {
                Debug.LogWarning("GameManager not found when clicking START RACE");
            }
        });

        // Add instructions label below the button
        GameObject instructionsObj = new GameObject("Instructions");
        instructionsObj.transform.SetParent(parent);
        RectTransform instructionsRect = instructionsObj.AddComponent<RectTransform>();
        instructionsRect.anchorMin = new Vector2(0.5f, 0.5f);
        instructionsRect.anchorMax = new Vector2(0.5f, 0.5f);
        instructionsRect.pivot = new Vector2(0.5f, 1f);
        instructionsRect.anchoredPosition = new Vector2(0, -60); // 60 pixels below button
        instructionsRect.sizeDelta = new Vector2(500, 80);

        TextMeshProUGUI instructionsText = instructionsObj.AddComponent<TextMeshProUGUI>();
        instructionsText.text = "<b>Controls:</b>\nWASD / Arrow keys = Move\nSpace = Jump\nR = Reset Position\nEsc = Menu";
        instructionsText.fontSize = 18;
        instructionsText.color = Color.white;
        instructionsText.alignment = TextAlignmentOptions.Top | TextAlignmentOptions.Center;
        instructionsText.enableWordWrapping = true;
    }
    
    void CreateHUD(Transform parent)
    {
        GameObject hudObj = new GameObject("HUD");
        hudObj.transform.SetParent(parent);
        hudObj.SetActive(false); // Hidden initially
        
        // Timer
        var timer = CreateHUDElement(hudObj.transform, "Timer", "05:00", new Vector2(-300, 250));
        // Speed
        var speed = CreateHUDElement(hudObj.transform, "Speed", "0 km/h", new Vector2(300, 250));
        // Health bar would go here

        // Wire up to GameManager, retry if not found
        StartCoroutine(AssignHUDToGameManagerWhenReady(hudObj, timer, speed));
    }

    System.Collections.IEnumerator AssignHUDToGameManagerWhenReady(GameObject hudObj, TMPro.TextMeshProUGUI timer, TMPro.TextMeshProUGUI speed)
    {
        GameManager gm = null;
        while ((gm = FindFirstObjectByType<GameManager>()) == null)
        {
            yield return null; // Wait a frame
        }
        gm.gameUI = hudObj;
        gm.timerText = timer;
        gm.speedText = speed;
    }
    
    TextMeshProUGUI CreateHUDElement(Transform parent, string name, string text, Vector2 position)
    {
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
