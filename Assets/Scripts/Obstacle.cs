using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [Header("Obstacle Settings")]
    public ObstacleType obstacleType;
    public float damage = 20f;
    public float slowdownFactor = 0.5f;
    
    [Header("Animation")]
    public bool shouldRotate = false;
    public Vector3 rotationSpeed = Vector3.zero;
    
    void Update()
    {
        if (shouldRotate)
        {
            transform.Rotate(rotationSpeed * Time.deltaTime);
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player)
        {
            player.TakeDamage(damage);
            
            // Apply specific obstacle effects
            switch (obstacleType)
            {
                case ObstacleType.Spinner:
                    // Spin the player
                    other.GetComponent<Rigidbody>().AddTorque(Vector3.up * 1000f);
                    break;
                case ObstacleType.Saw:
                    // Extra damage
                    player.TakeDamage(damage * 0.5f);
                    break;
                case ObstacleType.TapeMeasure:
                    // Temporary speed reduction
                    StartCoroutine(TemporarySpeedReduction(player));
                    break;
                case ObstacleType.AllenKeys:
                    // Slight steering disruption
                    other.GetComponent<Rigidbody>().AddForce(Vector3.right * 500f);
                    break;
            }
        }
    }
    
    System.Collections.IEnumerator TemporarySpeedReduction(PlayerController player)
    {
        float originalMaxSpeed = player.maxSpeed;
        player.maxSpeed *= 0.5f;
        
        yield return new WaitForSeconds(3f);
        
        player.maxSpeed = originalMaxSpeed;
    }
}

public enum ObstacleType
{
    Spinner,
    Saw,
    TapeMeasure,
    AllenKeys
}