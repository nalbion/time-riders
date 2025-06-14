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
    public void CreateBasicTerrain() {
        // --- Motocross Terrain Generation (1 meter per unit) ---
        int size = 513; // 513x513 heightmap, 1m per sample
        float terrainWidth = 512f; // meters
        float terrainLength = 512f; // meters
        float terrainHeight = 30f; // meters (vertical scale)
        TerrainData terrainData = new TerrainData();
        terrainData.heightmapResolution = size;
        terrainData.size = new Vector3(terrainWidth, terrainHeight, terrainLength);

        // Generate smooth base terrain (gentle undulations)
        float[,] heights = new float[size, size];
        for (int x = 0; x < size; x++) {
            for (int y = 0; y < size; y++) {
                float nx = (float)x / (size - 1);
                float ny = (float)y / (size - 1);
                // Low-frequency Perlin for gentle hills
                float baseHill = Mathf.PerlinNoise(nx * 2f, ny * 2f) * 0.08f;
                heights[x, y] = 0.15f + baseHill;
            }
        }

        // --- Procedural Track Path (simple loop for demo) ---
        // Parameters for track shape
        int numPoints = 18;
        float trackRadius = 170f; // meters (from center)
        float trackWidth = 12f; // meters
        float bermRadius = 16f; // meters (for banked turns)
        float jumpHeight = 0.08f; // normalized height (relative to terrainHeight)
        float jumpLength = 24f; // meters
        Vector2 center = new Vector2(size / 2, size / 2);
        Vector2[] trackPoints = new Vector2[numPoints];
        float[] trackElev = new float[numPoints];
        for (int i = 0; i < numPoints; i++) {
            float angle = (2 * Mathf.PI * i) / numPoints;
            float straight = (i % 3 == 0) ? 1.2f : 1f;
            float r = trackRadius * straight;
            float x = center.x + Mathf.Cos(angle) * r;
            float y = center.y + Mathf.Sin(angle) * r;
            trackPoints[i] = new Vector2(x, y);
            // Add jumps every 4th point
            trackElev[i] = (i % 4 == 2) ? jumpHeight : 0f;
        }

        // Paint track onto heightmap
        for (int i = 0; i < numPoints; i++) {
            Vector2 p0 = trackPoints[i];
            Vector2 p1 = trackPoints[(i + 1) % numPoints];
            float elev0 = trackElev[i];
            float elev1 = trackElev[(i + 1) % numPoints];
            int steps = (int)(Vector2.Distance(p0, p1));
            for (int s = 0; s <= steps; s++) {
                float t = (float)s / steps;
                Vector2 pos = Vector2.Lerp(p0, p1, t);
                float elev = Mathf.Lerp(elev0, elev1, t);
                // Track centerline
                int cx = Mathf.RoundToInt(pos.x);
                int cy = Mathf.RoundToInt(pos.y);
                for (int dx = -Mathf.CeilToInt(trackWidth / 2); dx <= Mathf.CeilToInt(trackWidth / 2); dx++) {
                    for (int dy = -Mathf.CeilToInt(trackWidth / 2); dy <= Mathf.CeilToInt(trackWidth / 2); dy++) {
                        int tx = cx + dx;
                        int ty = cy + dy;
                        if (tx < 0 || tx >= size || ty < 0 || ty >= size) continue;
                        float dist = Mathf.Sqrt(dx * dx + dy * dy);
                        float norm = Mathf.Clamp01(1f - (dist / (trackWidth / 2f)));
                        // Carve track into terrain (flatten, add jump if present)
                        float baseHeight = 0.17f + elev * Mathf.Exp(-Mathf.Pow(dist / (jumpLength / 2f), 2));
                        heights[tx, ty] = Mathf.Lerp(heights[tx, ty], baseHeight, norm * 0.85f);
                        // Add berm on sharp bends
                        if (dist > (trackWidth / 2f) * 0.7f && dist < bermRadius) {
                            float berm = 0.06f * Mathf.Pow(norm, 1.5f) * Mathf.Abs(Mathf.Sin((angleBetween(trackPoints, i) - Mathf.PI) / 2));
                            heights[tx, ty] += berm;
                        }
                    }
                }
            }
        }
        terrainData.SetHeights(0, 0, heights);

        // --- Create Terrain Object ---
        GameObject terrainObj = Terrain.CreateTerrainGameObject(terrainData);
        terrainObj.name = "MotocrossTerrain";
        Terrain terrain = terrainObj.GetComponent<Terrain>();
        terrainObj.AddComponent<TerrainChecker>().terrainType = TerrainType.Bitumen;

        // --- Ensure TerrainCollider is present and assigned ---
        TerrainCollider collider = terrainObj.GetComponent<TerrainCollider>();
        if (!collider) collider = terrainObj.AddComponent<TerrainCollider>();
        collider.terrainData = terrainData;

        // --- Assign a WebGL-compatible material (Nature/Terrain/Standard) ---
        // Ensure you have a Resources/StandardTerrainMaterial.mat using Nature/Terrain/Standard shader
        Material terrainMat = Resources.Load<Material>("StandardTerrainMaterial");
        if (terrainMat != null) {
            terrain.materialTemplate = terrainMat;
        }

        // --- Setup Terrain Layers (track and grass) ---
        TerrainLayer trackLayer = new TerrainLayer();
        TerrainLayer grassLayer = new TerrainLayer();
        trackLayer.diffuseTexture = Texture2D.grayTexture;
        trackLayer.tileSize = new Vector2(10, 10);
        grassLayer.diffuseTexture = CreateSolidColorTexture(Color.green);
        grassLayer.tileSize = new Vector2(20, 20);
        terrainData.terrainLayers = new TerrainLayer[] { grassLayer, trackLayer };

        // Helper to create a solid color texture
        Texture2D CreateSolidColorTexture(Color color) {
            Texture2D tex = new Texture2D(2, 2);
            Color[] pixels = new Color[4] { color, color, color, color };
            tex.SetPixels(pixels);
            tex.Apply();
            return tex;
        }

        // --- Paint track texture ---
        int alphaRes = 512;
        terrainData.alphamapResolution = alphaRes;
        float[,,] alphas = new float[alphaRes, alphaRes, 2];
        for (int x = 0; x < alphaRes; x++) {
            for (int y = 0; y < alphaRes; y++) {
                // Map alphamap coords to heightmap/track space
                float normX = (float)x / (alphaRes - 1);
                float normY = (float)y / (alphaRes - 1);
                int hmX = Mathf.RoundToInt(normX * (size - 1));
                int hmY = Mathf.RoundToInt(normY * (size - 1));
                float minDist = 9999f;
                for (int i = 0; i < numPoints; i++) {
                    float d = Vector2.Distance(new Vector2(hmX, hmY), trackPoints[i]);
                    if (d < minDist) minDist = d;
                }
                float t = Mathf.Clamp01(1f - (minDist / (trackWidth / 1.6f)));
                alphas[x, y, 0] = 1f - t; // Grass
                alphas[x, y, 1] = t;      // Track
            }
        }
        terrainData.SetAlphamaps(0, 0, alphas);

        // --- TODO: Implement option to save the generated track (heightmap/path) to file after a drive ---
    }

    // Helper to estimate angle between track segments (for berms)
    private float angleBetween(Vector2[] pts, int i) {
        int prev = (i - 1 + pts.Length) % pts.Length;
        int next = (i + 1) % pts.Length;
        Vector2 a = (pts[i] - pts[prev]).normalized;
        Vector2 b = (pts[next] - pts[i]).normalized;
        return Mathf.Acos(Mathf.Clamp(Vector2.Dot(a, b), -1f, 1f));
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
