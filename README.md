# KARIOMART
Andreas Zweifel C# Unity Assignment: KarioMart

## Description
This KarioMart project of mine was a bit of a dumpster fire at times since this last week I've been sick and have had a hard time working. My main focus was a bit outside the scope of the project seeing as I've spent a lot of time crafting a sort of "Level Editor", I wanted to automate the process of creating a road with a mesh and colliders. Most of the other time was spent putting together the car mechanics using the new input system and Unity Physics. Physics and movement overall has never been my strong suit so I've encountered a boat load of problems, the final product sort of feels like driving on ice, but hey! If I create a ice-looking material that could be the (very bad) hook for the game. Northern Sweden Racing Simulator!

The Game utilizes Tank controls (WASD/Arrow Keys - Forward, Steering, Reversing).

DISCLAIMER: I'm no Game Designer, I tried playing around with all the values on the players and came quite close to something nice but then I HAD to go fiddle around with the values and mess everything up without writing down the good ones ughhh

## Getting Started
The game should work out of the box, boot it up in Unity 2022.3.10f1, hop onto the MainMenu Scene and start from there!

## Project Structure, Code Structure and Design
In the "ARCADE - FREE Racing Car" folder there are a collection of free assets courtesy of "Mena" on the Unity Asset Store (https://assetstore.unity.com/packages/3d/vehicles/land/arcade-free-racing-car-161085)

In the "Input" folder, you can find the inputactions files

Materials are found in the aptly named "Materials" folder

Physics Materials are found in the aptly named "PhysicsMaterials" folder

Prefabs are found in the aptly named "Prefabs" folder

Scenes are found in the aptly named "Scenes" folder

Scripts are found in the aptly named "Scripts" folder

I've made use of the Singleton pattern throughout this project. I made the GameManager script into a Singleton, ensuring there's only one instance of this manager in the game. I use it to manage player progress, initialization and scene loading.

I've made use of a number of events in the game, for example the "OnCheckpointReached" event in the Checkpoint script to notify other parts of the game that the player has in fact reached a checkpoint! I use this to keep track of which player passes through which checkpoint and make sure they go through the stage sequentially and don't just back up behind the finish line and drive through.

I also wanted to play around with using a Finite State Machine for the PlayerController. The player has three states that change depending on player input: Idle, Accelerating, and Reversing. I've never really played around with FSMs (consciously at least), so I saw it fit to include it here as I want to have a distinct set of physics instructions depending on the state of the player car.

As previously mentioned, I also played around with Procedural Generation a ton. The SplineMeshCreator script generates road segments dynamically based on a set of points on a spline, and some other values in the inspector (width and resolution). The barebones basics were based on a video by Game Dev Guide on YouTube (https://www.youtube.com/watch?v=ZiHH_BvjoGk), but then I added other functionality such as my own editor script to control the actual editing of the road mesh as well as procedurally generating box colliders along the inner and outer edges of the road using the "extruded" points from the spline which I know make up the edge verts of the road mesh.

I tried to make use of prefabs where possible in my scuffed project setup (would be much nicer and cleaner and better if I had this last week to happily and healthily set it all up)

I opted to set up the entire Game session inside one single scene instead of loading between different scenes by loading different "Course" prefabs that contain the road mesh, colliders, checkpoints, the goal, and spawnpoints

I missed the last two lectures of the course since I was resting in bed at the time, but I will watch them and apply a bit of that knowledge on future projects and update this current one if time permits.

## Notable files

PlayerController.cs:
  -Manages player character control
  -Utilizes a Finite State Machine
  -Implements input actions

GameManager.cs;
  -Handles game-wide functionality
  -Utilizes the Singleton pattern
  -Manages player progress and victory conditions

Checkpoint.cs:
  -Handles checkpoints for player progress
  -Utilizes events and actions to signal other scripts
  -Detects player collisions

SplineMeshCreator.cs:
  -Procedurally generates road segments
  -Utilizes the Unity Spline tool
  -Handles the creation of road colliders

Course.cs:
  -Manages courses and checkpoint progress
  -Uses events and actions to respond to checkpoint progress
  -Keeps track of player progress and triggers victory conditions
  
## How to Play
Player 1
  WASD - Forward / Steer / Reverse

Player 2
  Arrow Keys - Forward / Steer / Reverse

Goal: Drive through the course, get to the finish line, repeat 3 times, ???, Profit!

## Inspirations
Main source of inspiration was the video by Game Dev Guide on YouTube (https://www.youtube.com/watch?v=ZiHH_BvjoGk), other than that? Mostly previous experience of earlier projects and a bunch of Googling.

## Unity Version
2022.3.10f1
