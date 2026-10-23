# Merge Grid - Test Task

Technical test assignment implemented for Mystery Tag.

## Features & Requirements
* **Grid & Mechanics:** 5*7 grid with smooth drag-and-drop item movement. Merging two identical items upgrades them to the next level.
* **Generator:** Generator button spawns a level 1 item into a random empty cell.
* **Progression & Timer:** Level goal is to reach the final item level. A gameplay timer starts upon the first successful merge and stops upon completion.
* **Save System:** Best completion times are saved between sessions and displayed next to the active timer during replays.
* **Responsive UI:** Fully adapted and tested for 9:16, 9:19.5, and 3:4 aspect ratios.

## Tech Stack & Architecture
* **Engine:** Unity 
* **Architecture:** Simplified MVP (Model-View-Presenter) with a custom Lightweight DI Container and Finite State Machine (FSM).
* **Tweening:** DOTween for smooth animations (spawns, merges, and UI transitions).
* **Data:** ScriptableObject-based level configuration system and JSON save service.

## How to Run
1. Open the project in Unity.
2. Navigate to `Assets/Scenes/MainScene`.
3. Press "Play" in the editor.
