open System
open System.Text
open MainScreen



[<EntryPoint>]
let main argv =
    Console.CursorVisible <- false

    let initialSaveData = loadSave ()

    lvSelectLoop initialSaveData

    0