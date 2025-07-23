🧼 **VertiClean – Therapeutic VR Cleaning Simulator**

A fully immersive Unity VR game designed to treat fear of heights through gradual exposure , built with modular sprints, tested thoroughly, and backed by automated CI/CD.

---

🕹️ **Game Overview**

VertiClean is a virtual reality window-cleaning simulator built in Unity (C#), designed to assist users in gradually overcoming acrophobia (fear of heights). The player is immersed in increasingly high-altitude environments while performing satisfying cleaning tasks – simulating real-world exposure therapy in a controlled, gamified format.

You begin by cleaning low heights surfaces and progress to rooftops of skyscrapers, with carefully designed visual, physical, and audio feedback.

---

🧠 **Gameplay Features**

- **Gradual Exposure Therapy**

  - Players are slowly introduced to higher and more complex environments in a structured and therapeutic progression.
  - Every new stage elevates difficulty both vertically and cognitively.

- **Stage-Based Progression**

  - Very low heights → mid-level balconies → high-rise suspended platforms
  - Each stage contains dirt targets to be cleaned using appropriate tools

- **Dynamic Tools**

  - Spray Bottle 💧
  - Sponge 🧽&#x20;
  - Mop 🧹

- **Platform Interaction**

  - Realistic cleaning platforms  that move, vibrate, and simulate height

- **Sensory Feedback**

  - Sound effects for cleaning, wind, lifting and walking
  - Haptic feedback (vibrations) from VR controllers
  - Visual progress indicators and clean-up feedback

- **Save System**

  - Save/Load/Delete profiles
  - Resume progress per user
  - Multiple slots

- **Statistical Tracking**

  - Time taken per stage
  - Minimun & Maximun session time per stage
  - Stage rating

---

🧰 **VR Integration**

- **Headset Support**

  - Developed with Meta Quest (via OpenXR/Oculus Integration)
  - Full positional tracking and immersive 3D sound

- **Controller Usage**

  - Realistic movement and cleaning actions via XR Toolkit
  - Haptic Feedback per tool (stronger vibration when scrubbing with advanced tools) and when lifting the platform

- **Packages Used:**

  - Unity.XR.Interaction.Toolkit
  - Unity.InputSystem
  - Oculus Integration
  - TextMeshPro for crisp VR UI

---

📦 **Architecture & Code Structure**

- **Modular Codebase**

  - Runtime scripts are separated from Tests
  - Uses asmdef files for clean compilation and CI

- **Main Script Categories:**

  - CleaningToolBase, SprayTool, SpongeTool, MopTool
  - Player, StageManager, GameManager, StatisticsManager
  - SoundManager, HapticManager, SaveManager

- **UI & UX**

  - SaveMenuUI, StageFeedbackUI, CleaningProgressUI
  - IntroManager, MenuManager

- **Design Patterns Used:**

  - Singletons for managers (e.g. GameManager, SoundManager)
  - ScriptableObjects for tool definitions (CleaningToolSO)
  - Modular Stage Management

---

🧪 **Testing Strategy**

Testing is at the heart of VertiClean's development.

1. ✅ **Unit Tests (UnitTests.cs)**

   - Individual component logic (e.g. cleaning targets, save data)
   - Independent validation of state changes, event triggers

2. **🔗 Integration Tests (IntegrationTests.cs)**

   - Cross-system flows:
     - Clean progress → UI update → stage transition
     - Save/Load logic with UI and GameManager
     - Sound + Haptic + Gameplay sync

3. **📉 Performance Test (Stage3PerformanceLogger.cs)**

   - Runtime logging of:
     - FPS
     - Garbage collection
     - Memory usage
     - Device profiling

All tests run automatically in CI and manually via Unity Test Runner.

---

🔄 **Agile Workflow (Sprint-Based)**

- Every sprint = separate GitHub branch
- Feature-focused development:
  - sprint-1: player,  cleaning tools & menu
  - sprint-2: stage manager & haptic manager
  - sprint-3: save system & replay stages system
  - sprint-4: statistics manager 
- CI/CD pipelines test and build every sprint
- Modular codebase that enables isolated testing and improvements

---

🚀 **CI/CD – Full DevOps Integration**

🛠 GitHub Actions Workflows:

1. **Test Phase**

   - Triggered on push or pull request
   - Executes PlayMode tests
   - Validates performance, logic, and game flow

2. **Build Phase**

   - Triggered only after successful tests
   - Builds standalone executable for Windows
   - Uploads artifacts for distribution/testing

🔐 **Secure:**

- Uses secrets for Unity credentials
- Unity activated via UNITY\_LICENSE key

🧪 **Workflow Sample:**

- uses: game-ci/unity-test-runner\@v4 with: testMode: PlayMode

- uses: game-ci/unity-builder\@v4
  with:
  targetPlatform: StandaloneWindows64

---

💡 **Running the Game**

**Clone the Repository:**

- git clone [https://github.com/Eladgel1/VertiClean](https://github.com/Eladgel1/VertiClean)

**Open in Unity:**

- Unity version: 6000.0.41f1
- Open the main menu scene (usually in Assets/Scenes/MainMenuScene.unity)

**Play with VR:**

- Connect Meta Quest via Link or AirLink
- Ensure Oculus app + OpenXR settings are configured

🧪 **Running the Tests**

From Unity Editor:

- Window > General > Test Runner
- Select PlayMode
- Run all tests or filter by type

Or via GitHub CI:

- Just push to your branch and let Actions do the rest
- Results appear under "Actions >  Tests Results"

📁 **Project Dependencies:**

- Input System: VR interactions, controller tracking
- TextMeshPro: In-VR crisp UI and text
- XR Toolkit: Headset + hands integration
- Oculus Integration: Optimized Meta Quest support
- Game-CI: GitHub-based Unity CI framework

📈 **Player Analytics:**

- Time spent every stage

- Minimun & Maximun session time per stage

- Final rating per stage

- Logged via StatisticsManager, displayed via MenuManager

🎯 **Project Goals:**

VertiClean isn’t just a game - it’s a therapeutic tool for treating fear through controlled exposure. The mission is:

"Use gamified VR to help people overcome acrophobia by experiencing gradual height challenges in a safe, guided, and rewarding environment."

🤝 **Team & Contributors:**

- 🎮 Elad Gelerenter - [https://github.com/Eladgel1](https://github.com/Eladgel1)
- 🎮 Ohad Ezra - [https://github.com/OhadEzra](https://github.com/OhadEzra)

📣 Final Words:

VertiClean is a full-stack, fully-automated, therapeutically-driven Unity VR project - and now, it’s yours to explore, test, improve, and use.

🔥 Ready to clean your fear of heights away?
