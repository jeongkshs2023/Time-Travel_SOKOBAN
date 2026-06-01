module Viewer

open Types
open System
open Setting

let applyColor color =
    if currentSettings.UseColors then
        Console.ForegroundColor <- color
    else
        Console.ResetColor()

let cellPrint pos (level: LevelData) (state: GameState) = 

  if pos = state.MainChar then 
    applyColor ConsoleColor.Green
    printf "@@"
  elif List.contains pos state.Clones then
    applyColor ConsoleColor.DarkGreen
    printf "&&"
  else
    let isTrap = List.contains pos state.Traps
    let boxOpt = List.tryFind (fun box -> box.Pos = pos) state.Boxes

    match boxOpt, isTrap with
    | Some box, true -> 
      applyColor ConsoleColor.Red
      printf "\u2610%d" box.RequiredPower
    | Some box, false -> 
      applyColor ConsoleColor.Yellow
      printf "\u2610%d" box.RequiredPower
    | None, true -> 
      applyColor ConsoleColor.Red
      printf "XX"
    | None, false ->
      if pos = level.Goal then
        applyColor ConsoleColor.Cyan
        printf ">>"
      elif Set.contains pos level.Walls then
        applyColor ConsoleColor.Gray
        printf "##"
      else
        printf "  "

let render (level: LevelData) (state: GameState) = 
  Console.SetCursorPosition(0, 0)
  applyColor ConsoleColor.White
  printfn "======================================="
  printfn " %d. %s " level.LevelNo level.Title
  printfn "======================================="
  applyColor ConsoleColor.Cyan
  printfn " Move: %d / %d | Clones: %d / %d" 
    state.MoveCount level.TargetMoves 
    state.Clones.Length level.CloneLimit
  printfn "---------------------------------------"
  let size = level.Size
  for y in 0..size do
    for x in 0..size do
      cellPrint (x,y) level state
    printfn ""
  Console.ResetColor()
  printfn "==================================="
  printfn "[ W/A/S/D or Arrows ]: Move | [ Z ]: Undo | [ ESC ]: Pause | [ R ]: Fast Reset"

//     그래서 box 표현은 어디로 가는가 >>>>> digit으로 바꾸기
//     그 다음 viewer 완성하고
//     ruinning 만들고
//     gameloop + 메인화면 만들고
//     세부 규칙 정비하고
//     레벨 디자인 종류별로 하고
//     설정하고
//     자잘한 거(효과음, 뭐있지 아무튼 뭐시기) 설정하기