#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Editor utility to set up background images for UI scenes
/// </summary>
public class BackgroundSetupHelper : EditorWindow
{
    [MenuItem("TimeRiders/Setup UI/Set MainMenu Background")]
    public static void SetMainMenuBackground()
    {
        // Load the background image
        string imagePath = "Assets/Images/884-nose-dive.png";
        Texture2D backgroundTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(imagePath);
        
        if (backgroundTexture == null)
        {
            EditorUtility.DisplayDialog("Error", $"Could not find background image at {imagePath}", "OK");
            return;
        }
        
        // Create sprite from texture
        Sprite backgroundSprite = AssetDatabase.LoadAssetAtPath<Sprite>(imagePath);
        if (backgroundSprite == null)
        {
            // Configure texture import settings to create sprite
            TextureImporter importer = AssetImporter.GetAtPath(imagePath) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.maxTextureSize = 2048; // Adjust based on your needs
                importer.textureCompression = TextureImporterCompression.Compressed;
                AssetDatabase.ImportAsset(imagePath);
                backgroundSprite = AssetDatabase.LoadAssetAtPath<Sprite>(imagePath);
            }
        }
        
        if (backgroundSprite == null)
        {
            EditorUtility.DisplayDialog("Error", "Failed to create sprite from background image", "OK");
            return;
        }
        
        // Find or create Canvas in current scene
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            EditorUtility.DisplayDialog("Error", "No Canvas found in current scene. Please open the MainMenu scene first.", "OK");
            return;
        }
        
        // Check if background already exists
        Transform existingBackground = canvas.transform.Find("Background");
        if (existingBackground != null)
        {
            bool replace = EditorUtility.DisplayDialog("Background Exists", 
                "A background already exists. Do you want to replace it?", 
                "Replace", "Cancel");
            
            if (!replace) return;
            
            DestroyImmediate(existingBackground.gameObject);
        }
        
        // Create background GameObject
        GameObject backgroundObj = new GameObject("Background");
        backgroundObj.transform.SetParent(canvas.transform, false);
        backgroundObj.transform.SetAsFirstSibling(); // Put it behind other UI elements
        
        // Add Image component
        Image backgroundImage = backgroundObj.AddComponent<Image>();
        backgroundImage.sprite = backgroundSprite;
        backgroundImage.type = Image.Type.Simple;
        
        // Set up RectTransform to fill the screen
        RectTransform rectTransform = backgroundObj.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        
        // Set appropriate scaling mode
        backgroundImage.preserveAspect = false; // Stretch to fill
        
        // Mark scene as dirty
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        
        Debug.Log($"Successfully set MainMenu background to: {backgroundSprite.name}");
        EditorUtility.DisplayDialog("Background Set", 
            $"Background image '{backgroundSprite.name}' has been applied to the MainMenu scene.\n\n" +
            "The background is set to stretch and fill the entire screen.", 
            "OK");
    }
    
    [MenuItem("TimeRiders/Setup UI/Configure Background Image Settings")]
    public static void ConfigureBackgroundImageSettings()
    {
        string imagePath = "Assets/Images/884-nose-dive.png";
        
        // Configure texture import settings for optimal UI usage
        TextureImporter importer = AssetImporter.GetAtPath(imagePath) as TextureImporter;
        if (importer == null)
        {
            EditorUtility.DisplayDialog("Error", $"Could not find image at {imagePath}", "OK");
            return;
        }
        
        // Set optimal settings for UI background
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.maxTextureSize = 2048;
        importer.textureCompression = TextureImporterCompression.Compressed;
        importer.compressionQuality = 80; // Good balance of quality vs size
        importer.crunchedCompression = true;
        importer.streamingMipmaps = false;
        importer.generateCubemap = TextureImporterGenerateCubemap.None;
        
        // Apply settings
        AssetDatabase.ImportAsset(imagePath);
        AssetDatabase.Refresh();
        
        Debug.Log("Background image import settings configured for optimal UI usage");
        EditorUtility.DisplayDialog("Settings Applied", 
            "Background image has been configured with optimal settings for UI usage:\n\n" +
            "• Texture Type: Sprite\n" +
            "• Max Size: 2048\n" +
            "• Compression: Compressed with Crunch\n" +
            "• Quality: 80%", 
            "OK");
    }
    
    [MenuItem("TimeRiders/Setup UI/Create Responsive Background")]
    public static void CreateResponsiveBackground()
    {
        // This creates a more sophisticated background setup that adapts to different screen ratios
        string imagePath = "Assets/Images/884-nose-dive.png";
        Sprite backgroundSprite = AssetDatabase.LoadAssetAtPath<Sprite>(imagePath);
        
        if (backgroundSprite == null)
        {
            EditorUtility.DisplayDialog("Error", 
                "Background sprite not found. Please run 'Configure Background Image Settings' first.", 
                "OK");
            return;
        }
        
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            EditorUtility.DisplayDialog("Error", "No Canvas found. Please open the MainMenu scene first.", "OK");
            return;
        }
        
        // Remove existing background
        Transform existingBackground = canvas.transform.Find("ResponsiveBackground");
        if (existingBackground != null)
        {
            DestroyImmediate(existingBackground.gameObject);
        }
        
        // Create responsive background container
        GameObject backgroundContainer = new GameObject("ResponsiveBackground");
        backgroundContainer.transform.SetParent(canvas.transform, false);
        backgroundContainer.transform.SetAsFirstSibling();
        
        // Set up container RectTransform
        RectTransform containerRect = backgroundContainer.AddComponent<RectTransform>();
        containerRect.anchorMin = Vector2.zero;
        containerRect.anchorMax = Vector2.one;
        containerRect.offsetMin = Vector2.zero;
        containerRect.offsetMax = Vector2.zero;
        
        // Create the actual background image
        GameObject backgroundObj = new GameObject("BackgroundImage");
        backgroundObj.transform.SetParent(backgroundContainer.transform, false);
        
        Image backgroundImage = backgroundObj.AddComponent<Image>();
        backgroundImage.sprite = backgroundSprite;
        backgroundImage.type = Image.Type.Simple;
        backgroundImage.preserveAspect = true; // Maintain aspect ratio
        
        // Set up RectTransform for aspect ratio preservation
        RectTransform imageRect = backgroundObj.GetComponent<RectTransform>();
        imageRect.anchorMin = Vector2.zero;
        imageRect.anchorMax = Vector2.one;
        imageRect.offsetMin = Vector2.zero;
        imageRect.offsetMax = Vector2.zero;
        
        // Add AspectRatioFitter to maintain proper scaling
        AspectRatioFitter aspectFitter = backgroundObj.AddComponent<AspectRatioFitter>();
        aspectFitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
        aspectFitter.aspectRatio = (float)backgroundSprite.texture.width / backgroundSprite.texture.height;
        
        // Mark scene as dirty
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        
        Debug.Log("Created responsive background that maintains aspect ratio");
        EditorUtility.DisplayDialog("Responsive Background Created", 
            "A responsive background has been created that:\n\n" +
            "• Maintains the original image aspect ratio\n" +
            "• Scales to cover the entire screen\n" +
            "• Adapts to different screen resolutions\n" +
            "• Prevents image distortion", 
            "OK");
    }
}
#endif
