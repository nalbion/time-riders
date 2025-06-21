using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages course/track selection with previews and difficulty information
/// </summary>
public class CourseSelectionManager : MonoBehaviour 
{
    [Header("UI References")]
    [SerializeField] private Transform courseContainer;
    [SerializeField] private GameObject courseButtonPrefab;
    [SerializeField] private Image coursePreviewImage;
    [SerializeField] private Text courseNameText;
    [SerializeField] private Text courseDifficultyText;
    [SerializeField] private Text courseDescriptionText;
    [SerializeField] private Button startRaceButton;
    [SerializeField] private Button backButton;
    
    [Header("Course Navigation")]
    [SerializeField] private Button previousCourseButton;
    [SerializeField] private Button nextCourseButton;
    
    [Header("Course Data")]
    [SerializeField] private List<CourseData> availableCourses = new List<CourseData>();
    
    private int selectedCourseIndex = 0;
    private List<Button> courseButtons = new List<Button>();
    
    private void Start() 
    {
        SetupUI();
        PopulateCourseList();
        UpdateCourseDisplay();
    }
    
    private void SetupUI() 
    {
        // Setup button listeners
        startRaceButton.onClick.AddListener(StartRace);
        backButton.onClick.AddListener(GoBack);
        previousCourseButton.onClick.AddListener(() => ChangeCourse(-1));
        nextCourseButton.onClick.AddListener(() => ChangeCourse(1));
    }
    
    private void PopulateCourseList() 
    {
        // Clear existing buttons
        foreach (Transform child in courseContainer) 
        {
            Destroy(child.gameObject);
        }
        courseButtons.Clear();
        
        // Create buttons for each course
        for (int i = 0; i < availableCourses.Count; i++) 
        {
            int courseIndex = i; // Capture for closure
            GameObject buttonObj = Instantiate(courseButtonPrefab, courseContainer);
            Button button = buttonObj.GetComponent<Button>();
            
            // Setup button appearance
            CourseButton courseButtonScript = buttonObj.GetComponent<CourseButton>();
            if (courseButtonScript != null) 
            {
                courseButtonScript.SetupCourse(availableCourses[i]);
            }
            else 
            {
                // Fallback if CourseButton script doesn't exist
                Text buttonText = buttonObj.GetComponentInChildren<Text>();
                if (buttonText != null) 
                {
                    buttonText.text = availableCourses[i].courseName;
                }
            }
            
            // Setup button listener
            button.onClick.AddListener(() => SelectCourse(courseIndex));
            courseButtons.Add(button);
        }
    }
    
    private void SelectCourse(int index) 
    {
        if (index < 0 || index >= availableCourses.Count) return;
        
        selectedCourseIndex = index;
        UpdateCourseDisplay();
        UpdateButtonHighlighting();
    }
    
    private void ChangeCourse(int direction) 
    {
        int newIndex = selectedCourseIndex + direction;
        
        // Wrap around
        if (newIndex < 0) 
        {
            newIndex = availableCourses.Count - 1;
        }
        else if (newIndex >= availableCourses.Count) 
        {
            newIndex = 0;
        }
        
        SelectCourse(newIndex);
    }
    
    private void UpdateCourseDisplay() 
    {
        if (selectedCourseIndex < 0 || selectedCourseIndex >= availableCourses.Count) return;
        
        CourseData selectedCourse = availableCourses[selectedCourseIndex];
        
        // Update UI elements
        courseNameText.text = selectedCourse.courseName;
        courseDifficultyText.text = GetDifficultyText(selectedCourse.difficulty);
        courseDescriptionText.text = selectedCourse.description;
        
        if (selectedCourse.previewImage != null) 
        {
            coursePreviewImage.sprite = selectedCourse.previewImage;
        }
        
        // Update difficulty color
        Color difficultyColor = GetDifficultyColor(selectedCourse.difficulty);
        courseDifficultyText.color = difficultyColor;
    }
    
    private void UpdateButtonHighlighting() 
    {
        for (int i = 0; i < courseButtons.Count; i++) 
        {
            // Highlight selected button
            ColorBlock colors = courseButtons[i].colors;
            colors.normalColor = (i == selectedCourseIndex) ? Color.yellow : Color.white;
            courseButtons[i].colors = colors;
        }
    }
    
    private string GetDifficultyText(CourseDifficulty difficulty) 
    {
        switch (difficulty) 
        {
            case CourseDifficulty.Easy: return "Easy";
            case CourseDifficulty.Medium: return "Medium";
            case CourseDifficulty.Hard: return "Hard";
            case CourseDifficulty.Expert: return "Expert";
            default: return "Unknown";
        }
    }
    
    private Color GetDifficultyColor(CourseDifficulty difficulty) 
    {
        switch (difficulty) 
        {
            case CourseDifficulty.Easy: return Color.green;
            case CourseDifficulty.Medium: return Color.yellow;
            case CourseDifficulty.Hard: return Color.red;
            case CourseDifficulty.Expert: return Color.magenta;
            default: return Color.white;
        }
    }
    
    private void StartRace() 
    {
        if (selectedCourseIndex < 0 || selectedCourseIndex >= availableCourses.Count) return;
        
        // Store selection data
        SelectionData.SelectedTrack = selectedCourseIndex;
        
        // Save last played configuration
        SaveLastPlayedConfig();
        
        // Load the race scene
        CourseData selectedCourse = availableCourses[selectedCourseIndex];
        SceneManager.LoadScene(selectedCourse.scenePath);
    }
    
    private void SaveLastPlayedConfig() 
    {
        // Save to PlayerPrefs for quick replay
        if (SelectionData.SelectedCharacter != null) 
        {
            PlayerPrefs.SetString("LastCharacter", SelectionData.SelectedCharacter.characterName);
        }
        PlayerPrefs.SetInt("LastGameMode", SelectionData.SelectedGameMode);
        PlayerPrefs.SetInt("LastTrack", SelectionData.SelectedTrack);
        PlayerPrefs.Save();
    }
    
    private void GoBack() 
    {
        SceneManager.LoadScene("CharacterSelect");
    }
}

[System.Serializable]
public enum CourseDifficulty 
{
    Easy,
    Medium,
    Hard,
    Expert
}
