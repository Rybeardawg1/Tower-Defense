# Tower Defense Game 
## Overview:
    This tower defense game integrates traditional mechanics with an educational twist, combining strategic gameplay 
    with engaging math challenges. Players must solve math problems of varying difficulty to earn points, which they
    use to build and upgrade towers. The game uses AI-based pathfinding for enemies, ensuring dynamic gameplay, and 
    features intuitive mechanics, polished visuals, and sound effects for an immersive experience.

### What makes this game unique?
    Unlike classic tower defense games, ours incorporates a learning component. Players actively solve 
    math problems to progress, 
    promoting education alongside entertainment.

## Key Features

### Educational Component - Math Challenges
    Players solve math problems during gameplay to earn points. Problems are categorized into:
    
    - Easy: Low rewards, suitable for quick answers.
    - Medium: Moderate difficulty with balanced rewards.
    - Hard: High difficulty with maximum rewards.
    The math-solving panel can be toggled on/off, ensuring it integrates seamlessly with the core gameplay.
    
    - Educational Value: This component promotes problem-solving and learning in an engaging way, transforming the 
    game into an educational tool.
### Dynamic Game Elements 
#### Towers
    Towers can be dragged and dropped onto valid zones. Placement resets if invalid.
    Four types of towers: Weak, Medium, Strong, Ultimate, with each offering increased damage output.
    Towers fire projectiles at enemies within range, with projectiles carrying damage values based on tower strength.
#### Enemies and Pathfinding
    Enemies traverse the grid using Dijkstra’s shortest path algorithm, dynamically adjusting to complex paths. 
    They spawn in waves, increasing in difficulty, keeping players challenged.
    - Enemy Health Bars: Visualize health for better clarity.
    - Enemy Types: Includes Orcs and Goblins with animations for movement and attacks.
#### Resources and UI
    - Balance Management: Points are updated dynamically based on math problems solved and towers placed.
    - Health Management: Health decreases when enemies reach the end.
    - Interactive UI: Real-time updates for health, balance, and tower availability.

### Mechanics 
    The mechanics are designed to be intuitive and engaging, with seamless integration of strategy and education:
    
    - Tower Placement: Drag-and-drop mechanics ensure smooth gameplay.
    - Firing System: Towers detect enemies in range and fire projectiles dynamically.
    - Resource Management: Players must balance solving math problems and placing/upgrading towers.
    These mechanics create an enjoyable challenge while maintaining educational value.
    Math problems, balance, and health are displayed in real time.
    Buttons reflect availability based on the player's balance.

### AI and Pathfinding 
    Enemies use Dijkstra’s algorithm for optimal pathfinding. The AI ensures:
    
    - Enemies dynamically calculate the shortest route.
    - Path complexity with branching paths makes the game challenging.
    - Innovation: The AI adapts in real-time to grid changes, enhancing gameplay.

### Physics and Animations 
    - Projectile Physics: Projectiles inherit damage values and move smoothly towards targets.
    - Enemy Animations: Enemies (Orcs, Goblins) use high-quality animations for movement and attacks.
    - Tower Effects: Visual cues highlight tower placement and firing.

### User Interface 
    - Main Menu: Includes instructions on gameplay and math challenges.
    - HUD: Displays health, balance, and math problem panel.
    - Pause/Play Buttons: Players can pause/resume the game at any time.

### Aesthetics and Sound Effects 
    - Visuals: A combination of sci-fi and natural environments using assets like forests, rocks, and backgrounds.
    - Sound Effects: Added for towers firing, enemies taking damage, and button clicks, creating an immersive experience.
    

### Performance 
    The game runs smoothly at a consistent frame rate of 60 FPS on tested devices, ensuring a lag-free experience.

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

### Ryan Barry
    As part of enhancing the game environment and improving gameplay clarity, I worked on the following updates:

    Space Background:
    I added a space-themed background to the game scene, creating a visually immersive and futuristic atmosphere.
    This background serves as a static visual layer, complementing the overall theme of the game and engaging the player.
    
    Tower Cells for Strategic Placement:
    I implemented tower cells to clearly define valid grid positions where towers can be placed.
    These cells are visually highlighted, helping players strategize their tower placements effectively based on the enemy path.
    Implementation Details
    Files worked on: GridGenerator.cs, EnemyManager.cs:



