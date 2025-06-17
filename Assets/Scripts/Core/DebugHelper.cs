using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugHelper : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            DebugGameState();
        }
    }
    
    void DebugGameState()
    {
        // GameLogger.Info("DebugHelper", "=== TIME RIDERS DEBUG ===");
        // GameLogger.Info("DebugHelper", $"GameManager exists: {FindFirstObjectByType<GameManager>() != null}");
        // GameLogger.Info("DebugHelper", $"Players found: {FindObjectsByType<PlayerController>(FindObjectsSortMode.None).Length}");
        // GameLogger.Info("DebugHelper", $"Canvas exists: {FindFirstObjectByType<Canvas>() != null}");
        // GameLogger.Info("DebugHelper", $"Camera exists: {Camera.main != null}");
        
        // PlayerController player = FindFirstObjectByType<PlayerController>();
        // if (player)
        // {
        //     GameLogger.Info("DebugHelper", $"Player position: {player.transform.position}");
        //     GameLogger.Info("DebugHelper", $"Player has Rigidbody: {player.GetComponent<Rigidbody>() != null}");
        //     GameLogger.Info("DebugHelper", $"Wheel colliders: {player.wheelColliders?.Length ?? 0}");
        // }
    }
}
