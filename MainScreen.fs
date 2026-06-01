module MainScreen

open System
open System.Threading
open System.IO
open Reader
open Viewer
open Logic
open Setting

// Saving
let saveFilePath = "save.txt"
let maxLevel = 10              //set current max level

let initSaveData () =
  let data = Array.create maxLevel 0
  data.[0] <- 1 
  data

let loadSave () = 
  try
    if File.Exists(saveFilePath) then
      let txt = File.ReadAllText(saveFilePath)
      let parts = txt.Split(',')
      let data = initSaveData ()
      for i in 0 .. min (parts.Length - 1) (maxLevel - 1) do
        match Int32.TryParse(parts.[i]) with
        | true, v -> data.[i] <- v
        | _ -> ()
      data
    else 
      let data = initSaveData ()
      File.WriteAllText(saveFilePath, String.Join(",", data))
      data
  with | _ -> initSaveData ()

let saving (data: int array) = 
  try File.WriteAllText(saveFilePath, String.Join(",", data))
  with | _ -> ()


// Pause Menu actions
type PauseAction =
    | Resume
    | GoToLevelSelect
    | ResetLevel
    | GoToSettings

type GameResult = 
    | Quit
    | Cleared of int * int

// Pause Menu
let rec pauseMenuLoop () =
    Console.Clear()
    Console.ForegroundColor <- ConsoleColor.Yellow
    printfn "==================================="
    printfn "           [ PAUSED ]              "
    printfn "==================================="
    Console.ResetColor()
    printfn " 1. Return to level selection"
    printfn " 2. Reset current level"
    printfn " 3. Setting"
    printfn " ESC. Resume"
    printfn "==================================="  //Menu printing
    
    match Console.ReadKey(true).Key with                        //selection
    | ConsoleKey.D1 | ConsoleKey.NumPad1 -> GoToLevelSelect
    | ConsoleKey.D2 | ConsoleKey.NumPad2 -> ResetLevel
    | ConsoleKey.D3 | ConsoleKey.NumPad3 -> GoToSettings
    | ConsoleKey.Escape -> Resume
    | _ -> pauseMenuLoop ()


let rec gameLoop level state initialState =
  render level state
  let key = Console.ReadKey(true).Key
  let cmd = parseInput key
  
  match cmd with
  | Pause ->
    match pauseMenuLoop () with
    | Resume -> 
      Console.Clear()
      gameLoop level state initialState

    | ResetLevel -> 
      Console.Clear()
      gameLoop level initialState initialState

    | GoToSettings ->
      settingMenuLoop ()
      Console.Clear()
      Quit

    | GoToLevelSelect -> 
      Console.Clear()
      Quit

  | Reset -> 
    //confirming on
    if currentSettings.ConfirmReset then
      Console.Clear()
      Console.ForegroundColor <- ConsoleColor.Yellow
      printfn "\n==================================="
      printfn "  Really Reset Immediately? (Enter/Esc) "
      printfn "==================================="
      Console.ResetColor()

      //confirming reset
      let rec confirmLoop () = 
        match Console.ReadKey(true).Key with
        | ConsoleKey.Enter -> 
          Console.Clear()
          gameLoop level initialState initialState 
        | ConsoleKey.Escape -> 
          Console.Clear()
          gameLoop level state initialState
        | _ -> confirmLoop ()
      confirmLoop ()

    //confirming off
    else
      Console.Clear()
      gameLoop level initialState initialState

  | _ ->
    let newState = update level state cmd
    //Game Over
    if newState.IsGameOver then
      render level newState
      Console.ForegroundColor <- ConsoleColor.Red
      printfn "\n==================================="
      printfn "       You Died: Level Failed      "
      printfn "==================================="
      Console.ResetColor()
      printfn "Game Over"
      printfn "==================================="
      printfn "[Enter] Continue"
      printfn "[ESC] Level Selection"
      
      let rec gameOverLoop () = 
        match Console.ReadKey(true).Key with
        | ConsoleKey.Enter ->
          Console.Clear()
          gameLoop level initialState initialState
        | ConsoleKey.Escape -> 
          Console.Clear()
          Quit
        | _ -> gameOverLoop()
      gameOverLoop()

    // Level Clear
    elif newState.MainChar = level.Goal then
      render level newState

      let moves = newState.MoveCount
      let target = level.TargetMoves

      if moves <= target then Console.ForegroundColor <- ConsoleColor.Yellow
      else Console.ForegroundColor <- ConsoleColor.DarkBlue
      printfn "\n==================================="
      printfn "           Level Clear!            "
      printfn "==================================="
      Console.ResetColor()

      if moves <= target then Console.ForegroundColor <- ConsoleColor.Yellow
      else Console.ForegroundColor <- ConsoleColor.Red
      printfn "Ex-Turns: %d / %d" moves target
      Console.ResetColor()
      
      printfn "\n   Press A key to continue"
      Console.ReadKey(true) |> ignore
      Console.Clear()
      Cleared(moves, target)
    else
      gameLoop level newState initialState

// Level Selection (Main Menu)
let rec lvSelectLoop (saveData: int array) = 
  Console.Clear()
  printfn "=========================================="
  printfn "        Time-Travel SOKOBAN(TTS)          "
  printfn "=========================================="
  Console.ResetColor()
  printfn " [ Level List ]"
  Console.ForegroundColor <- ConsoleColor.DarkGray
  printf  " \u25A0 Locked "
  Console.ForegroundColor <- ConsoleColor.White
  printf  "| \u25A0 Unlocked "
  Console.ForegroundColor <- ConsoleColor.DarkBlue
  printf  "| \u25A0 Clear "
  Console.ForegroundColor <- ConsoleColor.Yellow
  printfn "| \u25A0 Ex-Clear"
  Console.ResetColor()
  printfn "------------------------------------------"
  for i in 1 .. maxLevel do
    let status = saveData.[i - 1]
    match status with
    | 0 -> Console.ForegroundColor <- ConsoleColor.DarkGray // Locked
    | 1 -> Console.ForegroundColor <- ConsoleColor.White    // Not Cleared
    | 2 -> Console.ForegroundColor <- ConsoleColor.DarkBlue    // Cleared
    | 3 -> Console.ForegroundColor <- ConsoleColor.Yellow   // Ex-Cleared
    | _ -> Console.ResetColor()
    printf "%2d " i
    if i % 5 = 0 then printfn "" // 10 levels in one line
  Console.ResetColor()
  if maxLevel % 5 <> 0 then printfn ""
  printfn "=========================================="
  printfn "Enter Level Number. (Q to Turn off)"
  printf "> "
  let input = Console.ReadLine()

  if input.ToUpper() = "Q" then ()
  elif input.ToUpper() = "DELETE" then
    let newData = initSaveData ()
    saving newData
    lvSelectLoop newData
  else
    match Int32.TryParse(input) with
    | true, lv when lv > 0 && lv <= maxLevel -> 
      if saveData.[lv-1] = 0 then                   //when unlocked
        Console.ForegroundColor <- ConsoleColor.Red
        printfn "\n[403] It's unloked level."
        Console.ResetColor()
        printfn "Please wait a second..."
        Thread.Sleep(1500)
        lvSelectLoop saveData
      else
        try
          let (levelData, initialState) = getMap (lv.ToString())
          let result = 
            Console.Clear()
            gameLoop levelData initialState initialState

          match result with
          | Cleared (moves, target) ->
            let isEx = if moves <= target then 3 else 2
            saveData.[lv-1] <- max saveData.[lv - 1] isEx
            if lv < maxLevel then
              saveData.[lv] <- max saveData.[lv] 1

            saving saveData
            lvSelectLoop saveData

          | Quit -> lvSelectLoop saveData
        with
        | ex -> 
          Console.ForegroundColor <- ConsoleColor.Red
          printfn "\n[404] Map Loading Failed: %s" ex.Message
          Console.ResetColor()
          printfn "Please wait a second..."
          Thread.Sleep(1500)
          lvSelectLoop saveData
    | _ -> 
      Console.ForegroundColor <- ConsoleColor.Red
      printfn "\n[400] only enter number 1 to %d." maxLevel
      Console.ResetColor()
      printfn "Please wait a second..."
      Thread.Sleep(1500)
      lvSelectLoop saveData