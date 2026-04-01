 

Game Design Document: 2D Snake Game 

1. Project Overview 

Project Title: 2D Snake Game. 

Developer Team: BSSE Team “SYED HABIB HUSSAIN ”,”ZAHID ULLAH”,”MUHAMMD HAROON KHAN ”,”ALI AFZAL”  

Target Platform: Windows / WebGL. 

Core Engine: Unity 6 (6000.4.0f1). 

2. Gameplay Mechanics 

This section defines how the game actually works so the Developer (Member 3) knows what to code. 

Movement: The snake moves in a continuous, grid-based fashion. Players use WASD or Arrow keys to change direction. 

Growth Logic: When the snake's head overlaps with a food item, a new segment is instantiated and attached to the tail. 

Collision Rules: * Wall Collision: Touching the boundary triggers a Game Over. 

Self-Collision: Touching any part of the snake's own body triggers a Game Over. 

Food Spawning: Food must appear at a random coordinate within the grid that is NOT currently occupied by the snake's body. 

3. Game Flow & UI (Wireframes) 

As the Team Lead, you need to ensure the Designer (Member 2) follows this flow: 

Main Menu: Display "Start Game," "High Score," and "Quit". 

Gameplay HUD: Display the current "Score" and "High Score" in real-time. 

Pause State: Freeze time (Time.timeScale = 0) and show a "Resume" or "Menu" option. 

Game Over Screen: Display the final score and a "Replay" button. 

4. Difficulty Curve 

To make the game professional, it shouldn't stay the same speed forever. 

Speed Scaling: Every 5 pieces of food eaten, increase the snake's movement speed by 10%. 

Level Obstacles: In later levels (Week 11), static blocks will be placed in the grid to increase difficulty. 

5. Technical Specifications 

Input System: Unity’s Input System (Standard). 

Data Persistence: Use PlayerPrefs to store the highest score achieved on the local machine. 

Resolution: 1920x1080 (16:9 Aspect Ratio) for standard Windows builds. 

 

 
