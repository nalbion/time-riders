using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

/// <summary>
/// Provides visual feedback for button interactions including hover, press, and navigation states
/// </summary>
[RequireComponent(typeof(Button))]
public class ButtonVisualEffects : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, ISelectHandler, IDeselectHandler
{
    [Header("Visual Settings")]
    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private float pressScale = 0.95f;
    [SerializeField] private float animationDuration = 0.2f;
    
    [Header("Color Settings")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color hoverColor = new Color(1f, 1f, 0.8f, 1f); // Light yellow
    [SerializeField] private Color pressedColor = new Color(0.8f, 0.8f, 0.8f, 1f); // Light gray
    [SerializeField] private Color selectedColor = new Color(0.9f, 0.9f, 1f, 1f); // Light blue
    
    [Header("Effects")]
    [SerializeField] private bool enableScaleEffect = true;
    [SerializeField] private bool enableColorEffect = true;
    [SerializeField] private bool enableSoundEffect = true;
    
    private Button button;
    private Image buttonImage;
    private RectTransform rectTransform;
    private Vector3 originalScale;
    private Color originalColor;
    private bool isPressed = false;
    private bool isHovered = false;
    private bool isSelected = false;
    
    // Coroutine references for cleanup
    private Coroutine scaleCoroutine;
    private Coroutine colorCoroutine;
    
    private void Awake()
    {
        button = GetComponent<Button>();
        buttonImage = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        
        // Store original values
        originalScale = rectTransform.localScale;
        if (buttonImage != null)
        {
            originalColor = buttonImage.color;
        }
    }
    
    private void Start()
    {
        // Set initial normal color if color effect is enabled
        if (enableColorEffect && buttonImage != null)
        {
            buttonImage.color = normalColor;
            originalColor = normalColor;
        }
    }
    
    private void OnDestroy()
    {
        // Clean up coroutines
        if (scaleCoroutine != null)
            StopCoroutine(scaleCoroutine);
        if (colorCoroutine != null)
            StopCoroutine(colorCoroutine);
    }
    
    #region Pointer Events
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!button.interactable) return;
        
        isHovered = true;
        UpdateVisualState();
        
        if (enableSoundEffect)
        {
            PlayHoverSound();
        }
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        UpdateVisualState();
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!button.interactable) return;
        
        isPressed = true;
        UpdateVisualState();
        
        if (enableSoundEffect)
        {
            PlayPressSound();
        }
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        UpdateVisualState();
    }
    
    #endregion
    
    #region Selection Events (for keyboard/gamepad navigation)
    
    public void OnSelect(BaseEventData eventData)
    {
        if (!button.interactable) return;
        
        isSelected = true;
        UpdateVisualState();
    }
    
    public void OnDeselect(BaseEventData eventData)
    {
        isSelected = false;
        UpdateVisualState();
    }
    
    #endregion
    
    private void UpdateVisualState()
    {
        if (!button.interactable)
        {
            ResetToNormal();
            return;
        }
        
        // Determine target scale and color based on current state
        float targetScale = originalScale.x;
        Color targetColor = originalColor;
        
        if (isPressed)
        {
            targetScale = originalScale.x * pressScale;
            targetColor = enableColorEffect ? pressedColor : originalColor;
        }
        else if (isHovered || isSelected)
        {
            targetScale = originalScale.x * hoverScale;
            targetColor = enableColorEffect ? (isSelected ? selectedColor : hoverColor) : originalColor;
        }
        else
        {
            targetScale = originalScale.x;
            targetColor = enableColorEffect ? normalColor : originalColor;
        }
        
        // Apply scale effect
        if (enableScaleEffect)
        {
            AnimateScale(new Vector3(targetScale, targetScale, targetScale));
        }
        
        // Apply color effect
        if (enableColorEffect && buttonImage != null)
        {
            AnimateColor(targetColor);
        }
    }
    
    private void AnimateScale(Vector3 targetScale)
    {
        if (!gameObject.activeInHierarchy)
        {
            // If the GameObject is inactive, just set the scale directly without animation
            rectTransform.localScale = targetScale;
            return;
        }
        
        if (scaleCoroutine != null)
            StopCoroutine(scaleCoroutine);
        scaleCoroutine = StartCoroutine(ScaleCoroutine(targetScale));
    }
    
    private IEnumerator ScaleCoroutine(Vector3 targetScale)
    {
        float elapsedTime = 0;
        Vector3 initialScale = rectTransform.localScale;
        while (elapsedTime < animationDuration)
        {
            rectTransform.localScale = Vector3.Lerp(initialScale, targetScale, elapsedTime / animationDuration);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        rectTransform.localScale = targetScale;
    }
    
    private void AnimateColor(Color targetColor)
    {
        if (!enableColorEffect || buttonImage == null) return;
        
        if (!gameObject.activeInHierarchy)
        {
            // If the GameObject is inactive, just set the color directly without animation
            buttonImage.color = targetColor;
            return;
        }
        
        if (colorCoroutine != null)
            StopCoroutine(colorCoroutine);
        colorCoroutine = StartCoroutine(ColorCoroutine(targetColor));
    }
    
    private IEnumerator ColorCoroutine(Color targetColor)
    {
        float elapsedTime = 0;
        Color initialColor = buttonImage.color;
        while (elapsedTime < animationDuration)
        {
            buttonImage.color = Color.Lerp(initialColor, targetColor, elapsedTime / animationDuration);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        buttonImage.color = targetColor;
    }
    
    private void ResetToNormal()
    {
        if (enableScaleEffect)
        {
            AnimateScale(originalScale);
        }
        
        if (enableColorEffect && buttonImage != null)
        {
            AnimateColor(originalColor);
        }
    }
    
    private void PlayHoverSound()
    {
        // TODO: Implement audio system integration
        // AudioManager.Instance?.PlaySFX("ButtonHover");
        Debug.Log("Button hover sound");
    }
    
    private void PlayPressSound()
    {
        // TODO: Implement audio system integration
        // AudioManager.Instance?.PlaySFX("ButtonPress");
        Debug.Log("Button press sound");
    }
    
    #region Public Methods for Manual Control
    
    /// <summary>
    /// Manually set the button to hover state (useful for animations or special effects)
    /// </summary>
    public void SetHoverState(bool hover)
    {
        isHovered = hover;
        UpdateVisualState();
    }
    
    /// <summary>
    /// Reset button to normal state
    /// </summary>
    public void ResetButton()
    {
        isHovered = false;
        isPressed = false;
        isSelected = false;
        UpdateVisualState();
    }
    
    /// <summary>
    /// Configure colors at runtime
    /// </summary>
    public void SetColors(Color normal, Color hover, Color pressed, Color selected)
    {
        normalColor = normal;
        hoverColor = hover;
        pressedColor = pressed;
        selectedColor = selected;
        
        // Update the visual state only if the GameObject is active
        if (gameObject.activeInHierarchy)
        {
            UpdateVisualState();
        }
        else
        {
            // If inactive, just set the normal color directly without animation
            if (buttonImage != null)
            {
                buttonImage.color = normalColor;
            }
        }
    }
    
    #endregion
}
