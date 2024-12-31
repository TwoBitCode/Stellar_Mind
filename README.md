# 🚀 Stellar Mind: A Space Adventure for Cognitive Growth 🌌
I added a detailed explenation for Assignment4 at the end.

itch game link - Assignment 6: https://twobitcode.itch.io/stellar-mind-with-story

itch game link - Assignment 4: https://twobitcode.itch.io/steller-mind


**Stellar Mind** is an engaging, space-themed educational game designed to help children with ADHD improve their working memory and executive functions. Through interactive missions, captivating narratives, and strategic challenges, children develop practical skills while embarking on an interstellar journey to save the galaxy! 🌟
![download (1)](https://github.com/user-attachments/assets/cf21a9cf-fbb7-49ae-9f23-6ad5ebd2b307)

---

## 🎯 Game Objective

Stellar Mind's primary goal is to strengthen working memory while teaching kids effective strategies to overcome cognitive challenges. By combining fun gameplay with therapeutic elements, the game provides children with tools to navigate real-life situations.  

---

## 🌟 Key Features

### 🚀 **Space Exploration**
- Explore planets, space stations, and galaxies while completing unique cognitive missions.
- Each level presents new challenges tailored to improve memory, attention, and planning.

### 🧠 **Cognitive Training**
- Develop working memory with tasks like:
  - Remembering and replicating alien signals. 👾
  - Organizing interstellar resources like floating tubes and space tools. 🛠️
  - Planning and sequencing missions to repair ships and stations. 🛸

### 🛠️ **Practical Strategies**
- Introduces strategies such as:
  - Breaking tasks into smaller steps. 🗂️
  - Using visual and auditory cues for memory support. 🎧👀
  - Encouraging self-checking and reflection. ✅

### 🌌 **Therapeutic Fun**
- A vibrant, child-friendly design with interactive characters like friendly aliens and robots. 🤖  
- A rewarding system with stars, badges, and exciting upgrades for motivation. ✨🏆  

---

## 🎮 Gameplay Overview

1. **Welcome Screen 🌟**:  
   Players are greeted with a galactic theme, an inspiring mission brief, and simple navigation options.  

2. **Unique Missions 🔧**:
   - **Decoding Alien Signals**: Memorize a sequence of shapes (🔵🔺➖) and replicate them.  
   - **Repairing Space Systems**: Reconnect cables and fix broken systems based on instructions.  
   - **Navigating the Galaxy**: Remember the sequence of planets and chart the correct path.  

3. **Progress Tracking 📈**:  
   - Players receive feedback after each mission to understand their strengths and areas to improve.  
   - Unlock new planets and upgrades by mastering levels.  

---

## 🔧 How to Play

1. **Launch the Game 🚀**: Start your adventure and choose your mission.  
2. **Follow Instructions 📝**: Pay attention to the mission brief and memorize key details.  
3. **Complete Tasks ✅**: Solve puzzles, remember sequences, and navigate challenges.  
4. **Earn Rewards 🌟**: Collect stars and power-ups to advance to new levels.  

---

## 🛠️ Built With

- **Unity** 🎮: For immersive and interactive game development.  
- **C#** 💻: To implement smooth gameplay mechanics.  
- **TextMeshPro** 🖋️: For engaging and accessible in-game text.  

---

## 👥 Team Members

### **Lead Developers**  
- **Vivian Umansky** & **Miriam Nagar**  
  - Roles: Coding, design, and implementation.

### **Occupational Therapy Partners**  
- **Morag Granot**, **Nirit**, **Yael Godman**  
  - Roles: Therapeutic insights, defining task-based challenges, adapting game mechanics to support children with ADHD.

---

## 🌟 Why Choose Stellar Mind?

- **Engaging Gameplay**: A space-themed adventure that kids will love! 🚀  
- **Therapeutic Benefits**: Helps children with ADHD build crucial cognitive skills. 🧠  
- **Customizable Difficulty**: Adaptive challenges that grow with the player's abilities. 🎮  

---


# Assignment 4 explenation
The Game is built from a few mini games, for Assignment4 we implemented 2 mini games connected to improving memory and consentration.

## The 2 mini games are:
## 1. Memory Game - Capsule Shuffle 🎮
This is a memory game where capsules of different colors are shuffled, and the player must rearrange them in the correct order. The player can drag and drop capsules in a grid, and the game provides the option to check the answer, restart the game, and prevent further actions until a restart is triggered.

Features 🌟
- **Capsule Shuffle**: Randomly shuffle capsules and move them to a stack after a set delay.
- **Drag and Drop**: Allows players to drag and drop capsules into the correct order.
- **Answer Check**: The game checks if the capsules are in the correct order and displays a message.
- **Restart Option**: The game can be restarted with new colors and random positions.
- **Freeze on Failure**: After a failure, capsules are frozen and cannot be moved until the restart button is pressed.
- **Dynamic Capsule Count**: The number of capsules is set dynamically based on a variable (`numObjects`).

---

---

## Scripts Breakdown 📝

### `GameManager`

- Handles the main logic of the game including capsule shuffling, checking answers, and restarting.
- Controls when the game is frozen (capsules can't be moved after a failure).
- Regenerates new capsules and positions on restart.

### `DraggableItem`

- Enables the drag-and-drop functionality for the capsules.
- Handles the movement of capsules within the grid and stack.

### `Slot`

- Manages the interaction when a capsule is dropped into a grid slot.
- Ensures only one capsule is placed in each slot at a time.

---

## Gameplay Logic 🧠

1. **Shuffling**:
   - Capsules are shuffled randomly after a delay and moved to the stack.
   
2. **Freezing Capsules**:
   - If the player fails (incorrect order), the capsules are frozen (dragging disabled) until the restart button is clicked.
   
3. **Check Answer**:
   - If the capsules are in the correct order, the game will display a "Correct Order!" message.
   - If the order is wrong, the game shows "Incorrect Order! Try Again!" and the restart button will appear.

4. **Restarting the Game**:
   - The game will reset with new colors and random positions for the capsules.





## 2. Sorting Game - Asteroid Sorting Challenge 🎮🪐
This is a sorting game where asteroids of different colors are randomly assigned to the left or right side. The player needs to drag and drop them into the correct area based on their assigned color. The game features a countdown timer, random item assignments, and a scoring system. Upon game completion, the final score is displayed in the game-over scene.

---

## Features 🌟

- **Random Item Assignment** 🔄: Asteroids are randomly assigned to either the left or right area at the start of the game.
- **Drag and Drop** 🖱️: Players can drag and drop asteroids into the correct area (left or right) based on their color.
- **Game Timer** ⏳: The game includes a timer that starts after the instructions are displayed.
- **Scoring System** 💰: The player earns points for correctly sorting asteroids.
- **Final Score Display** 🏆: The final score is displayed in the game-over scene after the timer runs out.
- **Instructions** 📋: The game shows instructions about which color goes in which area before starting.

---

## How to Play 🕹️

1. **Game Start**: When the game begins, the player sees instructions about which color goes in which area (left or right).
2. **Random Assignment**: Asteroids are randomly assigned a color and a side (left or right).
3. **Rearrange Asteroids**: The player drags and drops the asteroids into the correct area.
4. **Timer Countdown**: The timer starts after the instructions are displayed and continues until time runs out.
5. **Check Answer**: At the end of the timer, the score is calculated based on the number of correctly placed asteroids.
6. **Game Over**: After the game ends, the final score is displayed in the game-over scene.

---

## Scripts Breakdown 📝

### `GameTimer` 🕰️

- **Function**: Manages the countdown timer, starting when the game begins and stopping when the timer reaches zero.
- **Main Tasks**: Updates the timer text, triggers the end of the game, and loads the game-over scene.

### `SortingGameManager` 🛠️

- **Function**: Controls the overall gameplay, including random assignment of asteroid colors, spawning asteroids, and managing the game’s start and end.
- **Main Tasks**: Handles game state, starts the timer, and manages asteroid spawning and placement.

### `SortingDraggableItem` 🛸

- **Function**: Manages the draggable functionality of asteroids.
- **Main Tasks**: Ensures asteroids are dragged and dropped in the correct area based on their randomly assigned color.

### `ScoreManager` 💰

- **Function**: Tracks and updates the score based on correct asteroid placements.
- **Main Tasks**: Adds points for correct placements and updates the UI with the current score.

---

## Gameplay Logic 🧠

1. **Random Assignment**:
   - Asteroids are randomly assigned a color (either "blue" or "yellow") and a side (left or right).
   
2. **Drag and Drop**:
   - The player drags an asteroid to the correct area based on the color assigned.
   
3. **Timer**:
   - The game includes a timer that starts after the instructions and counts down until time runs out.
   
4. **Scoring**:
   - Points are awarded for correctly placed asteroids.
   
5. **Game Over**:
   - When the timer reaches zero, the game ends, and the final score is displayed in the game-over scene.

---

## How to Customize ✨

- **Timer Duration**: Adjust the `gameDuration` variable in the `GameTimer` script to change the game’s time limit.
- **Asteroid Colors**: Modify the random color generation logic in the `SortingDraggableItem` script to customize asteroid colors.
- **UI Elements**: Customize the look of the UI, including the timer text, instructions, and buttons.

- 
# UML Diagram for Project

```plaintext
+------------------------+         +--------------------+         +--------------------+
|      GameManager      |         |     ScoreManager    |         |   GameFlowManager  |
|------------------------|         |--------------------|         |--------------------|
| - gridManager         |         | - currentScore     |         | - goalScore        |
| - stackManager        |         | - scoreText        |         | - sceneTransition  |
|------------------------|         |--------------------|         |--------------------|
| + InitializeGame()    |         | + AddScore()       |         | + HandleTransition()|
| + CheckOrder()        |         | + ResetScore()     |         |--------------------|
+------------------------+         +--------------------+         +--------------------+
         |                             |                               |
         |                             |                               |
 +-------+------+             +--------+--------+             +-------+--------+
 |  GridManager  |             |   UIManager    |             | SceneTransition |
 |---------------|             |----------------|             |-----------------|
 | - slots       |             | - timerText    |             | - nextSceneName |
 |---------------|             | - scoreDisplay |             |-----------------|
 | + SetupGrid() |             |----------------|             | + LoadScene()   |
 | + ResetGrid() |             | + UpdateTimer()|             +-----------------+
 +---------------+             | + UpdateScore()|
                                +----------------+

+-----------------------+         +----------------------+         +--------------------+
|     StackManager      |         |     DraggableItem    |         |   DialogueSetup    |
|-----------------------|         |----------------------|         |--------------------|
| - stackItems          |         | - itemType           |         | - dialogueSequence |
|-----------------------|         |----------------------|         |--------------------|
| + AddToStack()        |         | + OnDrag()           |         | + Initialize()     |
| + CheckOrder()        |         | + OnDrop()           |         |--------------------|
+-----------------------+         +----------------------+         +--------------------+

+----------------------+         +----------------------+         +-----------------------+
| CharacterSelection  |         | PlayerDataManager    |         | SortingDraggableItem |
|----------------------|         |----------------------|         |-----------------------|
| - selectedCharacter |         | - playerName         |         | - pointsForCorrect   |
|----------------------|         | - targetScore        |         | - leftAreaName       |
| + SelectCharacter() |         |----------------------|         | - rightAreaName      |
| + ConfirmSelection()|         | + SaveData()         |         |-----------------------|
+----------------------+         | + LoadData()         |         | + HandleCorrect()    |
                                +----------------------+         | + HandleIncorrect()  |
                                                                +-----------------------+
```

### Explanation of UML Diagram:

1. **Game Management**:
   - `GameManager`: Handles memory game flow, including grid setup and order checking.
   - `GameFlowManager`: Handles scene transitions based on scores and conditions.
   - `ScoreManager`: Tracks and updates the player's score dynamically.

2. **UI Management**:
   - `UIManager`: Updates and manages UI elements like timers and scores.

3. **Gameplay Mechanics**:
   - `GridManager`: Manages the game grid, resetting and setting it up.
   - `StackManager`: Handles stack-based gameplay and checks item order.
   - `DraggableItem`: Implements generic drag-and-drop functionality.
   - `SortingDraggableItem`: Extends `DraggableItem` to implement sorting-specific behavior.

4. **Dialogue Management**:
   - `DialogueSetup`: Prepares and coordinates dialogue sequences.

5. **Data Management**:
   - `PlayerDataManager`: Saves and loads player-specific data, such as name and target score.

6. **Other Features**:
   - `CharacterSelection`: Allows players to choose a character.
   - `SceneTransition`: Manages transitions between scenes.


