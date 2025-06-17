using UnityEngine;

public class FinishLine : MonoBehaviour
{
    [Header("Finish Line Settings")]
    public GameObject flagPrefab;
    public GameObject tapePrefab;
    public float lineWidth = 10f;
    
    void Start()
    {
        SetupFinishLine();
    }
    
    void SetupFinishLine()
    {
        // Create flags on both sides
        // if (flagPrefab)
        // {
        //     Vector3 leftFlagPos = transform.position + Vector3.left * (lineWidth / 2);
        //     Vector3 rightFlagPos = transform.position + Vector3.right * (lineWidth / 2);
            
        //     Instantiate(flagPrefab, leftFlagPos, Quaternion.identity, transform);
        //     Instantiate(flagPrefab, rightFlagPos, Quaternion.identity, transform);
        // }
        
        // // Create tape across the line
        // if (tapePrefab)
        // {
        //     GameObject tape = Instantiate(tapePrefab, transform.position, Quaternion.identity, transform);
        //     tape.transform.localScale = new Vector3(lineWidth, 1f, 1f);
        // }
    }
    
    void OnTriggerEnter(Collider other)
    {
        // PlayerController player = other.GetComponent<PlayerController>();
        // if (player)
        // {
        //     GameManager gameManager = FindFirstObjectByType<GameManager>();
        //     if (gameManager)
        //     {
        //         gameManager.EndRace();
        //     }
        // }
    }
}