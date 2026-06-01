# Time-Travel_SOKOBAN

A command-line box-pushing game built with **F# / .NET 10**.

You play by moving onto goal position of grid map with some commands.

---

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)  
  Verify with: `dotnet --version` (should show `10.x.x`)

### Run

```bash
# Windows
run.bat

# Unix / macOS
chmod +x run.sh
./run.sh

# Or directly
dotnet run
```

### Build

```bash
dotnet build
```

### Publish Self-Contained Binary

```bash
# Windows x64
dotnet publish -c Release -r win-x64 --self-contained

# Linux x64
dotnet publish -c Release -r linux-x64 --self-contained
```

---

## How to Play

### Map Selection

- When you launch the game, you will see a grid menu of available levels.
- You can select a level by typing its corresponding number.
- You can only choose a board if all previous boards have been successfully cleared. Locked levels will reject your access.
- Typing 'DELETE' to clear save file

### Map Layout

During gameplay, the terminal is divided into three main visual sections:

- Header: Displays the current Level Number and the Title of the map.
- Status Board: Shows your current MoveCount against the TargetMoves, and the active Clones against the CloneLimit.
- Play Area: A square/rectangular grid showing the current state of the map using ASCII characters.

### Objects

- @ (Main Character): The player-controlled entity.
- & (Clone): A time-remnant of the Main Character. Clones mimic your directional movements exactly.
- #_(Wall): Unpassable obstacles. Neither characters nor boxes can move through them.
- X (Trap): Hazardous tiles. If a Clone steps on a trap, both the Clone and the Trap are destroyed. 
            If the Main Character steps on it, it triggers a Game Over.
- 1, 2, 3... (Boxes): Heavy blocks that require multiple characters to push. 
                      The number indicates the required "Pushing Power" (the number of characters that must be lined up in a row to move the box).
- |> (Goal): The destination. The Main Character must reach this tile to clear the level.

### Commands

- Arrow Keys / W, A, S, D: Move the Main Character (and all active Clones simultaneously) Up, Down, Left, or Right.
- Z (Undo & Clone): Rewinds time. The Main Character returns to their previous position, leaving a Clone at the location where Z was pressed. 
- R (Reset): Restarts the current level from the beginning.
- ESC (Pause): Pauses the game and opens the Pause Menu.

### Setting

You can access the Setting menu by pressing ESC during gameplay and selecting 3. Setting from the Pause Menu.

### Winning & Ending

| Result | Condition |
|--------|-----------|
| **Level clear** | The Main Character (`@`) steps onto the Goal (`>`).  |
| **Ex-Cleared** | Level Clear when  `MoveCount` is smaller than `TargetMoves`.|
| **Game Over** | The Main Character (`@`) steps on a Trap (`X`) or falls out of bounds. |
| **Quit** | Goes to Setting menu or level selection on Pause menu. |

---

## Example Session

```
The game prints a board with a main character (@), a trap (X), a numbered box (2), and a goal (>).
The user presses arrow keys to move the character forward.
The user presses 'Z'. The game moves the character to its previous position and generates a clone (&).
The user presses an arrow key. The main character and the clone move simultaneously.
The clone steps on the trap. The game removes both the clone and the trap from the board.
The user creates another clone using 'Z', aligns with it, and they push the numbered box together.
The main character successfully reaches the goal.
The game prints the player's rank based on the moves made and returns to the updated Map Selection screen.
```

---

## Project Structure

```
Time-Travel_SOKOBAN_/
├── Time-Travel_SOKOBAN.fsproj    # .NET 10 F# project file
├── run.bat
├── run.sh
├── Program.fs                  # Entry point, file save
├── Mapdef.fs                   # Defining Types for needs
├── Reader.fs                   # Read map file and pass map data
├── Setting.fs                  # Setting values by immutable variables
├── running.fs                  # Program running by loop
├── Viewer.fs                   # Printing maps
├── Mainscreen.fs               # Printing menu
├── README.md
├── requirements.md
└── maps/
    └── level1.txt      # Map data as txt file, for 1-10.
```


### Key Types

```fsharp
// Datas for the current map
type LevelData = {
    LevelNo: int
    Title: string
    Walls: Set<Position>             // ##
    Goal: Position                   //  >
    CloneLimit: int
    Size: int
    TargetMoves: int
}

// Datas for current game, for each turns
type GameState = {
    MainChar: Position               // @@
    Clones: Position list            // &&
    Boxes: Box list                  // digit
    Traps: Position list             // XX
    CloneLimit: int                     
    MoveCount: int                      
    History: GameState list          // records for undo
    IsGameOver: bool
}
```

### Module Overview

| Module | Responsibility |
|--------|---------------|
| `Types` | Defining types: LevelData, GameState |
| `Reader`  | Read map file and pass map data |
| `Setting` | Setting values by immutable variables |
| `Logic` | Main program moving while playing game |
| `Viewer` | Printing maps while playing game |
| `Mainscreen` | Print level selection, pause menu |
| `Program` | Entry point; drives play loop and initiate saving |

### Rules Summary

- You can choose map number at first
- You can pause and do some operations in that menu.
- You make move of @ with arrow key/wasd
- You can make clones by press z and another things go back previous turn.
- If @ meets X, game over.
- If @ meets >, you win.
- Digits means box, its number means the charactors need to push it.

## Use of LLM
I used LLM to do under tasks:
The order of developing functions of game accoreding to requirements, the LLM recommended following procedure:
map reading -> printing -> moving logics -> menu -> specific rules -> additional, not necessary functions -> maps

Some repeating for each object condition: printing each object by match with

Checking what I missed in requirements

Making maps with the rule, which is hard enough -> the LLM couldn't generate useful something, so I had to make them myself.

How to play sound -> I gave up to play sound because it was platform-dependent.