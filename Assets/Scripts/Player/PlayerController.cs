using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
// using SimpleMotorcyclePhysics;

public class PlayerController : MonoBehaviour {
    // [Header("Player Settings")]
    // public int playerNumber = 1;
    // public CharacterData characterData;
    

    // [Header("Health Settings")]
    // public float maxHealth = 100f;
    // public float currentHealth;
    
    // private Vector3 startPosition;
    // private Quaternion startRotation;
    // private GameManager gameManager;
    
    // Reference to SMP MotorbikeController
    public MotorbikeController motorbikeController;

    
    // void Start() {
    //     gameManager = FindFirstObjectByType<GameManager>();
    //     currentHealth = maxHealth;
    //     // Store start position
    //     startPosition = transform.position;
    //     startRotation = transform.rotation;
    // }
    
    // void Update() {
    //     // Example: Get speed from SMP
    //     // float speed = motorbikeController.CurrentSpeed;
    //     // You can add health checks or race logic here if needed
    // }

    // // Helper to check if we should consider the bike grounded (with coyote time)
    // private bool IsEffectivelyGrounded() {
    //     return (isGrounded || (Time.time - lastGroundedTime < coyoteTime));
    // }

    
    // void ApplyBankingAndStability() {
    //     if (IsEffectivelyGrounded()) {
    //         // --- Strong upright force to keep bike from falling over ---
    //         Vector3 localUp = transform.up;
    //         Vector3 uprightTorque = Vector3.Cross(localUp, Vector3.up);
    //         rb.AddTorque(uprightTorque * selfRightingStrength * 10f, ForceMode.Acceleration); // much stronger

    //         // --- Clamp Z rotation (lean/bank) to [-45, 45] degrees ---
    //         Vector3 euler = transform.eulerAngles;
    //         float z = euler.z;
    //         if (z > 180f) z -= 360f;
    //         z = Mathf.Clamp(z, -45f, 45f);
    //         euler.z = z;
    //         transform.eulerAngles = euler;
    //     }
    //     // --- Gentle banking into corners ---
    //     float targetBank = -steerInput * Mathf.Clamp(currentSpeed / 60f, 0f, 1f) * 15f; // up to 15 degrees
    //     Quaternion targetRotation = Quaternion.Euler(0f, transform.eulerAngles.y, targetBank);
    //     Quaternion currentRotation = transform.rotation;
    //     Quaternion desiredRotation = Quaternion.Lerp(currentRotation, targetRotation, Time.deltaTime * bankingStrength);
    //     rb.MoveRotation(desiredRotation);
    // }
    
    // void FixedUpdate() {
    //     ApplyMotor();
    //     ApplySteering();
    //     ApplyBraking();
    // }
    
    // void HandleInput() {
    //     float targetMotorInput = 0f;
    //     steerInput = 0f;
    //     if (playerNumber == 1) {
    //         // Player 1 - WASD keys
    //         if (Keyboard.current.wKey.isPressed) targetMotorInput += 1f;
    //         if (Keyboard.current.sKey.isPressed) targetMotorInput -= 1f;
    //         if (Keyboard.current.aKey.isPressed) steerInput -= 1f;
    //         if (Keyboard.current.dKey.isPressed) steerInput += 1f;
            
    //         if (Keyboard.current.spaceKey.wasPressedThisFrame) {
    //             Jump();
    //         }
    //     }
    //     else if (playerNumber == 2) {
    //         // Player 2 - IJKL keys
    //         if (Keyboard.current.iKey.isPressed) targetMotorInput += 1f;
    //         if (Keyboard.current.kKey.isPressed) targetMotorInput -= 1f;
    //         if (Keyboard.current.jKey.isPressed) steerInput -= 1f;
    //         if (Keyboard.current.lKey.isPressed) steerInput += 1f;
            
    //         if (Keyboard.current.rightShiftKey.wasPressedThisFrame) {
    //             Jump();
    //         }
    //     }
        
    //     // Mouse control (accessibility option)
    //     if (Mouse.current != null && Mouse.current.leftButton.isPressed) {
    //         Vector3 mousePos = Mouse.current.position.ReadValue();
    //         Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            
    //         Vector3 mouseOffset = mousePos - screenCenter;
    //         float normalizedX = mouseOffset.x / (Screen.width / 2);
    //         float normalizedY = mouseOffset.y / (Screen.height / 2);   
    //     }
    // }
    
    public void TakeDamage(float damage) {
    //     currentHealth -= damage;
    //     currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
    //     // Play crash sound
    //     if (gameManager) {
    //         gameManager.PlaySFX(gameManager.GetComponent<GameManager>().sfxSource.clip);
    //     }
        
    //     if (currentHealth <= 0) {
    //         // Player is out of health - respawn or end race
    //         ResetToStartPosition();
    //         currentHealth = maxHealth;
    //     }
    }
    
    // public void ResetToStartPosition() {
    //     transform.position = startPosition;
    //     transform.rotation = startRotation;
    //     rb.linearVelocity = Vector3.zero;
    //     rb.angularVelocity = Vector3.zero;
    // }
    
    // public float GetHealthPercentage() {
    //     return currentHealth / maxHealth;
    // }
    // // If you need speed, get it from motorbikeController
    // public float GetSpeed() {
    //     return motorbikeController != null ? motorbikeController.CurrentSpeed : 0f;
    // }
    
    // void OnTriggerEnter(Collider other) {
    //     // Handle collisions with obstacles and NPCs
    //     if (other.CompareTag("Obstacle")) {
    //         TakeDamage(20f);
    //         // Slow down the bike
    //         rb.linearVelocity *= 0.5f;
    //     }
    //     else if (other.CompareTag("NPC")) {
    //         TakeDamage(15f);
    //         rb.linearVelocity *= 0.7f;
    //     }
    //     else if (other.CompareTag("Finish")) {
    //         if (gameManager) {
    //             gameManager.EndRace();
    //         }
    //     }
    // }
}

public enum TerrainType {
    Bitumen,
    Gravel,
    Dirt,
    OffRoad
}