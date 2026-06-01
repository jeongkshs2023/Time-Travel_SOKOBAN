module Logic

open Types
open Reader
open System

type Command = 
  | Move of int * int
  | Undo
  | Pause
  | Reset
  | Else

//checking command
let parseInput key =
    match key with
    | ConsoleKey.UpArrow | ConsoleKey.W    -> Move (0, -1)
    | ConsoleKey.DownArrow | ConsoleKey.S  -> Move (0, 1)
    | ConsoleKey.LeftArrow | ConsoleKey.A  -> Move (-1, 0)
    | ConsoleKey.RightArrow | ConsoleKey.D -> Move (1, 0)
    | ConsoleKey.Z                         -> Undo
    | ConsoleKey.Escape                    -> Pause
    | ConsoleKey.R                         -> Reset
    | _                                    -> Else

let move (x, y) (dx, dy) = (x+dx, y+dy)

let cntPusher (boxPos: Position) (dx, dy) (mainChar: Position) (clones: Position list) = 
  let rec loop (cx, cy) cnt = 
    if (cx, cy) = mainChar || List.contains (cx,cy) clones then loop (cx - dx, cy - dy) (cnt+1)
    else cnt
  loop (fst boxPos - dx, snd boxPos - dy) 0

let update (level: LevelData) (state: GameState) (cmd: Command) = 
  match cmd with
  | Else | Pause | Reset -> state
  | Undo -> 
    match state.History with
    | preState :: restHistory -> 
      if preState.Clones.Length < level.CloneLimit then
        {preState with 
          Clones = state.MainChar :: preState.Clones
          History = restHistory 
          MoveCount = state.MoveCount + 1}                       //Clone_made
      else 
        { preState with 
            History = restHistory 
            MoveCount = state.MoveCount + 1
            Clones = state.Clones}     //maxClone

    | [] -> state                                       //no previous move
  | Move (dx, dy) ->
    let dir = (dx, dy)

    // box moving
    let oppDir = (-dx, -dy)
    let getBoxAt pos = state.Boxes |> List.tryFind (fun b -> b.Pos = pos)
    let rec getChain pos acc =
      match getBoxAt pos with
      | Some b -> getChain (move pos dir) (b :: acc)
      | None -> List.rev acc

    let tailBoxes =
      state.Boxes
      |> List.filter (fun b -> getBoxAt (move b.Pos oppDir) |> Option.isNone)
    let chains = tailBoxes |> List.map (fun b -> getChain b.Pos [])

    let mvingChns, stdChns = 
      chains 
      |> List.partition (fun chain -> 
        let totalW = chain |> List.sumBy (fun b -> b.RequiredPower)
        let firstBoxPos = (List.head chain).Pos
        let lastBoxPos = (List.last chain).Pos
        
        let pushers = cntPusher firstBoxPos dir state.MainChar state.Clones
        let destPos = move lastBoxPos dir
        let noWall = not (Set.contains destPos level.Walls)
        
        pushers >= totalW && noWall
      )

    let mvingBx = mvingChns |> List.concat
    let stdBx = stdChns |> List.concat

    let nxtBxes = 
      (mvingBx |> List.map (fun b -> {b with Pos = move b.Pos dir}))
      @ stdBx
    
    let isObtacle pos = Set.contains pos level.Walls || Set.contains pos (stdBx |> List.map (fun b -> b.Pos) |> Set.ofList)

    //Main charactor moving
    let expMainPos = move state.MainChar dir
    
    let nxtMainPos = 
      if isObtacle expMainPos then state.MainChar             //blocked with wall / not-pushable box
      else expMainPos                                                     //simple move

    // trap/bound judging
    let isOutOfBound (x, y) =
      x < 0 || y < 0 || x >= level.Size || y >= level.Size

    if List.contains nxtMainPos state.Traps || isOutOfBound nxtMainPos then
      { state with IsGameOver = true; MainChar = nxtMainPos }
    else
      let movedClones =
        state.Clones
        |> List.map (fun c -> move c dir)
        |> List.filter (fun c -> not (isObtacle c))
      let steppedTraps = 
                movedClones 
                |> List.filter (fun c -> List.contains c state.Traps) 
                |> Set.ofList
      let nxtTraps = state.Traps |> List.filter (fun t -> not (Set.contains t steppedTraps))


      let nxtClones = 
        movedClones 
        |> List.filter (fun c -> not (Set.contains c steppedTraps))
        |> List.filter (fun c -> c <> nxtMainPos)
        |> List.distinct
      
      if nxtMainPos = state.MainChar && nxtClones = state.Clones && nxtBxes.IsEmpty then state
      else
        { state with 
            MainChar = nxtMainPos
            Clones = nxtClones
            Boxes = nxtBxes
            Traps = nxtTraps
            MoveCount = state.MoveCount + 1
            History = state :: state.History }
