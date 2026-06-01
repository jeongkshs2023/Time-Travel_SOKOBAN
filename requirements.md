**Project Title**: Time-Travel_SOKOBAN

**Overview**: This project is a Sokoban game, also known as Box-Pushing game, whose goal is moving onto specific position of grid map with some patterns. 

**Requirements**:

1. The user will see numbers of board selection and they can choose only the board that all previous boards are cleared.
2. When user select map number, correct board displays.
3. The board has wall, main character, traps, boxes with written number, goal as component.
4. The user will make a move of main character and clones (i.e. characters) by pressing the arrow keys.
5. When user press ¡®z¡¯ key, undo action occurs and a clone is generated at the location where the main character was just before pressing z.
6. When user trying undo while characters exist same number as limit, clone don't generate.
7. When main character trying to move to wall, it don¡¯t move.
8. When clone trying to move to wall, it disappears.
9. When clone or main character is on trap, both trap and the object disappear.
10. When main character disappears, game over and level restarts.
11. If and only if a number of characters equal to or greater than the number written on the box push it, the box will move.
12. When main characters are on goal, player wins and ranked depend on the moves they made

**Example Interaction**: 
 The game prints a board with a main character, a trap, a numbered box, and a goal. 
 The user presses arrow keys to move the character. 
 The user presses 'z'. The game moves the character to its previous position and generates a clone. 
 The user presses an arrow key. The character and the clone move simultaneously. 
 The clone steps on the trap. The game removes both the clone and the trap. 
 The user creates another clone, and they push the numbered box together. 
 The main character reaches the goal. The game prints the player's rank.