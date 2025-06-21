#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Editor helper to create and setup the MainMenu, CharacterSelect and CourseSelection scenes
/// </summary>
public class SceneSetupHelper : EditorWindow
{
    [MenuItem("TimeRiders/Setup Scenes")]
    public static void ShowWindow()
    {
        GetWindow<SceneSetupHelper>("Scene Setup Helper");
    }
    
    private void OnGUI()
    {
        GUILayout.Label("Time Riders Scene Setup", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        if (GUILayout.Button("Create MainMenu Scene", GUILayout.Height(30)))
        {
            CreateMainMenuSceneInternal();
        }
        
        GUILayout.Space(5);
        
        if (GUILayout.Button("Create CharacterSelect Scene", GUILayout.Height(30)))
        {
            CreateCharacterSelectSceneInternal();
        }
        
        GUILayout.Space(5);
        
        if (GUILayout.Button("Create CourseSelection Scene", GUILayout.Height(30)))
        {
            CreateCourseSelectionSceneInternal();
        }
        
        GUILayout.Space(10);
        GUILayout.Label("Instructions:", EditorStyles.boldLabel);
        GUILayout.TextArea(
            "1. Click 'Create MainMenu Scene' to set up the main menu\n" +
            "2. Click 'Create CharacterSelect Scene' to set up character selection\n" +
            "3. Click 'Create CourseSelection Scene' to set up course selection\n" +
            "4. Assign the created scripts to the appropriate GameObjects\n" +
            "5. Configure UI references in the Inspector\n" +
            "6. Add these scenes to Build Settings"
        );
    }
    
    private void CreateMainMenuSceneInternal()
    {
        // Create new scene
        var newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        
        // Setup Canvas
        GameObject canvasGO = new GameObject("MainMenu Canvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();
        
        // Add MainMenuManager
        GameObject managerGO = new GameObject("MainMenuManager");
        managerGO.AddComponent<MainMenuManager>();
        
        // Create UI structure
        CreateMainMenuUI(canvasGO.transform);
        
        // Save scene
        string scenePath = "Assets/Scenes/MainMenu.unity";
        EditorSceneManager.SaveScene(newScene, scenePath);
        
        Debug.Log($"MainMenu scene created at: {scenePath}");
        EditorUtility.DisplayDialog("Scene Created", "MainMenu scene has been created!\n\nNext steps:\n1. Assign UI references in MainMenuManager\n2. Add background image\n3. Style the UI elements", "OK");
    }
    
    private void CreateCharacterSelectSceneInternal()
    {
        // Create new scene
        var newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        
        // Setup Canvas
        GameObject canvasGO = new GameObject("CharacterSelect Canvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();
        
        // Add CharacterSelectionManager
        GameObject managerGO = new GameObject("CharacterSelectionManager");
        managerGO.AddComponent<CharacterSelectionManager>();
        
        // Create UI structure
        CreateCharacterSelectUI(canvasGO.transform);
        
        // Save scene
        string scenePath = "Assets/Scenes/CharacterSelect.unity";
        EditorSceneManager.SaveScene(newScene, scenePath);
        
        Debug.Log($"CharacterSelect scene created at: {scenePath}");
        EditorUtility.DisplayDialog("Scene Created", "CharacterSelect scene has been created!\n\nNext steps:\n1. Assign UI references in CharacterSelectionManager\n2. Create CharacterButton prefab\n3. Create CharacterData assets", "OK");
    }
    
    private void CreateCourseSelectionSceneInternal()
    {
        // Create new scene
        var newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        
        // Setup Canvas
        GameObject canvasGO = new GameObject("CourseSelection Canvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();
        
        // Add CourseSelectionManager
        GameObject managerGO = new GameObject("CourseSelectionManager");
        managerGO.AddComponent<CourseSelectionManager>();
        
        // Create UI structure
        CreateCourseSelectionUI(canvasGO.transform);
        
        // Save scene
        string scenePath = "Assets/Scenes/CourseSelection.unity";
        EditorSceneManager.SaveScene(newScene, scenePath);
        
        Debug.Log($"CourseSelection scene created at: {scenePath}");
        EditorUtility.DisplayDialog("Scene Created", "CourseSelection scene has been created!\n\nNext steps:\n1. Assign UI references in CourseSelectionManager\n2. Create CourseButton prefab\n3. Create CourseData assets", "OK");
    }
    
    private void CreateMainMenuUI(Transform parent)
    {
        // Background with actual image
        GameObject bgGO = new GameObject("Background");
        bgGO.transform.SetParent(parent);
        Image bgImage = bgGO.AddComponent<Image>();
        
        // Try to load the background image
        string imagePath = "Assets/Images/884-nose-dive.png";
        Sprite backgroundSprite = AssetDatabase.LoadAssetAtPath<Sprite>(imagePath);
        
        if (backgroundSprite != null)
        {
            bgImage.sprite = backgroundSprite;
            bgImage.type = Image.Type.Simple;
            bgImage.preserveAspect = false; // Stretch to fill screen
            Debug.Log("MainMenu: Applied background image");
        }
        else
        {
            // Fallback to solid color if image not found
            bgImage.color = new Color(0.1f, 0.1f, 0.2f, 1f); // Dark blue background
            Debug.LogWarning("MainMenu: Background image not found, using solid color fallback");
        }
        
        RectTransform bgRect = bgGO.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        
        // Title
        GameObject titleGO = new GameObject("Title");
        titleGO.transform.SetParent(parent);
        TextMeshProUGUI titleText = titleGO.AddComponent<TextMeshProUGUI>();
        titleText.text = "TIME RIDERS";
        titleText.fontSize = 72;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = Color.white;
        RectTransform titleRect = titleGO.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.8f);
        titleRect.anchorMax = new Vector2(0.5f, 0.9f);
        titleRect.anchoredPosition = Vector2.zero;
        titleRect.sizeDelta = new Vector2(600, 100);
        
        // Quick Play Panel
        GameObject quickPlayPanel = CreateQuickPlayPanel(parent);
        
        // Main Buttons
        CreateMainMenuButton(parent, "New Game Button", "NEW GAME", new Vector2(0.5f, 0.5f), new Vector2(300, 60));
        CreateMainMenuButton(parent, "Settings Button", "SETTINGS", new Vector2(0.5f, 0.4f), new Vector2(300, 60));
        CreateMainMenuButton(parent, "Exit Button", "EXIT", new Vector2(0.5f, 0.3f), new Vector2(300, 60));
    }
    
    private GameObject CreateQuickPlayPanel(Transform parent)
    {
        GameObject panelGO = new GameObject("Quick Play Panel");
        panelGO.transform.SetParent(parent);
        Image panelImage = panelGO.AddComponent<Image>();
        panelImage.color = new Color(0.2f, 0.2f, 0.3f, 0.8f);
        RectTransform panelRect = panelGO.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.55f);
        panelRect.anchorMax = new Vector2(0.5f, 0.7f);
        panelRect.anchoredPosition = Vector2.zero;
        panelRect.sizeDelta = new Vector2(400, 120);
        
        // Quick play content
        GameObject titleGO = new GameObject("Quick Play Title");
        titleGO.transform.SetParent(panelGO.transform);
        TextMeshProUGUI titleText = titleGO.AddComponent<TextMeshProUGUI>();
        titleText.text = "CONTINUE LAST GAME";
        titleText.fontSize = 24;
        titleText.alignment = TextAlignmentOptions.Center;
        RectTransform titleRect = titleGO.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0.7f);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.offsetMin = Vector2.zero;
        titleRect.offsetMax = Vector2.zero;
        
        // Last game info
        GameObject infoGO = new GameObject("Last Game Info");
        infoGO.transform.SetParent(panelGO.transform);
        TextMeshProUGUI infoText = infoGO.AddComponent<TextMeshProUGUI>();
        infoText.text = "Character: [Name]\nMode: [Mode]\nTrack: [Track]";
        infoText.fontSize = 16;
        infoText.alignment = TextAlignmentOptions.Center;
        RectTransform infoRect = infoGO.GetComponent<RectTransform>();
        infoRect.anchorMin = new Vector2(0, 0.3f);
        infoRect.anchorMax = new Vector2(0.7f, 0.7f);
        infoRect.offsetMin = Vector2.zero;
        infoRect.offsetMax = Vector2.zero;
        
        // Start Race button
        CreateMainMenuButton(panelGO.transform, "Start Race Button", "START RACE", 
            new Vector2(0.85f, 0.5f), new Vector2(100, 40));
        
        return panelGO;
    }
    
    private void CreateMainMenuButton(Transform parent, string name, string text, Vector2 anchor, Vector2 size)
    {
        GameObject buttonGO = new GameObject(name);
        buttonGO.transform.SetParent(parent);
        
        Image buttonImage = buttonGO.AddComponent<Image>();
        buttonImage.color = new Color(0.3f, 0.6f, 0.9f, 1f);
        
        Button button = buttonGO.AddComponent<Button>();
        
        RectTransform buttonRect = buttonGO.GetComponent<RectTransform>();
        buttonRect.anchorMin = anchor;
        buttonRect.anchorMax = anchor;
        buttonRect.anchoredPosition = Vector2.zero;
        buttonRect.sizeDelta = size;
        
        // Button text
        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(buttonGO.transform);
        TextMeshProUGUI buttonText = textGO.AddComponent<TextMeshProUGUI>();
        buttonText.text = text;
        buttonText.fontSize = 24;
        buttonText.alignment = TextAlignmentOptions.Center;
        buttonText.color = Color.white;
        
        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
    }
    
    private void CreateCharacterSelectUI(Transform parent)
    {
        // Background
        GameObject bgGO = new GameObject("Background");
        bgGO.transform.SetParent(parent);
        Image bgImage = bgGO.AddComponent<Image>();
        bgImage.color = new Color(0.1f, 0.1f, 0.2f, 1f);
        RectTransform bgRect = bgGO.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        
        // Title
        GameObject titleGO = new GameObject("Title");
        titleGO.transform.SetParent(parent);
        TextMeshProUGUI titleText = titleGO.AddComponent<TextMeshProUGUI>();
        titleText.text = "SELECT CHARACTER";
        titleText.fontSize = 48;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = Color.white;
        RectTransform titleRect = titleGO.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.9f);
        titleRect.anchorMax = new Vector2(0.5f, 0.95f);
        titleRect.anchoredPosition = Vector2.zero;
        titleRect.sizeDelta = new Vector2(600, 60);
        
        // Character Info Panel (Left Side)
        CreateCharacterInfoPanel(parent);
        
        // 3D Carousel Area (Center)
        Create3DCarouselArea(parent);
        
        // Game Mode Panel (Right Side)
        CreateGameModePanel(parent);
        
        // Navigation Buttons
        CreateMainMenuButton(parent, "Back Button", "BACK", new Vector2(0.1f, 0.1f), new Vector2(150, 50));
        CreateMainMenuButton(parent, "Continue Button", "CONTINUE", new Vector2(0.9f, 0.1f), new Vector2(150, 50));
    }
    
    private void CreateCharacterInfoPanel(Transform parent)
    {
        GameObject panelGO = new GameObject("Character Info Panel");
        panelGO.transform.SetParent(parent);
        Image panelImage = panelGO.AddComponent<Image>();
        panelImage.color = new Color(0.2f, 0.2f, 0.3f, 0.8f);
        RectTransform panelRect = panelGO.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.05f, 0.2f);
        panelRect.anchorMax = new Vector2(0.35f, 0.8f);
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        
        // Character Portrait
        GameObject portraitGO = new GameObject("Character Portrait");
        portraitGO.transform.SetParent(panelGO.transform);
        Image portraitImage = portraitGO.AddComponent<Image>();
        portraitImage.color = Color.gray; // Placeholder
        RectTransform portraitRect = portraitGO.GetComponent<RectTransform>();
        portraitRect.anchorMin = new Vector2(0.1f, 0.6f);
        portraitRect.anchorMax = new Vector2(0.9f, 0.9f);
        portraitRect.offsetMin = Vector2.zero;
        portraitRect.offsetMax = Vector2.zero;
        
        // Character Name
        GameObject nameGO = new GameObject("Character Name Text");
        nameGO.transform.SetParent(panelGO.transform);
        TextMeshProUGUI nameText = nameGO.AddComponent<TextMeshProUGUI>();
        nameText.text = "Character Name";
        nameText.fontSize = 24;
        nameText.alignment = TextAlignmentOptions.Center;
        nameText.color = Color.white;
        RectTransform nameRect = nameGO.GetComponent<RectTransform>();
        nameRect.anchorMin = new Vector2(0.1f, 0.5f);
        nameRect.anchorMax = new Vector2(0.9f, 0.6f);
        nameRect.offsetMin = Vector2.zero;
        nameRect.offsetMax = Vector2.zero;
        
        // Ability Text
        GameObject abilityGO = new GameObject("Ability Text");
        abilityGO.transform.SetParent(panelGO.transform);
        TextMeshProUGUI abilityText = abilityGO.AddComponent<TextMeshProUGUI>();
        abilityText.text = "Character ability description";
        abilityText.fontSize = 14;
        abilityText.alignment = TextAlignmentOptions.TopLeft;
        abilityText.color = Color.white;
        RectTransform abilityRect = abilityGO.GetComponent<RectTransform>();
        abilityRect.anchorMin = new Vector2(0.1f, 0.3f);
        abilityRect.anchorMax = new Vector2(0.9f, 0.5f);
        abilityRect.offsetMin = Vector2.zero;
        abilityRect.offsetMax = Vector2.zero;
        
        // Stats Text
        GameObject statsGO = new GameObject("Stats Text");
        statsGO.transform.SetParent(panelGO.transform);
        TextMeshProUGUI statsText = statsGO.AddComponent<TextMeshProUGUI>();
        statsText.text = "Speed: 85\nHealth: 90\nJump: 75";
        statsText.fontSize = 12;
        statsText.alignment = TextAlignmentOptions.TopLeft;
        statsText.color = Color.white;
        RectTransform statsRect = statsGO.GetComponent<RectTransform>();
        statsRect.anchorMin = new Vector2(0.1f, 0.1f);
        statsRect.anchorMax = new Vector2(0.9f, 0.3f);
        statsRect.offsetMin = Vector2.zero;
        statsRect.offsetMax = Vector2.zero;
    }
    
    private void Create3DCarouselArea(Transform parent)
    {
        GameObject carouselGO = new GameObject("3D Carousel Area");
        carouselGO.transform.SetParent(parent);
        RectTransform carouselRect = carouselGO.GetComponent<RectTransform>();
        carouselRect.anchorMin = new Vector2(0.4f, 0.2f);
        carouselRect.anchorMax = new Vector2(0.6f, 0.8f);
        carouselRect.offsetMin = Vector2.zero;
        carouselRect.offsetMax = Vector2.zero;
        
        // Placeholder for 3D carousel - will be set up separately
        GameObject placeholderGO = new GameObject("Carousel Placeholder");
        placeholderGO.transform.SetParent(carouselGO.transform);
        Image placeholderImage = placeholderGO.AddComponent<Image>();
        placeholderImage.color = new Color(0.3f, 0.3f, 0.4f, 0.5f);
        RectTransform placeholderRect = placeholderGO.GetComponent<RectTransform>();
        placeholderRect.anchorMin = Vector2.zero;
        placeholderRect.anchorMax = Vector2.one;
        placeholderRect.offsetMin = Vector2.zero;
        placeholderRect.offsetMax = Vector2.zero;
        
        TextMeshProUGUI placeholderText = placeholderGO.AddComponent<TextMeshProUGUI>();
        placeholderText.text = "3D Character\nCarousel\n(Setup Required)";
        placeholderText.fontSize = 16;
        placeholderText.alignment = TextAlignmentOptions.Center;
        placeholderText.color = Color.white;
    }
    
    private void CreateGameModePanel(Transform parent)
    {
        GameObject panelGO = new GameObject("Game Mode Panel");
        panelGO.transform.SetParent(parent);
        Image panelImage = panelGO.AddComponent<Image>();
        panelImage.color = new Color(0.2f, 0.2f, 0.3f, 0.8f);
        RectTransform panelRect = panelGO.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.65f, 0.2f);
        panelRect.anchorMax = new Vector2(0.95f, 0.8f);
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        
        // Game Mode Label
        GameObject labelGO = new GameObject("Game Mode Label");
        labelGO.transform.SetParent(panelGO.transform);
        TextMeshProUGUI labelText = labelGO.AddComponent<TextMeshProUGUI>();
        labelText.text = "Game Mode";
        labelText.fontSize = 18;
        labelText.alignment = TextAlignmentOptions.Center;
        labelText.color = Color.white;
        RectTransform labelRect = labelGO.GetComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0.1f, 0.8f);
        labelRect.anchorMax = new Vector2(0.9f, 0.9f);
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;
        
        // Game Mode Dropdown
        GameObject dropdownGO = new GameObject("Game Mode Dropdown");
        dropdownGO.transform.SetParent(panelGO.transform);
        Image dropdownImage = dropdownGO.AddComponent<Image>();
        dropdownImage.color = Color.white;
        Dropdown dropdown = dropdownGO.AddComponent<Dropdown>();
        RectTransform dropdownRect = dropdownGO.GetComponent<RectTransform>();
        dropdownRect.anchorMin = new Vector2(0.1f, 0.7f);
        dropdownRect.anchorMax = new Vector2(0.9f, 0.8f);
        dropdownRect.offsetMin = Vector2.zero;
        dropdownRect.offsetMax = Vector2.zero;
        
        // Character Stats Sliders
        CreateStatSlider(panelGO.transform, "Speed Slider", new Vector2(0.1f, 0.5f), new Vector2(0.9f, 0.6f));
        CreateStatSlider(panelGO.transform, "Health Slider", new Vector2(0.1f, 0.4f), new Vector2(0.9f, 0.5f));
        CreateStatSlider(panelGO.transform, "Jump Slider", new Vector2(0.1f, 0.3f), new Vector2(0.9f, 0.4f));
    }
    
    private void CreateStatSlider(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax)
    {
        GameObject sliderGO = new GameObject(name);
        sliderGO.transform.SetParent(parent);
        RectTransform sliderRect = sliderGO.GetComponent<RectTransform>();
        sliderRect.anchorMin = anchorMin;
        sliderRect.anchorMax = anchorMax;
        sliderRect.offsetMin = Vector2.zero;
        sliderRect.offsetMax = Vector2.zero;
        
        Slider slider = sliderGO.AddComponent<Slider>();
        slider.minValue = 0;
        slider.maxValue = 100;
        slider.value = 50;
        
        // Background
        GameObject bgGO = new GameObject("Background");
        bgGO.transform.SetParent(sliderGO.transform);
        Image bgImage = bgGO.AddComponent<Image>();
        bgImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        RectTransform bgRect = bgGO.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        
        // Fill Area
        GameObject fillAreaGO = new GameObject("Fill Area");
        fillAreaGO.transform.SetParent(sliderGO.transform);
        RectTransform fillAreaRect = fillAreaGO.GetComponent<RectTransform>();
        fillAreaRect.anchorMin = Vector2.zero;
        fillAreaRect.anchorMax = Vector2.one;
        fillAreaRect.offsetMin = Vector2.zero;
        fillAreaRect.offsetMax = Vector2.zero;
        
        GameObject fillGO = new GameObject("Fill");
        fillGO.transform.SetParent(fillAreaGO.transform);
        Image fillImage = fillGO.AddComponent<Image>();
        fillImage.color = new Color(0.3f, 0.7f, 0.3f, 1f);
        RectTransform fillRect = fillGO.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;
        
        slider.fillRect = fillRect;
    }
    
    private void CreateCourseSelectionUI(Transform parent)
    {
        // Background
        GameObject bgGO = new GameObject("Background");
        bgGO.transform.SetParent(parent);
        Image bgImage = bgGO.AddComponent<Image>();
        bgImage.color = new Color(0.1f, 0.1f, 0.2f, 1f);
        RectTransform bgRect = bgGO.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        
        // Title
        GameObject titleGO = new GameObject("Title");
        titleGO.transform.SetParent(parent);
        TextMeshProUGUI titleText = titleGO.AddComponent<TextMeshProUGUI>();
        titleText.text = "SELECT COURSE";
        titleText.fontSize = 48;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = Color.white;
        RectTransform titleRect = titleGO.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.9f);
        titleRect.anchorMax = new Vector2(0.5f, 0.95f);
        titleRect.anchoredPosition = Vector2.zero;
        titleRect.sizeDelta = new Vector2(600, 60);
        
        // Course Preview Panel
        CreateCoursePreviewPanel(parent);
        
        // Course Grid
        CreateCourseGrid(parent);
        
        // Navigation Buttons
        CreateMainMenuButton(parent, "Back Button", "BACK", new Vector2(0.1f, 0.1f), new Vector2(150, 50));
        CreateMainMenuButton(parent, "Start Race Button", "START RACE", new Vector2(0.9f, 0.1f), new Vector2(150, 50));
    }
    
    private void CreateCoursePreviewPanel(Transform parent)
    {
        GameObject panelGO = new GameObject("Course Preview Panel");
        panelGO.transform.SetParent(parent);
        Image panelImage = panelGO.AddComponent<Image>();
        panelImage.color = new Color(0.2f, 0.2f, 0.3f, 0.9f);
        RectTransform panelRect = panelGO.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.05f, 0.5f);
        panelRect.anchorMax = new Vector2(0.45f, 0.85f);
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        
        // Preview Image
        GameObject previewGO = new GameObject("Course Preview Image");
        previewGO.transform.SetParent(panelGO.transform);
        Image previewImage = previewGO.AddComponent<Image>();
        previewImage.color = Color.gray; // Placeholder
        RectTransform previewRect = previewGO.GetComponent<RectTransform>();
        previewRect.anchorMin = new Vector2(0.05f, 0.4f);
        previewRect.anchorMax = new Vector2(0.95f, 0.95f);
        previewRect.offsetMin = Vector2.zero;
        previewRect.offsetMax = Vector2.zero;
        
        // Course Info
        GameObject infoGO = new GameObject("Course Info");
        infoGO.transform.SetParent(panelGO.transform);
        TextMeshProUGUI infoText = infoGO.AddComponent<TextMeshProUGUI>();
        infoText.text = "Course Name\nDifficulty: ★★☆\nDescription goes here...";
        infoText.fontSize = 18;
        infoText.alignment = TextAlignmentOptions.TopLeft;
        RectTransform infoRect = infoGO.GetComponent<RectTransform>();
        infoRect.anchorMin = new Vector2(0.05f, 0.05f);
        infoRect.anchorMax = new Vector2(0.95f, 0.35f);
        infoRect.offsetMin = Vector2.zero;
        infoRect.offsetMax = Vector2.zero;
    }
    
    private void CreateCourseGrid(Transform parent)
    {
        GameObject gridGO = new GameObject("Course Grid");
        gridGO.transform.SetParent(parent);
        RectTransform gridRect = gridGO.GetComponent<RectTransform>();
        gridRect.anchorMin = new Vector2(0.55f, 0.2f);
        gridRect.anchorMax = new Vector2(0.95f, 0.85f);
        gridRect.offsetMin = Vector2.zero;
        gridRect.offsetMax = Vector2.zero;
        
        // Add GridLayoutGroup
        GridLayoutGroup gridLayout = gridGO.AddComponent<GridLayoutGroup>();
        gridLayout.cellSize = new Vector2(150, 120);
        gridLayout.spacing = new Vector2(10, 10);
        gridLayout.startCorner = GridLayoutGroup.Corner.UpperLeft;
        gridLayout.startAxis = GridLayoutGroup.Axis.Horizontal;
        gridLayout.childAlignment = TextAnchor.UpperCenter;
        
        // Create sample course buttons
        for (int i = 0; i < 6; i++)
        {
            CreateCourseButton(gridGO.transform, $"Course {i + 1}");
        }
    }
    
    private void CreateCourseButton(Transform parent, string courseName)
    {
        GameObject buttonGO = new GameObject($"Course Button - {courseName}");
        buttonGO.transform.SetParent(parent);
        
        Image buttonImage = buttonGO.AddComponent<Image>();
        buttonImage.color = new Color(0.3f, 0.3f, 0.4f, 1f);
        
        Button button = buttonGO.AddComponent<Button>();
        
        // Course image
        GameObject imageGO = new GameObject("Course Image");
        imageGO.transform.SetParent(buttonGO.transform);
        Image courseImage = imageGO.AddComponent<Image>();
        courseImage.color = Color.gray; // Placeholder
        RectTransform imageRect = imageGO.GetComponent<RectTransform>();
        imageRect.anchorMin = new Vector2(0.1f, 0.3f);
        imageRect.anchorMax = new Vector2(0.9f, 0.9f);
        imageRect.offsetMin = Vector2.zero;
        imageRect.offsetMax = Vector2.zero;
        
        // Course name
        GameObject textGO = new GameObject("Course Name");
        textGO.transform.SetParent(buttonGO.transform);
        TextMeshProUGUI courseText = textGO.AddComponent<TextMeshProUGUI>();
        courseText.text = courseName;
        courseText.fontSize = 14;
        courseText.alignment = TextAlignmentOptions.Center;
        courseText.color = Color.white;
        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0, 0);
        textRect.anchorMax = new Vector2(1, 0.3f);
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
    }
}

#endif
