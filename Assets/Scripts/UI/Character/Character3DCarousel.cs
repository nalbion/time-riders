using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages a 3D carousel of character models that rotates around a vertical axis
/// </summary>
public class Character3DCarousel : MonoBehaviour 
{
    [Header("Carousel Settings")]
    [SerializeField] private Transform carouselCenter;
    [SerializeField] private float radius = 3f;
    [SerializeField] private float rotationSpeed = 90f; // degrees per second
    [SerializeField] private float characterSpacing = 60f; // degrees between characters
    
    [Header("Input Settings")]
    [SerializeField] private bool enableSwipeInput = true;
    [SerializeField] private float swipeSensitivity = 100f;
    [SerializeField] private float autoRotateDelay = 3f; // seconds of inactivity before auto-rotate
    
    [Header("Character Models")]
    [SerializeField] private List<GameObject> characterModels = new List<GameObject>();
    [SerializeField] private List<CharacterData> characterData = new List<CharacterData>();
    
    [Header("UI References")]
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    
    // Private variables
    private int currentCharacterIndex = 0;
    private bool isRotating = false;
    private float targetRotation = 0f;
    private float currentRotation = 0f;
    private float lastInputTime;
    private bool autoRotateEnabled = false;
    
    // Touch/Mouse input
    private Vector2 startTouchPosition;
    private bool isTouching = false;
    
    public System.Action<int, CharacterData> OnCharacterChanged;
    
    private void Start() 
    {
        SetupCarousel();
        SetupInput();
        UpdateCharacterPositions();
        
        // Initialize with first character
        OnCharacterChanged?.Invoke(currentCharacterIndex, characterData[currentCharacterIndex]);
    }
    
    private void SetupCarousel() 
    {
        if (carouselCenter == null) 
        {
            carouselCenter = transform;
        }
        
        // Ensure we have matching character models and data
        if (characterModels.Count != characterData.Count) 
        {
            Debug.LogWarning("Character models and data count mismatch!");
        }
    }
    
    private void SetupInput() 
    {
        // Setup button listeners
        if (leftButton != null) 
        {
            leftButton.onClick.AddListener(() => RotateToCharacter(-1));
        }
        
        if (rightButton != null) 
        {
            rightButton.onClick.AddListener(() => RotateToCharacter(1));
        }
    }
    
    private void Update() 
    {
        HandleInput();
        UpdateRotation();
        CheckAutoRotate();
    }
    
    private void HandleInput() 
    {
        if (!enableSwipeInput) return;
        
        // Handle mouse/touch input for swiping
        if (Input.GetMouseButtonDown(0)) 
        {
            startTouchPosition = Input.mousePosition;
            isTouching = true;
            lastInputTime = Time.time;
        }
        else if (Input.GetMouseButtonUp(0) && isTouching) 
        {
            Vector2 endTouchPosition = Input.mousePosition;
            Vector2 swipeDelta = endTouchPosition - startTouchPosition;
            
            // Check if it's a significant horizontal swipe
            if (Mathf.Abs(swipeDelta.x) > swipeSensitivity) 
            {
                if (swipeDelta.x > 0) 
                {
                    RotateToCharacter(1); // Swipe right
                }
                else 
                {
                    RotateToCharacter(-1); // Swipe left
                }
            }
            
            isTouching = false;
        }
        
        // Handle keyboard input
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) 
        {
            RotateToCharacter(-1);
            lastInputTime = Time.time;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) 
        {
            RotateToCharacter(1);
            lastInputTime = Time.time;
        }
    }
    
    private void UpdateRotation() 
    {
        if (isRotating) 
        {
            // Smoothly rotate towards target
            currentRotation = Mathf.MoveTowardsAngle(currentRotation, targetRotation, rotationSpeed * Time.deltaTime);
            
            // Apply rotation to carousel center
            carouselCenter.rotation = Quaternion.Euler(0, currentRotation, 0);
            
            // Check if rotation is complete
            if (Mathf.Approximately(currentRotation, targetRotation)) 
            {
                isRotating = false;
                currentRotation = targetRotation;
            }
        }
    }
    
    private void CheckAutoRotate() 
    {
        if (autoRotateEnabled && !isRotating && Time.time - lastInputTime > autoRotateDelay) 
        {
            RotateToCharacter(1);
            lastInputTime = Time.time;
        }
    }
    
    public void RotateToCharacter(int direction) 
    {
        if (isRotating) return;
        
        int newIndex = currentCharacterIndex + direction;
        
        // Wrap around
        if (newIndex < 0) 
        {
            newIndex = characterModels.Count - 1;
        }
        else if (newIndex >= characterModels.Count) 
        {
            newIndex = 0;
        }
        
        SetCharacterIndex(newIndex);
    }
    
    public void SetCharacterIndex(int index) 
    {
        if (index < 0 || index >= characterModels.Count) return;
        
        currentCharacterIndex = index;
        targetRotation = -index * characterSpacing; // Negative for counter-clockwise
        isRotating = true;
        
        // Highlight the selected character
        HighlightCharacter(index);
        
        // Notify listeners
        OnCharacterChanged?.Invoke(currentCharacterIndex, characterData[currentCharacterIndex]);
    }
    
    private void UpdateCharacterPositions() 
    {
        for (int i = 0; i < characterModels.Count; i++) 
        {
            if (characterModels[i] == null) continue;
            
            // Calculate position on circle
            float angle = i * characterSpacing * Mathf.Deg2Rad;
            Vector3 position = new Vector3(
                Mathf.Sin(angle) * radius,
                0,
                Mathf.Cos(angle) * radius
            );
            
            // Position relative to carousel center
            characterModels[i].transform.position = carouselCenter.position + position;
            
            // Make characters face center
            characterModels[i].transform.LookAt(carouselCenter.position);
            characterModels[i].transform.Rotate(0, 180, 0); // Face outward instead
        }
    }
    
    private void HighlightCharacter(int index) 
    {
        // Reset all character highlighting
        for (int i = 0; i < characterModels.Count; i++) 
        {
            if (characterModels[i] == null) continue;
            
            // Scale and lighting effects for highlighting
            float scale = (i == index) ? 1.2f : 1.0f;
            characterModels[i].transform.localScale = Vector3.one * scale;
            
            // You can add additional highlighting effects here
            // e.g., outline shaders, particle effects, etc.
        }
    }
    
    public CharacterData GetCurrentCharacterData() 
    {
        if (currentCharacterIndex >= 0 && currentCharacterIndex < characterData.Count) 
        {
            return characterData[currentCharacterIndex];
        }
        return null;
    }
    
    public int GetCurrentCharacterIndex() 
    {
        return currentCharacterIndex;
    }
    
    public void SetAutoRotate(bool enabled) 
    {
        autoRotateEnabled = enabled;
    }
    
    // Method to add new character at runtime
    public void AddCharacter(GameObject model, CharacterData data) 
    {
        characterModels.Add(model);
        characterData.Add(data);
        UpdateCharacterPositions();
    }
}
