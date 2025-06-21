using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Individual course button component for the course selection grid
/// </summary>
public class CourseButton : MonoBehaviour 
{
    [Header("UI References")]
    [SerializeField] private Image courseImage;
    [SerializeField] private Text courseNameText;
    [SerializeField] private Text difficultyText;
    [SerializeField] private Image difficultyIcon;
    [SerializeField] private GameObject lockedOverlay;
    [SerializeField] private Image bestTimeIcon;
    [SerializeField] private Text bestTimeText;
    
    private CourseData courseData;
    private bool isLocked = false;
    
    public void SetupCourse(CourseData data) 
    {
        courseData = data;
        
        // Set course name
        if (courseNameText != null) 
        {
            courseNameText.text = data.courseName;
        }
        
        // Set course image
        if (courseImage != null && data.previewImage != null) 
        {
            courseImage.sprite = data.previewImage;
        }
        
        // Set difficulty
        if (difficultyText != null) 
        {
            difficultyText.text = GetDifficultyText(data.difficulty);
            difficultyText.color = GetDifficultyColor(data.difficulty);
        }
        
        // Set difficulty icon color
        if (difficultyIcon != null) 
        {
            difficultyIcon.color = GetDifficultyColor(data.difficulty);
        }
        
        // Set best time if available
        UpdateBestTime();
        
        // Check if course is unlocked
        CheckUnlockStatus();
    }
    
    private void UpdateBestTime() 
    {
        if (bestTimeText != null) 
        {
            float savedBestTime = PlayerPrefs.GetFloat($"BestTime_{courseData.courseName}", 0f);
            
            if (savedBestTime > 0) 
            {
                bestTimeText.text = FormatTime(savedBestTime);
                if (bestTimeIcon != null) 
                {
                    bestTimeIcon.gameObject.SetActive(true);
                }
            }
            else 
            {
                bestTimeText.text = "--:--";
                if (bestTimeIcon != null) 
                {
                    bestTimeIcon.gameObject.SetActive(false);
                }
            }
        }
    }
    
    private void CheckUnlockStatus() 
    {
        // For now, all courses are unlocked
        // In a full game, you might check progression requirements
        isLocked = false;
        
        if (lockedOverlay != null) 
        {
            lockedOverlay.SetActive(isLocked);
        }
        
        // Disable button if locked
        Button button = GetComponent<Button>();
        if (button != null) 
        {
            button.interactable = !isLocked;
        }
    }
    
    private string GetDifficultyText(CourseDifficulty difficulty) 
    {
        switch (difficulty) 
        {
            case CourseDifficulty.Easy: return "★☆☆";
            case CourseDifficulty.Medium: return "★★☆";
            case CourseDifficulty.Hard: return "★★★";
            case CourseDifficulty.Expert: return "★★★★";
            default: return "?";
        }
    }
    
    private Color GetDifficultyColor(CourseDifficulty difficulty) 
    {
        switch (difficulty) 
        {
            case CourseDifficulty.Easy: return new Color(0.3f, 0.8f, 0.3f); // Green
            case CourseDifficulty.Medium: return new Color(0.9f, 0.7f, 0.2f); // Yellow
            case CourseDifficulty.Hard: return new Color(0.9f, 0.3f, 0.2f); // Red
            case CourseDifficulty.Expert: return new Color(0.7f, 0.2f, 0.9f); // Purple
            default: return Color.white;
        }
    }
    
    private string FormatTime(float timeInSeconds) 
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
        int milliseconds = Mathf.FloorToInt((timeInSeconds * 100f) % 100f);
        
        return string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, milliseconds);
    }
    
    public CourseData GetCourseData() 
    {
        return courseData;
    }
    
    public bool IsLocked() 
    {
        return isLocked;
    }
}
