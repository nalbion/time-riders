using UnityEngine;

/// <summary>
/// ScriptableObject for storing course/track data
/// </summary>
[CreateAssetMenu(fileName = "CourseData", menuName = "TimeRiders/Course Data", order = 1)]
public class CourseData : ScriptableObject 
{
    [Header("Course Info")]
    public string courseName;
    [TextArea(3, 5)]
    public string description;
    public Sprite previewImage;
    public CourseDifficulty difficulty;
    
    [Header("Scene Settings")]
    public string scenePath;
    public float bestTime = 0f; // For leaderboards
    
    [Header("Course Stats")]
    public int laps = 3;
    public float estimatedTime = 120f; // seconds
    public bool hasJumps = false;
    public bool hasObstacles = false;
    public bool weatherEffects = false;
    
    [Header("Unlock Requirements")]
    public bool isUnlocked = true;
    public CourseData[] requiredCoursesToComplete;
    public int minimumStarsRequired = 0;
    
    /// <summary>
    /// Check if this course is unlocked based on progression
    /// </summary>
    /// <returns>True if unlocked</returns>
    public bool IsUnlocked() 
    {
        if (isUnlocked) return true;
        
        // Check if required courses are completed
        if (requiredCoursesToComplete != null && requiredCoursesToComplete.Length > 0) 
        {
            foreach (var requiredCourse in requiredCoursesToComplete) 
            {
                string completionKey = $"CourseCompleted_{requiredCourse.courseName}";
                if (!PlayerPrefs.HasKey(completionKey) || PlayerPrefs.GetInt(completionKey) == 0) 
                {
                    return false;
                }
            }
        }
        
        // Check star requirements
        if (minimumStarsRequired > 0) 
        {
            int totalStars = PlayerPrefs.GetInt("TotalStars", 0);
            if (totalStars < minimumStarsRequired) 
            {
                return false;
            }
        }
        
        return true;
    }
    
    /// <summary>
    /// Get the best time for this course
    /// </summary>
    /// <returns>Best time in seconds, 0 if no time recorded</returns>
    public float GetBestTime() 
    {
        return PlayerPrefs.GetFloat($"BestTime_{courseName}", 0f);
    }
    
    /// <summary>
    /// Set a new best time if it's better than the current one
    /// </summary>
    /// <param name="newTime">New time to compare</param>
    /// <returns>True if it's a new record</returns>
    public bool SetBestTime(float newTime) 
    {
        float currentBest = GetBestTime();
        
        if (currentBest == 0f || newTime < currentBest) 
        {
            PlayerPrefs.SetFloat($"BestTime_{courseName}", newTime);
            PlayerPrefs.Save();
            return true;
        }
        
        return false;
    }
    
    /// <summary>
    /// Mark this course as completed
    /// </summary>
    /// <param name="stars">Number of stars earned (1-3)</param>
    public void MarkCompleted(int stars = 1) 
    {
        PlayerPrefs.SetInt($"CourseCompleted_{courseName}", 1);
        
        // Update best star rating
        string starKey = $"CourseStars_{courseName}";
        int currentStars = PlayerPrefs.GetInt(starKey, 0);
        if (stars > currentStars) 
        {
            PlayerPrefs.SetInt(starKey, stars);
            
            // Update total stars
            int totalStars = PlayerPrefs.GetInt("TotalStars", 0);
            totalStars += (stars - currentStars);
            PlayerPrefs.SetInt("TotalStars", totalStars);
        }
        
        PlayerPrefs.Save();
    }
    
    /// <summary>
    /// Get the star rating for this course
    /// </summary>
    /// <returns>Star rating (0-3)</returns>
    public int GetStarRating() 
    {
        return PlayerPrefs.GetInt($"CourseStars_{courseName}", 0);
    }
}
