module Reader

open System.IO
open Types

let getMap (lv: string) = 
  let lines = File.ReadAllLines($"./maps/level{lv}.txt")
  
  //get clone limit / size / Ex-target
  let header = lines.[0].Split(' ')
  let limit = int header.[0]
  let size = int header.[1]
  let targetMoves = int header.[2]

  //map title
  let title = lines.[1]

  let mapLines = lines.[2..]

  //get multiple positions of exact char(thing)
  let finds thing = 
    mapLines
    |> Array.mapi (fun y line -> 
      line.ToCharArray()
        |> Array.mapi (fun x char ->
        if char = thing then Some (x, y) else None
      )
    )
    |> Seq.concat
    |> Seq.choose id
    |> Seq.toList

  //get a position of exact char(thing)
  let find (thing: char) =
    mapLines
    |> Array.mapi (fun y line -> 
      let x = line.IndexOf(thing)
      if x >= 0 then Some(x, y) else None
    )
    |> Array.tryPick id

  let mainCharPos = 
    match find '@' with                  //exception but don't occur except my mistake
    | Some pos -> pos
    | None -> Position(-1, -1) 
  
  let goalPos = 
    match find '>' with
      | Some pos -> pos
      | None -> Position(-1, -1)

  let walls = finds '#' |> Set.ofList

  let traps = finds 'X'

  let boxes = 
    mapLines
    |> Array.mapi (fun y line -> 
      line.ToCharArray()
      |> Array.mapi (fun x char ->
        if System.Char.IsDigit(char) then
          let power = int (char.ToString()) // '3' -> 3
          Some { Pos = (x, y); RequiredPower = power }
        else None
      )
    )
    |> Seq.concat
    |> Seq.choose id
    |> Seq.toList

  let levelData = {
    LevelNo = int lv
    Title = title
    Walls = walls
    Goal = goalPos
    CloneLimit = limit
    Size = size
    TargetMoves = targetMoves
  }

  let initialGameState = {
    MainChar = mainCharPos
    Clones = []
    Boxes = boxes
    Traps = traps
    CloneLimit = limit
    MoveCount = 0
    History = []
    IsGameOver = false
  }
  

  (levelData, initialGameState)