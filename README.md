# 🍉 Laser Fruit Ninja (Touchless Arcade)

A touchless, VR-style arcade game prototype built in Unity using **Google MediaPipe** for real-time Hand-Tracking. Experience a classic fruit-slashing style game without ever touching your mouse, keyboard, or screen!

## ✨ Features

- **✋ Touchless Controls (Computer Vision):** Uses your laptop's webcam and Google MediaPipe to track your hand's 21-point landmarks in real-time. No VR headset or physical controllers required!
- **🔫 Laser Aiming:** Point your index finger at the screen to control a futuristic laser beam. The laser automatically blasts fruits upon contact.
- **✊ Fist Gesture UI:** Navigate the Main Menu by pointing your finger and closing your hand into a **Fist** to "Click" buttons. 
- **📈 Dynamic Progression:** The game gets progressively harder the longer you survive. Fruits and bombs will spawn at an incredibly fast rate as time goes on.
- **💥 Juicy Game Feel:** Experience heavy impacts with *Hit-Stop* (time freeze), violent *Camera Shakes*, and dynamic *Laser Pulses* whenever you destroy a target.
- **💣 Risk & Reward:** Blasting fruits grants points, but hitting bombs reduces your HP and triggers a red screen flash. Survive as long as you can!

## 🛠️ Requirements & Tech Stack

- **Unity Engine**
- **MediaPipe Unity Plugin:** https://github.com/homuler/MediaPipeUnityPlugin Used for local machine-learning hand-tracking processing.
- **Webcam:** A standard webcam is required to track your hand movements.

## 🎮 How to Play

### Main Menu
1. Raise your hand in front of the webcam.
2. Point your **Index Finger** at the UI buttons (Start / Exit).
3. To click a button, make a **Fist** (curl your middle, ring, and pinky fingers tightly towards your palm while aiming).

### In-Game
1. Point your **Index Finger** to aim the laser.
2. Hover the laser over the falling **Fruits** to blast them into pieces and gain points.
3. Avoid aiming at the **Bombs**! Hitting them will deduct 25 HP.
4. The game ends when your HP reaches 0.

## 📂 Key Scripts Structure

- `Assets/Scripts/FruitNinja/`
  - `LaserGun.cs`: Handles aiming logic, laser visuals, and auto-shooting mechanics in-game.
  - `MenuLaserPointer.cs`: Handles UI raycasting and Fist-Gesture detection for the Main Menu.
  - `Fruit.cs`: Manages the state of fruits/bombs, point values, and explosion physics.
  - `FruitSpawner.cs`: Manages the dynamic difficulty curve and randomized spawning logic.
  - `GameManager.cs`: Singleton that handles scoring, HP, screen shake, hit-stop, UI Overlays, and game over states.

## 🚀 Setup Instructions

1. Clone this repository.
2. Open the project in Unity.
3. Add `Scenes/MainMenu` and `Scenes/Play` to your **Build Settings**.
4. Press **Play** in the Unity Editor starting from the `MainMenu` scene.
5. Ensure your webcam is active and your room is well-lit for optimal hand tracking.
