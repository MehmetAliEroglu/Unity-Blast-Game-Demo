# Blast Game Mechanics Prototype
![Gameplay Demo](BlastGame.gif)

A scalable Match-2 puzzle game template focused on clean code principles and algorithm efficiency.

## 🚀 Key Features

### 🧠 Smart Algorithms
* **Deadlock Handling:** Automatically detects unplayable states and reshuffles the board intelligently using **Fisher-Yates**.
* **Flood Fill:** Uses **BFS** to detect matching color groups instantly.

### ⚡ Performance
* **Object Pooling:** Minimizes memory fragmentation by recycling block objects.
* **Optimization:** Logic and View layers are separated to ensure high FPS even on larger grids.

### 🛠️ Configurable Levels
* Levels are defined as `ScriptableObjects`.
* Supports dynamic grid sizes, color counts, and win conditions.
* **Test Included:** Try `Level_Big` in `Assets/Levels` to see the performance on large grids.

## Project Setup
* **Unity Version:** [6000.0.38f1]
* **Dependencies:** DOTween (Included)

## Folder Structure
* `Scripts/Managers`: Systems controlling the game flow.
* `Scripts/Gameplay`: Block logic and algorithms.
* `Assets/Levels`: Level configuration files.

---
*Created by Mehmet Ali Eroğlu*
