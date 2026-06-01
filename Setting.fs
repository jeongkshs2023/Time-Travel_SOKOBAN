module Setting

open System

type GameSettings = {
    UseColors: bool
    ConfirmReset: bool
}

let mutable currentSettings = { 
  UseColors = true;
  ConfirmReset = true}

let rec settingMenuLoop () = 
  Console.Clear()
  if currentSettings.UseColors then Console.ForegroundColor <- ConsoleColor.Cyan
  printfn "==================================="
  printfn "         [ SETTINGS MENU ]         "
  printfn "==================================="
  Console.ResetColor()

  let colorStatus = if currentSettings.UseColors then "ON" else "OFF"
  let resetStatus = if currentSettings.ConfirmReset then "ON" else "OFF"

  printfn " 1. Using Various Colors : %s" colorStatus
  printfn " 2. Confirming Reset : %s" resetStatus
  printfn "==================================="
  printfn " [Num] Change | [ESC] Return"

  match Console.ReadKey(true).Key with
  | ConsoleKey.D1 | ConsoleKey.NumPad1 ->
    currentSettings <- {currentSettings with UseColors = not currentSettings.UseColors}
    settingMenuLoop ()

  | ConsoleKey.D2 | ConsoleKey.NumPad2 ->
    currentSettings <- { currentSettings with ConfirmReset = not currentSettings.ConfirmReset }
    settingMenuLoop()

  | ConsoleKey.Escape -> ()

  | _ -> settingMenuLoop ()