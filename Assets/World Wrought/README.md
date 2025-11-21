# LifeSim Unity Project Skeleton

This repository contains a skeleton Unity project for a fully original sandbox life‑simulation game.  It includes the core C# scripts for needs and mood systems, characters, dynastic families, towns, world generation and loading screens.  Use it as a starting point for building the deep AI and procedural world described in the design document.

## Project structure

```
project_life_sim/
  Assets/
    Scripts/          # All C# scripts for gameplay systems
    LoadingScreens/   # Example images for loading screens
    Scenes/           # Create your scenes here (MainMenu, Game)
    Prefabs/          # Prefabs for characters, towns, monsters
  README.md
```

## Getting started

1. **Open the project in Unity Hub**
   - Copy `project_life_sim` into your Unity projects folder.  In Unity Hub, choose **Add**, browse to the folder and open it with a recent Unity LTS version (e.g., 2022 LTS).

2. **Create a terrain**
   - In `Assets/Scenes`, create a new scene called `Game`.  Add a **Terrain** GameObject and attach the `WorldGenerator` script.  Press Play to generate a random landscape.

3. **Setup the player**
   - Create an empty GameObject called `Player`.  Add a **CharacterController** component and attach the `PlayerController` script.  Add a `Character` component and configure needs (e.g., Hunger, Rest).  Create a camera, attach the `CameraController` script and set its `target` to the `Player`.

4. **Add loading screens**
   - Create a UI Canvas with an `Image` and a `Slider` for the loading bar.  Attach the `LoadingScreenManager` script to an empty GameObject and assign the UI elements and loading screen sprites.  When changing scenes, call `LoadScene("Game")` instead of `SceneManager.LoadScene`.

5. **Experiment with AI**
   - Use the `TownAI` script on an empty GameObject to create a simple resource‑producing town.  Create villager GameObjects with the `Character` component and assign them to the `Villagers` list in `TownAI`.

6. **Extend the systems**
   - Add new needs, traits and skills by extending the enumerations and creating subclasses of `Trait`.  Implement your own `TownAI` and `Nemesis` systems based on the design document.

This is a minimal framework to get you started.  You are expected to build on top of it—create scenes, prefabs, art assets and deeper gameplay mechanics.  Refer to the design document for guidance on dynastic mechanics, belief systems, diplomacy and nemesis features.