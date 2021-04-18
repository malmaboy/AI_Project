# Artificial Intelligence - TrouxaBot
## Authors
- Hugo Carvalho Nº 21901624

- André Figueira Nº 21901435

- Hugo Carvalho
  - Programing Negamax
  - Programing Heuristic 

- André Figueira
  - How Heuristic should work
  - Delivery Report

### Architecture Solution

- In this AI were applied the Negamax algorithm.(Explicar como Negamax funciona)
  
- In the Setup method we define 2 MaxDepth variables. If the board is lower than 8 Rows and 7 Cols MaxDepth its 3, on the other board MaxDepth will be 2. With this way our AI is able to think faster and well with different boards.
### How Heuristic Works

- Our Heuristic puts pieces in the middle of the board. 
- We determine the distance between two points, than the center of the Rows.
- We added another variable that will give point if the piece is placed in the middle of the board.
- Than We verify if that position have a piece, if it has, we verify if our color is white or red. If we are White it means we are the first to play. Then we verify if the piece in the that board position has different shape of ours, if it has it gives less score. If it has the same shape we give more score. The same happens when we verify if our color is red.
- Then return the final heuristic score for the given board.