# Time Riders - 3D Web-Based Motorbike Racing Game

A Unity 6.1 web-based motorbike racing game featuring accessibility options, character selection, and Australian-themed gameplay.

## 🎮 Game Features

- **2-Player Support**: Split-screen or turn-based racing
- **7 Unique Characters**: Choose from Olivia Rodrigo, P!nk, Taylor Swift, Billie Eilish, Lizzo, Claire, and Maddy
- **Accessible Controls**: 
  - WASD keys (Player 1)
  - IJKL keys (Player 2)  
  - Mouse control (Y-axis: throttle, X-axis: steering)
  - Gamepad support
- **Dynamic Terrain**: Bitumen, gravel, dirt, and off-road surfaces
- **Obstacles**: Spinners, saws, tape measures, and Allen keys
- **Australian Wildlife NPCs**: Kangaroos, dogs, possums, and chameleons
- **KTM Motorbike Physics**: Realistic bike handling and jumping
- **Leaderboard System**: Top 10 scores stored in browser
- **Prize System**: Confetti effect with AUD dollar notes

## 🛠️ Setup Instructions

### Prerequisites
- Unity 6.1 or later
- WebGL build support

### Installation
1. Clone this repository
2. Open the project in Unity 6.1
3. Import required packages:
   - TextMeshPro
   - Unity Input System (optional)
4. Open the Main scene
5. Build and Run

### Building for Web
1. Go to File > Build and Run
2. The game will be available at http://localhost:53353

## 🎯 Gameplay

### Objective
Complete the circuit in the fastest time while avoiding obstacles and NPCs.

### Controls
- **WASD**: Player 1 movement
- **IJKL**: Player 2 movement  
- **Space/Right Shift**: Jump
- **Mouse**: Alternative steering (hold left click)
- **Escape**: Pause/Menu

### Terrain Effects
- **Bitumen**: Full speed
- **Gravel**: 80% speed
- **Dirt**: 60% speed
- **Off-road**: 40% speed (allows shortcuts)

### Health System
- Start with 100% health
- Collisions with obstacles: -20 health
- Collisions with animals: -15 health
- Health regenerates at checkpoints

## 📁 Project Structure

```
Assets/
├── Scripts/           # Core game logic
├── Prefabs/          # Game objects and characters
├── Materials/        # Terrain and visual materials
├── Audio/           # Music and sound effects
├── UI/              # User interface prefabs
└── Scenes/          # Game scenes
```

## 🎨 Character Abilities

- **Olivia Rodrigo**: Speed bonus (+5)
- **P!nk**: Health bonus (+20), Jump bonus (+10)
- **Taylor Swift**: Balanced stats (+3 all)
- **Billie Eilish**: Stealth bonus (+2 speed, +8 jump)
- **Lizzo**: Durability (+30 health, -2 speed)
- **Claire**: Acrobatic (+15 jump, +8 speed, -5 health)
- **Maddy**: All-rounder (+5 all stats)

## 🔧 Customisation

### Adding New Characters
1. Create CharacterData ScriptableObject
2. Add character portrait and model
3. Configure stats and abilities
4. Add to character selection UI

### Creating New Obstacles
1. Create prefab with Obstacle script
2. Set obstacle type and damage values
3. Add appropriate collision detection
4. Place in track layout

### Terrain Modification
1. Use TerrainChecker script on surfaces
2. Set terrain type for speed modifiers
3. Apply appropriate materials and colours

## 🎵 Audio Requirements

Replace placeholder audio files with:
- Background music (royalty-free)
- Engine/motorbike sounds
- Crash and collision effects
- UI interaction sounds

## 📱 Platform Support

- **Primary**: Desktop web browsers
- **Secondary**: Mobile browsers (responsive UI)
- **Tested**: Chrome, Firefox, Safari, Edge

## 🏁 Project Story & Philosophy

Time Riders is a Unity-based bike racing game designed as a learning project for pragmatic, beginner-friendly software engineering. The codebase is structured for clarity, maintainability, and AI-assisted workflows, making it ideal for both new developers and collaborative environments (including Copilot, Cursor, and Cascade AI).

### Key Principles
- **SOLID**: Classes have a single responsibility, open for extension, and easy to test.
- **YAGNI**: Only use interfaces when multiple implementations are likely.
- **Brace Style**: Opening braces are always on the same line as method signatures and control statements.
- **Beginner-Friendly**: Code, comments, and docs are written for clarity and learning.
- **Storytelling**: Design docs, README, comments, naming, logs, and tests all tell the project's story.

## 🗂️ Folder Structure & Script Organization

```
Assets/
├── Scripts/
│   ├── Core/           # GameManager, Logger, EventSystem, etc.
│   ├── Player/         # PlayerController, CameraController, Character scripts
│   ├── UI/             # UI setup, HUD, QuickUISetup
│   ├── Environment/    # Terrain, obstacles, SceneSetup
│   ├── Setup/          # SceneSetup, QuickPlayerSetup, initial setup scripts
├── Config/             # ScriptableObject configs (e.g., BikeConfig)
└── Tests/              # Unit and integration tests
```

- See `unity-style-guide.md`, `.instructions.md`, and `refactor_tasks.md` for details on coding standards and ongoing improvements.

## 🤖 AI-Assisted Development

- The project is optimized for use with GitHub Copilot, Cursor, and Cascade AI.
- Follow the instructions in `.github/copilot-instructions.md` and `cursor-style-guide.md` for best practices.
- Refactoring tasks and progress are tracked in `refactor_tasks.md`.

## 📝 Logging, Config, and Testing

- Use `GameLogger` (Core/) for categorized, level-based logging.
- Store configuration data in ScriptableObjects (see `Config/BikeConfig.cs`).
- Add unit and play mode tests for all core gameplay logic.

## 🚀 Deployment

The game builds to WebGL for web deployment. Host on any web server that supports:
- HTML5
- WebAssembly (WASM)
- Browser local storage (for leaderboards)

## 🐛 Known Issues

- Audio may require user interaction to start (browser policy)
- Mobile performance may vary
- WebGL builds are larger than native builds

## 📄 License

This project uses open-source assets and follows fair use guidelines for character likenesses.

## 🤝 Contributing

1. Fork the repository
2. Create feature branch
3. Implement changes
4. Test thoroughly
5. Submit pull request

## 📞 Support

For issues and questions, please create a GitHub issue or contact the development team.