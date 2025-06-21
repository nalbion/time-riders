# Character Model System Documentation

This document explains how the character model swapping system works in Time Riders, allowing different characters to use different 3D models while maintaining the same motorcycle physics.

## 🎯 System Overview

The character model system consists of three main components:

1. **CharacterData** - Stores character information including the 3D model reference
2. **CharacterModelSwapper** - Handles dynamic model swapping at runtime
3. **MotorcycleWRider Prefab** - The motorcycle prefab that uses swappable character models

## 📁 File Structure

```
Assets/
├── Scripts/
│   ├── UI/Character/
│   │   └── CharacterData.cs                    # Character data with model field
│   ├── Gameplay/
│   │   └── CharacterModelSwapper.cs           # Runtime model swapping
│   └── Editor/
│       ├── CharacterDataCreator.cs            # Creates character assets
│       └── MotorcycleSetupHelper.cs           # Sets up prefab
├── Resources/Characters/
│   ├── [CharacterName].asset                  # Character data assets
│   └── CHARACTER_MODEL_SYSTEM.md             # This documentation
└── SimpleMotorcyclePhysics/
    ├── Models/
    │   └── Rider.fbx                          # Default rider model
    └── Prefabs/
        └── MotorcycleWRider.prefab            # Motorcycle with rider
```

## 🔧 Setup Instructions

### 1. Initial Setup

**Add Character Model Swapper to Motorcycle:**
```
Unity Menu > TimeRiders > Setup Motorcycle > Add Character Model Swapper
```

This will:
- Add `CharacterModelSwapper` component to `MotorcycleWRider.prefab`
- Automatically detect and assign the current rider model
- Set the default rider model reference

### 2. Create/Update Character Data

**Create all characters with default models:**
```
Unity Menu > TimeRiders > Create Character Data > Create All Characters
```

This will:
- Create CharacterData assets for all 7 characters
- Assign the default `Rider.fbx` model to each character
- Save assets in `Assets/Resources/Characters/`

### 3. Assign Custom Models (Optional)

For each character asset in `Assets/Resources/Characters/`:
1. Select the character asset in the Project window
2. In the Inspector, find the "3D Model" section
3. Assign a custom character model prefab or GameObject
4. If no custom model is assigned, the default rider model will be used

## 🎮 How It Works

### Runtime Model Swapping

1. **Game Start**: `CharacterModelSwapper.Start()` is called
2. **Character Detection**: System checks for selected character from:
   - `SelectionData.Instance.selectedCharacter` (primary)
   - `PlayerPrefs.GetString("SelectedCharacter")` (fallback)
3. **Model Loading**: Loads the character's assigned model or default model
4. **Model Swap**: Replaces the current rider model with the character's model

### Character Selection Flow

```
MainMenu → CharacterSelect → CourseSelection → Race Scene
                ↓
        SelectionData.selectedCharacter
                ↓
        CharacterModelSwapper detects selection
                ↓
        Swaps to character's 3D model
```

## 🛠 Component Details

### CharacterData Fields

```csharp
[Header("3D Model")]
public GameObject characterModel;  // Character's 3D model prefab
```

### CharacterModelSwapper Methods

```csharp
// Initialize model based on current selection
public void InitializeCharacterModel()

// Swap to specific character's model
public void SwapToCharacterModel(CharacterData characterData)

// Use default rider model
public void SwapToDefaultModel()

// Get current character
public CharacterData GetCurrentCharacter()

// Force refresh (useful for testing)
public void RefreshModel()
```

## 📋 Character Model Assignments

| Character | Current Model | Custom Model Status |
|-----------|---------------|-------------------|
| **Olivia Rodrigo** | Rider.fbx | Ready for custom model |
| **P!nk** | Rider.fbx | Ready for custom model |
| **Taylor Swift** | Rider.fbx | Ready for custom model |
| **Billie Eilish** | Rider.fbx | Ready for custom model |
| **Lizzo** | Rider.fbx | Ready for custom model |
| **Claire** | Rider.fbx | Ready for custom model |
| **Maddy** | Rider.fbx | Ready for custom model |

## 🎨 Adding Custom Character Models

### Step 1: Prepare Your Model
1. Import your character model (FBX, OBJ, etc.) into Unity
2. Ensure the model is properly rigged and animated (if needed)
3. Set up materials and textures
4. Create a prefab from the model (optional but recommended)

### Step 2: Assign to Character
1. Open the character's asset file (e.g., `OliviaRodrigo.asset`)
2. In the Inspector, drag your model to the "Character Model" field
3. Save the asset

### Step 3: Test
1. Start the game
2. Select the character in the character selection screen
3. Enter a race scene
4. Verify the custom model appears on the motorcycle

## 🔍 Troubleshooting

### Model Not Appearing
- **Check Selection**: Ensure character is properly selected in `SelectionData`
- **Check Assignment**: Verify the model is assigned in the CharacterData asset
- **Check Console**: Look for error messages from `CharacterModelSwapper`

### Model Position/Scale Issues
- **Transform Reset**: The swapper resets position, rotation, and scale to match parent
- **Model Pivot**: Ensure your character model has the correct pivot point
- **Parent Hierarchy**: Check that `riderParent` is correctly assigned

### Performance Issues
- **Model Complexity**: High-poly models may impact performance
- **Texture Size**: Large textures can affect memory usage
- **LOD System**: Consider implementing Level of Detail for complex models

## 🧪 Testing Commands

### Editor Menu Commands
```
TimeRiders > Setup Motorcycle > Show Prefab Info    # Debug prefab hierarchy
TimeRiders > Setup Motorcycle > Add Character Model Swapper  # Add component
```

### Runtime Context Menu
Right-click on `CharacterModelSwapper` component:
```
Refresh Character Model  # Force model refresh
```

### Debug Methods
```csharp
// In CharacterModelSwapper
GetCurrentCharacter()     // Get active character
RefreshModel()           // Force model update
```

## 📝 Notes

- **Default Fallback**: If no character is selected or model is missing, the default `Rider.fbx` is used
- **Memory Management**: Old models are properly destroyed when swapping to prevent memory leaks
- **Editor Safe**: The system works in both Play mode and Edit mode
- **Prefab Friendly**: All changes are applied to the prefab, not just instances

## 🔮 Future Enhancements

- **Animation Support**: Add character-specific animations
- **Customization Options**: Allow runtime model customization (colors, accessories)
- **Model Variants**: Support multiple model variants per character
- **Performance Optimization**: Implement model pooling for frequent swaps
