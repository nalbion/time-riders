using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform target;
    public float followDistance = 10f;
    public float followHeight = 5f;
    public float rotationSpeed = 2f;
    public float followSpeed = 3f;
    
    [Header("Split Screen")]
    public bool splitScreenMode = false;
    public Camera player2Camera;
    
    private Vector3 targetPosition;
    
    void Start()
    {
        if (splitScreenMode && player2Camera)
        {
            // Setup split screen
            GetComponent<Camera>().rect = new Rect(0, 0.5f, 1, 0.5f);
            player2Camera.rect = new Rect(0, 0, 1, 0.5f);
        }
    }
    
    void LateUpdate()
    {
        if (!target) return;
        
        // Calculate desired position
        Vector3 desiredPosition = target.position - target.forward * followDistance + Vector3.up * followHeight;
        
        // Smoothly move camera
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
        
        // Look at target
        transform.LookAt(target.position + Vector3.up * 2f);
    }
    
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}