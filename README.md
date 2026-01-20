# Inventory Mini-Game (PickITUP) (Unity)

This project is a **small gameplay prototype** developed as part of a take-home assignment.  
The primary focus is on **inventory design, interaction flow, and player UX**, rather than visual polish or art quality.

The game demonstrates how a player picks up items, manages inventory, equips weapons, uses ammo, and receives contextual UI guidance throughout the experience.

---

## 🎯 Focus Areas

- Inventory system (stackable & unique items)
- Weapon pickup, equip, drop flow
- Ammo management & firing logic
- Context-aware UX hints (no overlapping messages)
- Clear player guidance using minimal UI
- Robust interaction handling (edge cases covered)

> ⚠️ Visuals are intentionally minimal.  
> The UI is functional and designed only to support and demonstrate gameplay logic.

---

## 🕹️ Gameplay Overview

- Pick up **weapons** and **ammo** from the world
- Equip weapons automatically when picked up
- Fire bullets using the equipped weapon
- Drop weapons via inventory selection
- Ammo cannot be dropped (UX feedback provided)
- Contextual hints guide the player step-by-step

---

## 🎮 Controls

| Action | Key |
|------|----|
| Move | WASD |
| Look | Mouse |
| Pick up item | **E** |
| Fire weapon | **TAB** |
| Open / Close Inventory | **I** |
| Drop selected item | **Q** |
| Open / Close Help | **H** |

---

## 🧠 UX & Hint System

The game includes a **centralized Hint UI system** that:
- Shows **only one message at a time**
- Automatically hides messages after a short duration
- Displays contextual prompts such as:
  - “Press E to pick up”
  - “Select an item to drop”
  - “Ammo cannot be dropped”
  - “You already have a weapon”
  - “Pick ammo to use the gun”
  - “Pick a gun to use ammo”
  - “Press Fire to shoot” (shown only once)

---

## 📦 Inventory System Features

- Supports:
  - **Unique items** (weapons)
  - **Stackable items** (ammo)
- Ammo count is synchronized with the shooting system
- Inventory slots support:
  - Select
  - Deselect (click again)
  - Auto-deselect on inventory close
- Prevents invalid actions with proper UX feedback


---




## 🚀 How to Run

1. Clone the repository
2. Open the project in **Unity**
3. Load the main scene
4. Press **Play**



---

## 👤 Author

**Sujal Gupta**  
XR / Unity Developer  

---

## 📜 License

This project is shared for evaluation and learning purposes.
