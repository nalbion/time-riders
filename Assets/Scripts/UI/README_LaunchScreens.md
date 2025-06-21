# Time Riders Launch Screen System

This system provides a comprehensive launch screen experience for Time Riders, including character selection with 3D carousel, course selection, and quick replay functionality.

## Components Overview

### 1. MainMenuManager
**Location**: `Assets/Scripts/UI/MainMenuManager.cs`

The main entry point showing:
- Quick Start button (if previous game data exists)
- New Game button
- Settings button
- Exit button

**Unity Setup Required**:
- Create a MainMenu scene
- Add UI Canvas with MainMenuManager script
- Assign UI button references
- Configure `isMobileBuild` for platform-specific features

### 2. Character3DCarousel
**Location**: `Assets/Scripts/UI/Character/Character3DCarousel.cs`

Provides a 3D rotating carousel of character models:
- Touch/swipe support for mobile
- Keyboard arrow key support
- Button navigation fallback
- Auto-rotation feature
- Character highlighting

**Unity Setup Required**:
- Create character 3D models and place them in scene
- Set up carousel center transform
- Configure radius and rotation settings
- Assign character model GameObjects to the list
- Connect character data ScriptableObjects

### 3. Enhanced CharacterSelectionManager
**Location**: `Assets/Scripts/UI/Character/CharacterSelectionManager.cs`

Enhanced character selection with:
- 3D carousel integration
- Mobile/desktop mode detection
- Improved stat display with sliders
- Platform-specific game mode options

**Unity Setup Required**:
- Update existing CharacterSelect scene
- Add Character3DCarousel component
- Add stat sliders (Speed, Health, Jump)
- Configure mobile build detection

### 4. CourseSelectionManager & CourseButton
**Location**: `Assets/Scripts/UI/Course/CourseSelectionManager.cs`
**Location**: `Assets/Scripts/UI/Course/CourseButton.cs`

Course selection system with:
- Course preview images
- Difficulty indicators
- Best time tracking
- Course unlock progression

**Unity Setup Required**:
- Create CourseSelection scene
- Create CourseButton prefab with UI elements
- Create CourseData ScriptableObjects for each track
- Set up course preview UI layout

### 5. Player2SelectionManager
**Location**: `Assets/Scripts/UI/Character/Player2SelectionManager.cs`

Split-screen Player 2 character selection:
- Separate character selection for Player 2
- Character comparison display
- Only appears in 2-player mode

**Unity Setup Required**:
- Add Player2 selection panel to CharacterSelect scene
- Set up second character carousel or selection UI
- Configure character comparison displays

## Scene Flow

```
MainMenu → CharacterSelect → [Player2Select] → CourseSelection → Race
    ↑                                                              ↓
    ←--------------------- Quick Start ------------------------------
```

## Required Unity Setup Steps

### 1. Create Scenes
Create these scenes in your project:
- **MainMenu.unity** - Entry point with MainMenuManager
- **CharacterSelect.unity** - Enhanced character selection (already exists, needs updates)
- **CourseSelection.unity** - New course selection scene

### 2. Create Prefabs

#### MainMenu UI Structure
```
MainMenu Canvas
├── Background Image
├── Title/Logo
├── Quick Play Panel
│   ├── Last Character Info
│   ├── Last Game Mode Info
│   └── Start Race Button
├── New Game Button
├── Settings Button
└── Exit Button
```

#### Character Selection UI Structure
```
CharacterSelect Canvas
├── Character3D Carousel (in scene, not UI)
├── Character Info Panel
│   ├── Portrait Image
│   ├── Name Text
│   ├── Description Text
│   └── Stats Sliders (Speed, Health, Jump)
├── Game Mode Dropdown
├── Navigation Buttons (Left/Right)
├── Continue Button
└── Back Button
```

#### Course Selection UI Structure
```
CourseSelection Canvas
├── Course Preview Image
├── Course Info Panel
│   ├── Course Name
│   ├── Difficulty Display
│   └── Description
├── Course Grid (using CourseButton prefabs)
├── Navigation Buttons
├── Start Race Button
└── Back Button
```

### 3. Create ScriptableObjects

#### Character Data
1. Right-click in Project → Create → TimeRiders → Character Data
2. Fill in character information
3. Assign portrait sprites
4. Set stats (Speed, Health, Jump)
5. Save to `Assets/Resources/Characters/` folder

#### Course Data
1. Right-click in Project → Create → TimeRiders → Course Data
2. Fill in course information
3. Assign preview images
4. Set difficulty and scene paths
5. Configure unlock requirements

### 4. Mobile Configuration

For mobile builds:
- Set `isMobileBuild = true` in MainMenuManager
- Set `isMobileBuild = true` in CharacterSelectionManager
- Enable swipe input in Character3DCarousel
- Disable 2-player split-screen options

### 5. Input Configuration

The system supports:
- **Mouse/Touch**: Click and drag for carousel rotation
- **Keyboard**: Arrow keys or WASD for navigation
- **UI Buttons**: Fallback navigation buttons

## Integration with Existing Systems

### GameManager Integration
```csharp
// In your GameManager, check for saved selections
if (SelectionData.LoadFromPlayerPrefs()) {
    // Use loaded selections
    var character = SelectionData.SelectedCharacter;
    var gameMode = SelectionData.SelectedGameMode;
    // etc.
}
```

### Race Results Integration
```csharp
// After race completion, save best times
var courseData = GetCurrentCourseData();
bool newRecord = courseData.SetBestTime(raceTime);
if (newRecord) {
    // Show new record UI
}

// Mark course as completed
courseData.MarkCompleted(starsEarned);
```

## Customization Options

### Character Carousel
- Adjust `radius` for carousel size
- Modify `rotationSpeed` for animation speed
- Configure `characterSpacing` for character positioning
- Enable/disable `autoRotateEnabled` for demo mode

### Course Selection
- Customize difficulty colors in `GetDifficultyColor()`
- Add course filtering options
- Implement course unlock logic
- Add course rating system

### Mobile Optimizations
- Touch sensitivity adjustments
- UI scaling for different screen sizes
- Platform-specific feature toggles

## Testing Checklist

- [ ] Main menu shows previous game data correctly
- [ ] Character carousel rotates smoothly
- [ ] Character stats display properly
- [ ] Game mode dropdown excludes 2-player on mobile
- [ ] Course selection shows previews and difficulty
- [ ] Player 2 selection appears only in split-screen mode
- [ ] Quick start loads previous configuration
- [ ] Scene transitions work correctly
- [ ] Data persistence works between sessions

## Performance Considerations

- Use object pooling for course buttons if you have many courses
- Optimize 3D character models for real-time rendering
- Consider LOD (Level of Detail) for character models
- Preload common UI assets
- Use compressed textures for course preview images

This system provides a complete, professional launch screen experience that can be fully customized through Unity's UI system without requiring code changes for basic modifications.
