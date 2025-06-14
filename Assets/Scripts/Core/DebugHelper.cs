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
        Debug.Log("=== TIME RIDERS DEBUG ===");
        Debug.Log($"GameManager exists: {FindFirstObjectByType<GameManager>() != null}");
        Debug.Log($"Players found: {FindObjectsByType<PlayerController>(FindObjectsSortMode.None).Length}");
        Debug.Log($"Canvas exists: {FindFirstObjectByType<Canvas>() != null}");
        Debug.Log($"Camera exists: {Camera.main != null}");
        
        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player)
        {
            Debug.Log($"Player position: {player.transform.position}");
            Debug.Log($"Player has Rigidbody: {player.GetComponent<Rigidbody>() != null}");
            Debug.Log($"Wheel colliders: {player.wheelColliders?.Length ?? 0}");
        }
    }
}
