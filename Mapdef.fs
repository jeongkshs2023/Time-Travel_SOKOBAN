module Types

type Position = int * int

type Box = {
    Pos: Position
    RequiredPower: int        //limit num for pushing 
}

type LevelData = {
    LevelNo: int
    Title: string
    Walls: Set<Position>             // ##
    Goal: Position                   //  >
    CloneLimit: int
    Size: int
    TargetMoves: int
}

type GameState = {
    MainChar: Position               // @@
    Clones: Position list            // &&
    Boxes: Box list                  // ¡àdigit
    Traps: Position list             // XX
    CloneLimit: int                     
    MoveCount: int                      
    History: GameState list          // records for undo
    IsGameOver: bool
}