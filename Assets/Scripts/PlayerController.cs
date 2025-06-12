using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    public int playerNumber = 1;
    public CharacterData characterData;
    
    [Header("Movement Settings")]
    public float maxSpeed = 100f;
    public float acceleration = 10f;
    public float braking = 15f;
    public float turnSpeed = 180f;
    public float jumpForce = 500f;
    
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;
    
    [Header("Physics")]
    public WheelCollider[] wheelColliders;
    public Transform[] wheelMeshes;
    public Transform centerOfMass;
    
    private Rigidbody rb;
    private float motorInput;
    private float steerInput;
    private bool isGrounded;
    private float currentSpeed;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private GameManager gameManager;
    
    // Terrain speed modifiers
    private Dictionary<TerrainType, float> terrainSpeedModifiers = new Dictionary<TerrainType, float>
    {
        { TerrainType.Bitumen, 1.0f },
        { TerrainType.Gravel, 0.8f },
        { TerrainType.Dirt, 0.6f },
        { TerrainType.OffRoad, 0.4f }
    };
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        gameManager = FindFirstObjectByType<GameManager>();
        currentHealth = maxHealth;
        
        // Store start position
        startPosition = transform.position;
        startRotation = transform.rotation;
        
        // Set center of mass for better physics
        if (centerOfMass)
        {
            rb.centerOfMass = centerOfMass.localPosition;
        }
    }
    
    void Update()
    {
        HandleInput();
        UpdateWheelMeshes();
        CheckGrounded();
        currentSpeed = rb.linearVelocity.magnitude * 3.6f; // Convert to km/h
        Debug.Log($"[PlayerController] Speed: {currentSpeed:F2} km/h, MotorInput: {motorInput:F2}");
    }
    
    void FixedUpdate()
    {
        ApplyMotor();
        ApplySteering();
        ApplyBraking();
    }
    
    void HandleInput()
    {
        if (playerNumber == 1)
        {
            // Player 1 - WASD keys
            motorInput = 0f;
            steerInput = 0f;
            // WASD logging
            if (Keyboard.current.wKey.wasPressedThisFrame)
                Debug.Log("W pressed");
            if (Keyboard.current.wKey.wasReleasedThisFrame)
                Debug.Log("W released");
            if (Keyboard.current.sKey.wasPressedThisFrame)
                Debug.Log("S pressed");
            if (Keyboard.current.sKey.wasReleasedThisFrame)
                Debug.Log("S released");
            if (Keyboard.current.aKey.wasPressedThisFrame)
                Debug.Log("A pressed");
            if (Keyboard.current.aKey.wasReleasedThisFrame)
                Debug.Log("A released");
            if (Keyboard.current.dKey.wasPressedThisFrame)
                Debug.Log("D pressed");
            if (Keyboard.current.dKey.wasReleasedThisFrame)
                Debug.Log("D released");

            if (Keyboard.current.wKey.isPressed) motorInput += 1f;
            if (Keyboard.current.sKey.isPressed) motorInput -= 1f;
            if (Keyboard.current.aKey.isPressed) steerInput -= 1f;
            if (Keyboard.current.dKey.isPressed) steerInput += 1f;
            
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                Jump();
            }
        }
        else if (playerNumber == 2)
        {
            // Player 2 - IJKL keys
            motorInput = 0f;
            steerInput = 0f;
            if (Keyboard.current.iKey.isPressed) motorInput += 1f;
            if (Keyboard.current.kKey.isPressed) motorInput -= 1f;
            if (Keyboard.current.jKey.isPressed) steerInput -= 1f;
            if (Keyboard.current.lKey.isPressed) steerInput += 1f;
            
            if (Keyboard.current.rightShiftKey.wasPressedThisFrame)
            {
                Jump();
            }
        }
        
        // Mouse control (accessibility option)
        if (Mouse.current != null && Mouse.current.leftButton.isPressed)
        {
            Vector3 mousePos = Mouse.current.position.ReadValue();
            Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            
            Vector3 mouseOffset = mousePos - screenCenter;
            float normalizedX = mouseOffset.x / (Screen.width / 2);
            float normalizedY = mouseOffset.y / (Screen.height / 2);
            
            steerInput = Mathf.Clamp(normalizedX, -1f, 1f);
            motorInput = Mathf.Clamp(normalizedY, -1f, 1f);
        }
    }
    
    void ApplyMotor()
    {
        float motor = motorInput * maxSpeed;
        
        // Apply terrain speed modifier
        TerrainType currentTerrain = GetCurrentTerrain();
        motor *= terrainSpeedModifiers[currentTerrain];
        
        foreach (WheelCollider wheel in wheelColliders)
        {
            if (wheel.name.Contains("Rear"))
            {
                wheel.motorTorque = motor;
            }
        }
    }
    
    void ApplySteering()
    {
        float steering = steerInput * turnSpeed;
        
        foreach (WheelCollider wheel in wheelColliders)
        {
            if (wheel.name.Contains("Front"))
            {
                wheel.steerAngle = steering;
            }
        }
    }
    
    void ApplyBraking()
    {
        float brake = 0f;
        
        if (motorInput == 0)
        {
            brake = braking;
        }
        
        foreach (WheelCollider wheel in wheelColliders)
        {
            wheel.brakeTorque = brake;
        }
    }
    
    void UpdateWheelMeshes()
    {
        for (int i = 0; i < wheelColliders.Length; i++)
        {
            if (i < wheelMeshes.Length && wheelMeshes[i])
            {
                Vector3 pos;
                Quaternion rot;
                wheelColliders[i].GetWorldPose(out pos, out rot);
                
                wheelMeshes[i].position = pos;
                wheelMeshes[i].rotation = rot;
            }
        }
    }
    
    void CheckGrounded()
    {
        isGrounded = false;
        foreach (WheelCollider wheel in wheelColliders)
        {
            if (wheel.isGrounded)
            {
                isGrounded = true;
                break;
            }
        }
    }
    
    void Jump()
    {
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce);
        }
    }
    
    TerrainType GetCurrentTerrain()
    {
        // Raycast down to determine terrain type and road
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 5f))
        {
            Terrain terrain = hit.collider.GetComponent<Terrain>();
            if (terrain != null)
            {
                Vector3 terrainPos = hit.point - terrain.transform.position;
                TerrainData td = terrain.terrainData;
                int mapX = Mathf.RoundToInt((terrainPos.x / td.size.x) * td.alphamapWidth);
                int mapZ = Mathf.RoundToInt((terrainPos.z / td.size.z) * td.alphamapHeight);
                float[,,] splat = td.GetAlphamaps(mapX, mapZ, 1, 1);
                float roadWeight = splat[0, 0, 1]; // 1 = road layer
                if (roadWeight > 0.5f)
                    return TerrainType.Bitumen; // On road
                else
                    return TerrainType.Dirt; // Off road
            }
            TerrainChecker checker = hit.collider.GetComponent<TerrainChecker>();
            if (checker)
            {
                return checker.terrainType;
            }
        }
        return TerrainType.OffRoad; // Default to off-road
    }
    
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        // Play crash sound
        if (gameManager)
        {
            gameManager.PlaySFX(gameManager.GetComponent<GameManager>().sfxSource.clip);
        }
        
        if (currentHealth <= 0)
        {
            // Player is out of health - respawn or end race
            ResetToStartPosition();
            currentHealth = maxHealth;
        }
    }
    
    public void ResetToStartPosition()
    {
        transform.position = startPosition;
        transform.rotation = startRotation;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
    
    public float GetHealthPercentage()
    {
        return currentHealth / maxHealth;
    }
    
    public float GetSpeed()
    {
        return currentSpeed;
    }
    
    public float GetMotorInput()
    {
        return motorInput;
    }
    
    void OnTriggerEnter(Collider other)
    {
        // Handle collisions with obstacles and NPCs
        if (other.CompareTag("Obstacle"))
        {
            TakeDamage(20f);
            // Slow down the bike
            rb.linearVelocity *= 0.5f;
        }
        else if (other.CompareTag("NPC"))
        {
            TakeDamage(15f);
            rb.linearVelocity *= 0.7f;
        }
        else if (other.CompareTag("Finish"))
        {
            if (gameManager)
            {
                gameManager.EndRace();
            }
        }
    }
}

public enum TerrainType
{
    Bitumen,
    Gravel,
    Dirt,
    OffRoad
}