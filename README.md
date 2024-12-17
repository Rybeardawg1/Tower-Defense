# Tower Defense Game 
## Overview:
  This project is a tower defense game built in Unity using C#. It challenges players to make strategic decisions to place towers effectively based on the enemy’s path. The game combines traditional tower defense mechanics with AI-based pathfinding for enemies and an educational twist where players solve math problems to earn points. Points can be used to build and upgrade towers, making it a fun and educational experience. 

## Key Features
### AI-Based Enemy Pathfinding: 
    Enemies navigate the grid using pathfinding algorithms to find the optimal path to the target.
### Math Problems Integration:
      Players solve math challenges during gameplay.
      Problems are divided into:
      - Easy: Rewards fewer points.
      - Medium: Rewards moderate points.
      - Hard: Rewards maximum points.
     Correct solutions help players earn points to place or upgrade towers.
### Dynamic Tower Placement and Firing:
    Towers are placed on valid zones (can be dragged and dropped) and fire projectiles at enemies in range.
    Projectiles deal damage based on the tower's attributes.
### Resource and Health Management:
    Balance (points) and health are dynamically updated in the UI.
    Buttons for towers are disabled when the player cannot afford a tower.
### Interactive UI:
    Math problems, balance, and health are displayed in real time.
    Buttons reflect availability based on the player's balance.

## Contribution by each Team Member:
### Ashwin Goel:
    ⁠⁠Kickstarted with working on the Placement of the Turrets based on probability. This includes
     adding the respective Turret prefabs as well.
    •⁠  ⁠⁠Built the end-to-end Educational Component of the game which is solving Mathematical problems with varied difficulty 
    and points being awarded. This panel can be hidden/shown through a button.
    •⁠  ⁠⁠Developed the core GameManager which handles the Health and Balance logic connected with all 
    the game components like buying towers, killing enemies, enemies staying alive, solving mathematical problems etc.
    •⁠  ⁠It also handles the flow of game - Winning/Losing and End/Restart.
    •⁠  ⁠⁠Fixed some issues around Firing, Collision with enemies, points/health updation etc.
    •⁠  ⁠⁠Worked on these UI Components - Restart Panel at the end, Health and Balance display, Maths problem solving panel.
    
    Files worked on - 
    (Completely)
    MathProblemData.cs
    MathProblemManager.cs
    GameManager.cs
    
    (Collaboratively)
    GridGeneration.cs
    Enemy.cs
    Assets used - https://assetstore.unity.com/packages/3d/environments/sci-fi/tower-defence-sci-fi-turret-free-246331

### Somya Bharti :
    - Tower Drag-and-Drop System - Players can drag and drop tower buttons from the UI onto the map. 
        Buttons reset to their original position if not successfully placed or if there’s an invalid placement.
      - Files:  TowerDragAndDrop.cs: Handles drag-and-drop mechanics and placement validation.
    - Resource Management - Each tower has an associated cost. The player's balance decreases when towers are placed,
        and buttons dynamically disable when the balance is insufficient.
      - Files: GameManager.cs: Manages balance updates and interacts with the UI.
               TowerDragAndDrop.cs: Checks the player's balance before allowing tower placement.
    - Projectile and Damage Mechanics - Towers fire projectiles when placed. Projectiles inherit a damage value based on
        the firing tower’s strength, determining their effectiveness.
      - Files: Projectile.cs: Controls projectile behavior and movement.
               Tower.cs: Handles tower firing logic and initializes projectiles with the correct damage.
    - Dynamic Tower UI: Designed UI buttons for different types of towers (weak, medium, strong, and ultimate). Buttons visually 
        represent the tower's strength and update dynamically when resources change.
      - Files: Tower Button Prefabs: Configured with images representing weak, medium, strong, and ultimate towers.

### Mohammed Mohammed :
    I was responsible for the procedural generation of the tower defense grid and the enemy AI. I developed the GridGenerator.cs script, 
    which creates a dynamic, grid-based map with a winding path from right to left. This path generation includes configurable properties 
    such as minimum and maximum lengths for straight segments, as well as the probability for the path to branch out and reconnect, creating a more complex layout.

    To ensure enemies efficiently navigate these branching paths, I implemented Dijkstra's shortest path algorithm, allowing enemies 
    to dynamically choose the shortest route to the end. I also handled core aspects of enemy management, including the queuing system for enemy 
    summoning using Summoner.cs and Enemy_spawn_data.cs. Additionally, I was responsible for visual elements like the enemy health bars, background
     music using MusicController.cs, the CamControl.cs script for camera movement, as well as the design of the surrounding terrain and environment.
    
    Scripts
    
    High contributions:
    1.⁠ ⁠Enemy.cs
    2.⁠ ⁠EnemyManager.cs
    
    Sole work: 
    3.⁠ ⁠Enemy_spawn_data.CS
    4.⁠ ⁠Summoner.cs
    5.⁠ ⁠CamControl.cs
    6.⁠ ⁠MusicController.cs
    7.⁠ ⁠GridGenerator.cs
    
    
    Assests:
    1.⁠ ⁠Bush or Grass Bend as Detail Mesh
    2.⁠ ⁠Environment Pack: Free Forest Sample
    3.⁠ ⁠FREE Casual Game SFX Pack
    4.⁠ ⁠Free Sound Effects Pack
    5.⁠ ⁠Free Stylized Nature Environment
    6.⁠ ⁠Grass Flowers Pack Free
    7.⁠ ⁠Mountain Terrain, Rocks and Tree
    8.⁠ ⁠Rock Package
### Giang Nguyen 
    I was mostly responsible for the UI, I created the HUD bar and a button that allows you to play and pause the game, as well as the start 
    menu with the instructions at the beginning of the game. I also worked on the animation aspect of the enemies by getting assets from the Unity Assets 
    Store and added animations for each enemy. I also implemented the wave management with enhanced difficulty for each passing wave. I was solely 
    responsible for the MenuControl.cs file, and contributed partly to the EnemyManager.cs and Enemy.cs files.
    Here is what I used: assets used: 
    Orc: https://assetstore.unity.com/packages/3d/characters/3d-stylized-orc-animations-pbr-13-6k-faces-246181
    Goblin: https://assetstore.unity.com/packages/3d/characters/humanoids/3d-stylized-goblin-animations-252473
    Button and some UI: https://assetstore.unity.com/packages/2d/gui/icons/steampunkui-238976
