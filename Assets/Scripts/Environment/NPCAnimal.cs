using UnityEngine;

public class NPCAnimal : MonoBehaviour
{
    [Header("Animal Settings")]
    public AnimalType animalType;
    public float moveSpeed = 5f;
    public float damage = 15f;
    
    [Header("Movement")]
    public Transform[] waypoints;
    public bool randomMovement = true;
    
    private int currentWaypointIndex = 0;
    private float nextMoveTime;
    private Animator animator;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        nextMoveTime = Time.time + Random.Range(2f, 8f);
    }
    
    void Update()
    {
        if (randomMovement)
        {
            RandomMovement();
        }
        else
        {
            PatrolMovement();
        }
    }
    
    void RandomMovement()
    {
        if (Time.time >= nextMoveTime)
        {
            // Move to a random position
            Vector3 randomDirection = Random.insideUnitSphere * 10f;
            randomDirection.y = 0;
            
            Vector3 targetPosition = transform.position + randomDirection;
            StartCoroutine(MoveToPosition(targetPosition));
            
            nextMoveTime = Time.time + Random.Range(3f, 10f);
        }
    }
    
    void PatrolMovement()
    {
        if (waypoints.Length == 0) return;
        
        Transform targetWaypoint = waypoints[currentWaypointIndex];
        float distance = Vector3.Distance(transform.position, targetWaypoint.position);
        
        if (distance < 1f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
        else
        {
            Vector3 direction = (targetWaypoint.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
            transform.LookAt(targetWaypoint.position);
        }
    }
    
    System.Collections.IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        Vector3 startPosition = transform.position;
        float journey = 0f;
        float journeyLength = Vector3.Distance(startPosition, targetPosition);
        
        // Set animation based on animal type
        if (animator)
        {
            switch (animalType)
            {
                case AnimalType.Kangaroo:
                    animator.SetTrigger("Hop");
                    break;
                case AnimalType.Dog:
                    animator.SetTrigger("Run");
                    break;
                case AnimalType.Possum:
                    animator.SetTrigger("Scurry");
                    break;
                case AnimalType.Chameleon:
                    animator.SetTrigger("Walk");
                    break;
            }
        }
        
        while (journey < journeyLength)
        {
            journey += moveSpeed * Time.deltaTime;
            float fractionOfJourney = journey / journeyLength;
            transform.position = Vector3.Lerp(startPosition, targetPosition, fractionOfJourney);
            
            // Look at target
            Vector3 direction = (targetPosition - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
            
            yield return null;
        }
        
        if (animator)
        {
            animator.SetTrigger("Idle");
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player)
        {
            player.TakeDamage(damage);
            
            // Animal-specific reactions
            switch (animalType)
            {
                case AnimalType.Kangaroo:
                    // Kangaroo bounces player up
                    other.GetComponent<Rigidbody>().AddForce(Vector3.up * 300f);
                    break;
                case AnimalType.Dog:
                    // Dog barks and causes slight confusion
                    StartCoroutine(CauseConfusion(player));
                    break;
            }
        }
    }
    
    System.Collections.IEnumerator CauseConfusion(PlayerController player)
    {
        // Temporarily invert controls or add steering difficulty
        yield return new WaitForSeconds(2f);
        // Reset any effects
    }
}

public enum AnimalType
{
    Kangaroo,
    Dog,
    Possum,
    Chameleon
}