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
    public float maxSpeed = 500f; // Much more powerful for racing bike
    public float acceleration = 50f; // Controls how quickly throttle ramps up

    /// <summary>
    /// Public property for test compatibility
    /// </summary>
    public float MaxSpeed => maxSpeed;
    public float Acceleration => acceleration;

    [Header("Bike Stability")]
    public float selfRightingStrength = 4f;
    public float bankingStrength = 8f;
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
    // --- Coyote time (grace period for ground detection) ---
    private float lastGroundedTime = 0f;
    private const float coyoteTime = 0.15f; // 150ms grace period

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
        // GameLogger.Info("PlayerController", $"Speed: {currentSpeed:F2} km/h, MotorInput: {motorInput:F2}");

        ApplyBankingAndStability();
    }

    // Helper to check if we should consider the bike grounded (with coyote time)
    private bool IsEffectivelyGrounded()
    {
        return (isGrounded || (Time.time - lastGroundedTime < coyoteTime));
    }

    
    void ApplyBankingAndStability()
    {
        if (IsEffectivelyGrounded())
        {
            // --- Strong upright force to keep bike from falling over ---
            Vector3 localUp = transform.up;
            Vector3 uprightTorque = Vector3.Cross(localUp, Vector3.up);
            rb.AddTorque(uprightTorque * selfRightingStrength * 10f, ForceMode.Acceleration); // much stronger

            // --- Clamp Z rotation (lean/bank) to [-45, 45] degrees ---
            Vector3 euler = transform.eulerAngles;
            float z = euler.z;
            if (z > 180f) z -= 360f;
            z = Mathf.Clamp(z, -45f, 45f);
            euler.z = z;
            transform.eulerAngles = euler;
        }
        // --- Gentle banking into corners ---
        float targetBank = -steerInput * Mathf.Clamp(currentSpeed / 60f, 0f, 1f) * 15f; // up to 15 degrees
        Quaternion targetRotation = Quaternion.Euler(0f, transform.eulerAngles.y, targetBank);
        Quaternion currentRotation = transform.rotation;
        Quaternion desiredRotation = Quaternion.Lerp(currentRotation, targetRotation, Time.deltaTime * bankingStrength);
        rb.MoveRotation(desiredRotation);
    }
    
    void FixedUpdate()
    {
        ApplyMotor();
        ApplySteering();
        ApplyBraking();
    }
    
    void HandleInput()
    {
        float targetMotorInput = 0f;
        steerInput = 0f;
        if (playerNumber == 1) {
            // Player 1 - WASD keys
            if (Keyboard.current.wKey.isPressed) targetMotorInput += 1f;
            if (Keyboard.current.sKey.isPressed) targetMotorInput -= 1f;
            if (Keyboard.current.aKey.isPressed) steerInput -= 1f;
            if (Keyboard.current.dKey.isPressed) steerInput += 1f;
            
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                Jump();
            }
        }
        else if (playerNumber == 2) {
            // Player 2 - IJKL keys
            if (Keyboard.current.iKey.isPressed) targetMotorInput += 1f;
            if (Keyboard.current.kKey.isPressed) targetMotorInput -= 1f;
            if (Keyboard.current.jKey.isPressed) steerInput -= 1f;
            if (Keyboard.current.lKey.isPressed) steerInput += 1f;
            
            if (Keyboard.current.rightShiftKey.wasPressedThisFrame)
            {
                Jump();
            }
        }
        
        // Mouse control (accessibility option)
        if (Mouse.current != null && Mouse.current.leftButton.isPressed) {
            Vector3 mousePos = Mouse.current.position.ReadValue();
            Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            
            Vector3 mouseOffset = mousePos - screenCenter;
            float normalizedX = mouseOffset.x / (Screen.width / 2);
            float normalizedY = mouseOffset.y / (Screen.height / 2);
            
            steerInput = Mathf.Clamp(normalizedX, -1f, 1f);
            targetMotorInput = Mathf.Clamp(normalizedY, -1f, 1f);
        }

        // Gradual acceleration: smooth motorInput towards targetMotorInput
        motorInput = Mathf.MoveTowards(motorInput, targetMotorInput, acceleration * Time.deltaTime);
    }
    
    void ApplyMotor() {
        float speed = rb.linearVelocity.magnitude;
        float motor = motorInput * maxSpeed;
        
        // --- Uphill/low-speed boost ---
        if (motorInput > 0f && speed < 5f)
        {
            // Add extra torque when nearly stopped
            motor += 200f * (1f - (speed / 5f)); // scales down as speed increases
        }
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
    
    void ApplySteering() {
        float speed = rb.linearVelocity.magnitude;
        // At low speed, increase steering influence (up to 3x at zero speed)
        float steerMultiplier = Mathf.Lerp(3f, 1f, Mathf.Clamp01(speed / 8f));
        float steering = steerInput * turnSpeed * steerMultiplier;
        
        foreach (WheelCollider wheel in wheelColliders)
        {
            if (wheel.name.Contains("Front"))
            {
                wheel.steerAngle = steering;
            }
        }
    }
    
    void ApplyBraking() {
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
    
    void UpdateWheelMeshes() {
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
    
    void CheckGrounded() {
        bool wasGrounded = isGrounded;
        isGrounded = false;
        foreach (WheelCollider wheel in wheelColliders)
        {
            if (wheel.isGrounded)
            {
                isGrounded = true;
                break;
            }
        }
        // If grounded, update last grounded time
        if (isGrounded)
        {
            lastGroundedTime = Time.time;
        }
    }
    
    void Jump() {
        if (IsEffectivelyGrounded()) {
            rb.AddForce(Vector3.up * jumpForce);
        }
    }
    
    TerrainType GetCurrentTerrain() {
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
    
    public void TakeDamage(float damage) {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        // Play crash sound
        if (gameManager) {
            gameManager.PlaySFX(gameManager.GetComponent<GameManager>().sfxSource.clip);
        }
        
        if (currentHealth <= 0) {
            // Player is out of health - respawn or end race
            ResetToStartPosition();
            currentHealth = maxHealth;
        }
    }
    
    public void ResetToStartPosition() {
        transform.position = startPosition;
        transform.rotation = startRotation;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
    
    public float GetHealthPercentage() {
        return currentHealth / maxHealth;
    }
    
    public float GetSpeed() {
        return currentSpeed;
    }
    
    public float GetMotorInput() {
        return motorInput;
    }
    
    void OnTriggerEnter(Collider other) {
        // Handle collisions with obstacles and NPCs
        if (other.CompareTag("Obstacle")) {
            TakeDamage(20f);
            // Slow down the bike
            rb.linearVelocity *= 0.5f;
        }
        else if (other.CompareTag("NPC")) {
            TakeDamage(15f);
            rb.linearVelocity *= 0.7f;
        }
        else if (other.CompareTag("Finish")) {
            if (gameManager) {
                gameManager.EndRace();
            }
        }
    }
}

public enum TerrainType {
    Bitumen,
    Gravel,
    Dirt,
    OffRoad
}