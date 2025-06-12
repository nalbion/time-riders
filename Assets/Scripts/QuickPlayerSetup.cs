using UnityEngine;

public class QuickPlayerSetup : MonoBehaviour
{
    [Header("Quick Setup")]
    public bool createPlayerOnStart = true;
    
    void Start()
    {
        if (createPlayerOnStart)
        {
            CreateBasicPlayer();
            StartCoroutine(AlignPlayerToTerrainNextFrame());
        }
    }

    System.Collections.IEnumerator AlignPlayerToTerrainNextFrame()
    {
        yield return null; // Wait for one frame so terrain is created
        GameObject player = GameObject.Find("Player1");
        if (Terrain.activeTerrain != null)
        {
            Bounds bounds = Terrain.activeTerrain.terrainData.bounds;
            Vector3 tpos = Terrain.activeTerrain.GetPosition();
            Debug.Log($"Terrain origin: {tpos}, bounds size: {bounds.size}, world max: {tpos + bounds.size}");
        }
        if (player != null)
        {
            Debug.Log($"Initial player position: {player.transform.position}");
        }
        if (player != null && Terrain.activeTerrain != null)
        {
            Vector3 tOrigin = Terrain.activeTerrain.GetPosition();
            Vector3 tSize = Terrain.activeTerrain.terrainData.size;
            float x = tOrigin.x + tSize.x / 2f;
            float z = tOrigin.z + tSize.z / 2f;
            float y = Terrain.activeTerrain.SampleHeight(new Vector3(x, 0, z)) + 2f;
            player.transform.position = new Vector3(x, y, z);
            Debug.Log($"[Coroutine] Player repositioned to terrain center: ({x}, {y}, {z})");
        }
    }
    
    void CreateBasicPlayer()
    {
        // Create basic motorbike (using capsule for now)
        GameObject player = new GameObject("Player1");
        // Spawn at terrain center (on the road)
        float x = 0f, z = 0f, y = 2f;
        if (Terrain.activeTerrain != null)
        {
            Vector3 tOrigin = Terrain.activeTerrain.GetPosition();
            Vector3 tSize = Terrain.activeTerrain.terrainData.size;
            x = tOrigin.x + tSize.x / 2f;
            z = tOrigin.z + tSize.z / 2f;
            y = Terrain.activeTerrain.SampleHeight(new Vector3(x, 0, z)) + 2f;
            Debug.Log($"Spawning player at terrain center: ({x}, {y}, {z})");
        }
        player.transform.position = new Vector3(x, y, z);
        
        // Add basic mesh
        GameObject body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        body.transform.SetParent(player.transform);
        body.transform.localPosition = Vector3.zero;
        body.transform.localScale = new Vector3(1, 0.5f, 2);
        
        // Add wheels (spheres for now)
        CreateWheel(player.transform, "FrontWheel", new Vector3(0, -0.5f, 1));
        CreateWheel(player.transform, "RearWheel", new Vector3(0, -0.5f, -1));
        
        // Add physics
        Rigidbody rb = player.AddComponent<Rigidbody>();
        rb.mass = 100f;
        rb.centerOfMass = new Vector3(0, -0.5f, 0);
        
        // Add player controller
        PlayerController controller = player.AddComponent<PlayerController>();
        controller.playerNumber = 1;
        
        // Add wheel colliders (basic setup)
        AddWheelColliders(controller, player);
        
        // Setup camera to follow player
        SetupCamera(player.transform);
    }
    
    void CreateWheel(Transform parent, string name, Vector3 position)
    {
        GameObject wheel = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        wheel.name = name;
        wheel.transform.SetParent(parent);
        wheel.transform.localPosition = position;
        wheel.transform.localScale = Vector3.one * 0.6f;
        
        // Remove collider from visual wheel
        Destroy(wheel.GetComponent<Collider>());
    }
    
    void AddWheelColliders(PlayerController controller, GameObject player)
    {
        // Create wheel colliders
        GameObject frontWheelCollider = new GameObject("FrontWheelCollider");
        frontWheelCollider.transform.SetParent(player.transform);
        frontWheelCollider.transform.localPosition = new Vector3(0, -0.5f, 1);
        WheelCollider frontWheel = frontWheelCollider.AddComponent<WheelCollider>();
        
        GameObject rearWheelCollider = new GameObject("RearWheelCollider");
        rearWheelCollider.transform.SetParent(player.transform);
        rearWheelCollider.transform.localPosition = new Vector3(0, -0.5f, -1);
        WheelCollider rearWheel = rearWheelCollider.AddComponent<WheelCollider>();
        
        // Configure wheel colliders
        ConfigureWheelCollider(frontWheel);
        ConfigureWheelCollider(rearWheel);
        
        // Assign to player controller
        controller.wheelColliders = new WheelCollider[] { frontWheel, rearWheel };
        controller.wheelMeshes = new Transform[] { 
            player.transform.Find("FrontWheel"), 
            player.transform.Find("RearWheel") 
        };
    }
    
    void ConfigureWheelCollider(WheelCollider wheel)
    {
        JointSpring spring = wheel.suspensionSpring;
        spring.spring = 35000f;
        spring.damper = 4500f;
        wheel.suspensionSpring = spring;
        
        wheel.suspensionDistance = 0.3f;
        wheel.forceAppPointDistance = 0f;
        wheel.mass = 20f;
        
        WheelFrictionCurve sideways = wheel.sidewaysFriction;
        sideways.extremumSlip = 0.4f;
        sideways.extremumValue = 1f;
        sideways.asymptoteSlip = 0.8f;
        sideways.asymptoteValue = 0.5f;
        sideways.stiffness = 1f;
        wheel.sidewaysFriction = sideways;
        
        WheelFrictionCurve forward = wheel.forwardFriction;
        forward.extremumSlip = 0.4f;
        forward.extremumValue = 1f;
        forward.asymptoteSlip = 0.8f;
        forward.asymptoteValue = 0.5f;
        forward.stiffness = 1f;
        wheel.forwardFriction = forward;
    }
    
    void SetupCamera(Transform target)
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            GameObject cameraObj = new GameObject("Main Camera");
            mainCamera = cameraObj.AddComponent<Camera>();
            cameraObj.tag = "MainCamera";
        }
        
        CameraController camController = mainCamera.GetComponent<CameraController>();
        if (camController == null)
        {
            camController = mainCamera.gameObject.AddComponent<CameraController>();
        }
        
        camController.SetTarget(target);
    }
}