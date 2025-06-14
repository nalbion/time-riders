using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Sets up the initial scene, including GameManager, terrain, starting objects, and UI.
/// </summary>
public class SceneSetup : MonoBehaviour {
    [Header("Setup Options")]
    public bool autoSetupScene = true;
    
    /// <summary>
    /// Unity Start method. Automatically sets up the scene if enabled.
    /// </summary>
    void Start() {
        if (autoSetupScene) {
            SetupBasicScene();
        }
    }
    
    /// <summary>
    /// Sets up the core components of the scene in order.
    /// </summary>
    void SetupBasicScene() {
        // Create basic game objects if they don't exist
        CreateGameManager();
        CreateBasicTerrain();
        CreateStartingObjects();
        CreateUI();
    }
    
    /// <summary>
    /// Creates the GameManager and LeaderboardManager if not already present.
    /// </summary>
    void CreateGameManager() {
        if (FindFirstObjectByType<GameManager>() == null) {
            GameObject gameManagerObj = new GameObject("GameManager");
            gameManagerObj.AddComponent<GameManager>();
            gameManagerObj.AddComponent<LeaderboardManager>();
        }
    }
    
    /// <summary>
    /// Creates a procedural terrain with a central road and grass.
    /// </summary>
    void CreateBasicTerrain() {
        // Create TerrainData
        int size = 256;
        int height = 40;
        TerrainData terrainData = new TerrainData();
        terrainData.heightmapResolution = size + 1;
        terrainData.size = new Vector3(200, height, 200);

        // Generate hills with Perlin noise
        float[,] heights = new float[size, size];
        for (int x = 0; x < size; x++) {
            for (int y = 0; y < size; y++) {
                float nx = (float)x / size - 0.5f;
                float ny = (float)y / size - 0.5f;
                float hill = Mathf.PerlinNoise(x * 0.07f, y * 0.07f) * 0.15f;
                heights[x, y] = hill;
            }
        }
        terrainData.SetHeights(0, 0, heights);

        // Create the Terrain object
        GameObject terrainObj = Terrain.CreateTerrainGameObject(terrainData);
        terrainObj.name = "ProceduralTerrain";
        Terrain terrain = terrainObj.GetComponent<Terrain>();
        terrainObj.AddComponent<TerrainChecker>().terrainType = TerrainType.Bitumen;

        // Setup Terrain Layers (road and grass)
        TerrainLayer roadLayer = new TerrainLayer();
        TerrainLayer grassLayer = new TerrainLayer();
        // Road: use gray or white
        roadLayer.diffuseTexture = Texture2D.grayTexture;
        roadLayer.tileSize = new Vector2(10, 10);
        // Grass: create a solid green texture
        grassLayer.diffuseTexture = CreateSolidColorTexture(Color.green);
        grassLayer.tileSize = new Vector2(20, 20);
        terrainData.terrainLayers = new TerrainLayer[] { grassLayer, roadLayer };

        // Helper to create a solid color texture
        Texture2D CreateSolidColorTexture(Color color) {
            Texture2D tex = new Texture2D(2, 2);
            Color[] pixels = new Color[4] { color, color, color, color };
            tex.SetPixels(pixels);
            tex.Apply();
            return tex;
        }

        // Paint a road down the center
        float[,,] alphas = new float[size, size, 2];
        for (int x = 0; x < size; x++) {
            for (int y = 0; y < size; y++) {
                float roadCenter = size / 2;
                float roadWidth = size / 8;
                float dist = Mathf.Abs(y - roadCenter);
                float t = Mathf.Clamp01(1f - (dist / roadWidth));
                alphas[x, y, 0] = 1f - t; // Grass
                alphas[x, y, 1] = t;      // Road
            }
        }
        terrainData.SetAlphamaps(0, 0, alphas);
    }
    
    /// <summary>
    /// Creates the start/finish line object.
    /// </summary>
    void CreateStartingObjects() {
        // Create start/finish line
        GameObject finishLine = new GameObject("FinishLine");
        finishLine.AddComponent<BoxCollider>().isTrigger = true;
        finishLine.AddComponent<FinishLine>();
        finishLine.transform.position = new Vector3(0, 1, 0);
    }
    
    /// <summary>
    /// Creates a Canvas for UI if one does not already exist.
    /// </summary>
    void CreateUI() {
        // Create Canvas if it doesn't exist
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null) {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<GraphicRaycaster>();
        }
    }
}
