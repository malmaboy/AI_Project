# Artificial Intelligence - TrouxaBot
## Authors
- Hugo Carvalho Nº 21901624

- André Figueira Nº 21901435

- Hugo Carvalho
  - Programing Negamax;
  - Programing Heuristics;
  - Applying Heuristics to work in every track;

- André Figueira
  - Heuristics Idea;
  - AI and Heuristics Working Theory;
  - Delivery Report;

This work was divided in the following way. Hugo Carvalho programmed the AI and applied every heuristic to it. André Figueira came up with the heuristics used, how the AI should think and act, and the delivery report. Both students were equally involved in the project, although Hugo Carvalho worked much more in the program itself, while André Figueira worked more in the heuristics.

### Architecture Solution

- In this AI we applied the Negamax algorithm.
- The Negamax algorithm is a version of the minimax algorithm. While the minimax relies in a zero-sum property to be applied, the minimax comes to simplify its application by mirroring both player scores, as in one player wants to have a higher score and every good move gives them points, the other player wants to have a lower score, losing points with every good move. The Negamax algorithm is much simpler to apply considering the minimax, because we can use only one function in which our AI wants to have a higher score, and we can find a good move by choosing the one with the highest score, knowing as well that that move will be the worst to our opponent.
  
- In the Setup method we defined 2 MaxDepth variables. If the board is lower than 8 Rows and 7 Cols MaxDepth its 3, otherwise the board MaxDepth will be 2. 
- By doing so, our AI is able to think faster and adapt well to different board sizes, while maintaining consistent results.

### Trouxabot Heuristics

- Our Heuristic starts by putting one piece in the middle of the board. (By doing this we maximize the amount of possible victories).
- We determine where is the middle of the board with the distance between two points, to determine the first move.
- We added another variable that will verify if the piece was played in the middle. If it was it will increase the heuristics score, otherwise it will decrease it.
- After doing so, we verify if that position has a piece. If it does, we verify it's color, and consecutively its turn. Next, we check the piece's shape and if it's the same shape we increase the score,(As its beneficial for us because it increases our chance of victory) and if it's different we decrease it. The same happens when we check the piece's color.
- Finally, we return the final heuristic score for the given board.

![AI_example](AI_example.png)


### [color-shape-links-ai-competition](https://github.com/VideojogosLusofona/color-shape-links-ai-competition#tracks-and-prize-money)
