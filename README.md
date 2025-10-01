# Software-design-patterns-EX-2-

# 🚀 Mars Rover Simulation

This project is a **console-based simulation** of a Mars Rover navigating a grid-based terrain.  
The rover can move forward, turn left, and turn right, while avoiding obstacles and staying inside the grid boundaries.

---

## 📌 Problem Statement

- The rover starts at a given position `(x, y)` and direction (`N`, `S`, `E`, `W`).
- Commands:
  - `M` → Move forward
  - `L` → Turn left
  - `R` → Turn right
- The rover:
  - Must **not move outside** the grid
  - Must **stop when an obstacle is in the way**
- Status reports show the **current position, direction, and obstacle status**.

---

## 🛠 Features

- Pure **Object-Oriented Design**  
- Uses **Command Pattern** for commands (`M`, `L`, `R`)  
- No `if-else` chains → relies on **polymorphism**  
- Supports obstacle placement  
- Grid boundaries enforced  
- Final status report for rover’s position and direction  

