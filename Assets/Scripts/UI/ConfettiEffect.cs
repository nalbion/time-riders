using UnityEngine;

public class ConfettiEffect : MonoBehaviour
{
    [Header("Confetti Settings")]
    public ParticleSystem confettiParticles;
    public GameObject audDollarNotePrefab;
    public int numberOfNotes = 20;
    public float spawnRadius = 5f;
    public float fallSpeed = 2f;
    
    void OnEnable()
    {
        StartConfetti();
    }
    
    void StartConfetti()
    {
        // Start particle system
        if (confettiParticles)
        {
            confettiParticles.Play();
        }
        
        // Spawn AUD dollar notes
        if (audDollarNotePrefab)
        {
            for (int i = 0; i < numberOfNotes; i++)
            {
                Vector3 spawnPos = transform.position + Random.insideUnitSphere * spawnRadius;
                spawnPos.y += 5f; // Spawn above
                
                GameObject note = Instantiate(audDollarNotePrefab, spawnPos, Random.rotation);
                
                // Add falling physics
                Rigidbody rb = note.GetComponent<Rigidbody>();
                if (!rb)
                {
                    rb = note.AddComponent<Rigidbody>();
                }
                
                rb.linearDamping = 5f; // Slow fall
                rb.AddForce(Vector3.down * fallSpeed, ForceMode.VelocityChange);
                
                // Destroy after falling
                Destroy(note, 10f);
            }
        }
        
        // Auto-disable after duration
        Invoke("StopConfetti", 5f);
    }
    
    void StopConfetti()
    {
        if (confettiParticles)
        {
            confettiParticles.Stop();
        }
        
        gameObject.SetActive(false);
    }
}