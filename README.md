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
