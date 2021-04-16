using System;
using System.Threading;
using System.Collections.Generic;
using ColorShapeLinks.Common;
using ColorShapeLinks.Common.AI;
using UnityEngine;

namespace TrouxaBot
{
    public class TrouxaBotThinker : AbstractThinker
    {
        // Current board state
        private Winner winner;
        // Current shape state
        private PShape shape;
        
        // Maximum Negamax search depth
        private int maxDepth;

        // Default maximum search depth
        private const int DEFAULT_MAXIMUM_DEPTH = 3;

        /// <summary>
        /// Setup this AI 
        /// </summary>
        /// <param name="str"></param>
        public override void Setup(string str)
        {
            if (!int.TryParse(str, out maxDepth))
                maxDepth = DEFAULT_MAXIMUM_DEPTH;

            if (maxDepth < 1)
                maxDepth = DEFAULT_MAXIMUM_DEPTH;

            //losingMoves = new List<FutureMove>();
        }


        /// Returns the name of this AI which will include the maximum search depth
        public override string ToString() => "G03_" + base.ToString() + "_V2";

        public override FutureMove Think(Board board, CancellationToken ct)
        {
            // Invoke Negamax, starting with zero depth
            (FutureMove move, float score) decision = Negamax(board, ct,
                board.Turn, board.Turn, 0, float.NegativeInfinity,
                float.PositiveInfinity);
            // Return best move
            return decision.move;
        }

        private (FutureMove move, float score) Negamax(Board board,
            CancellationToken ct, PColor player, PColor turn, int depth,
            float alpha, float beta)
        {
            //Move to return 
            (FutureMove move, float score) selectedMove;
            

            // If a cancellation request was made 
            if (ct.IsCancellationRequested)
                // set a "no move" and skip the remaining part of algorith
                selectedMove = (FutureMove.NoMove, float.NaN);

            // Otherwise
            if ((winner = board.CheckWinner()) != Winner.None)
            {
                if (winner.ToPColor() == player)
                    // AI player wins, return hightest possible score
                    selectedMove = (FutureMove.NoMove, float.PositiveInfinity);
                else if (winner.ToPColor() == player.Other())
                    // Opponent wins, return lowest possible score
                    selectedMove = (FutureMove.NoMove, float.NegativeInfinity);
                else
                    // A draw, return zero
                    selectedMove = (FutureMove.NoMove, 0f);

            }
            // Apply Heuristic HERE!!!!!!
            else if (depth == maxDepth)
                selectedMove = (FutureMove.NoMove, Heuristics(board, player, shape));
            else
            {
                selectedMove = (FutureMove.NoMove, float.NegativeInfinity);

                // Test each column
                for (int i = 0; i < Cols; i++)
                {
                    if (board.IsColumnFull(i)) continue;

                    for (int j = 0; j < 2; j++)
                    {
                        // Get current shape
                        PShape shape = (PShape) j;

                        // Use this variable to keep the current board's score
                        float eval;

                        // Skip unavailable shapes
                        if (board.PieceCount(turn, shape) == 0) continue;

                        // Test move, call Negamax and undo move
                        board.DoMove(shape, i);
                        eval = -Negamax(
                            board, ct, player.Other(), turn.Other(), depth + 1,
                            float.NegativeInfinity,
                            float.PositiveInfinity).score;
                        board.UndoMove();
                            
                            //Alpha beta cut

                        if (eval > selectedMove.score)
                            // If Keep it
                            selectedMove = (new FutureMove(i, shape), eval);

                        // move so far?
                        if (eval > alpha)
                            // If so, keep it
                            alpha = eval;

                        if (beta < alpha)
                            return selectedMove;

                        
                    }
                }
            }

            return selectedMove;
        }

        /// Apply an Heuristic for winning
        private static float Heuristics(Board board, PColor color , PShape shape)
        {
            
            /*
            // GetBoardScoreFor 
            if(GetBoardScoreFor(board, color) - GetBoardScoreFor(board,
                color.Other()) > AndresHeuristic(board, shape, color))
            {
                // return GetBoardScoreFor
                return GetBoardScoreFor(board, color) - GetBoardScoreFor(board,
                    color.Other());
            }
            
            if (AndresHeuristic(board, shape,color) > GetBoardScoreFor(board, color) - GetBoardScoreFor(board,
                    color.Other()))
            {
                return AndresHeuristic(board, shape, color);
            }*/

            return AndresHeuristic(board, shape, color);
        }

        private static float GetBoardScoreFor(Board board, PColor turn)
        {
            // Current heuristic value
            float score = 0;
            // To see if line is available
            bool lineAvailable = true;

            for (int i = 0; i < board.rows; i++)
            {

                for (int j = 0; j < board.cols; j++)
                {
                    // Get piece in current board position 
                    Piece? piece = board[i, j];

                    // Verify if there any piece
                    if (piece.HasValue)
                    {
                        lineAvailable = false;
                        break;
                    }
                }

                if (lineAvailable)
                    
                    score++;
            }

            // Return the final heuristic score
            return score;
        }

        /// <summary>
        /// Never play under opponent winning condition
        /// </summary>
        /// <param name="board"></param>
        /// <returns>score</returns>
        private static float NeverPlayUnderWinningMove(Board board)
        {
            // Heuristic score
            float score = 0;
            // 
            bool isWin    = false;
            
            for (int i = 0; i < board.rows; i++)
            {
                
                if (board.IsColumnFull(i)) continue;

                for (int j = 0; j < board.cols; j++)
                {
                    // Get current shape
                    PShape shape = (PShape) j;
                    
                    // Verify if the position is valid
                    if (j + 1 >= board.cols)
                    {
                        continue;
                    }
                    // Get Enemy board win position
                    // +1 is above 
                    Piece? enemyPiece = board[i, j + 1];
                   
                    // Verify if any piece in that position
                    if (enemyPiece.HasValue)
                    {
                        // It is a win for the opponent
                        isWin = true;
                        // Less score
                        score--;
                        break;
                    }
                    // Need to be more precise
                    if (isWin)
                    { 
                        score++;
                    }
                }
            }

            return score;
        }

        private static float AndresHeuristic(Board board, PShape turn, PColor color)
        {
            // Heuristic score
            float score = 0;
            
            // Distancee between two points
            float Distance(float x, float y, float x1, float y1)
            {
                return (float)Math.Sqrt(
                    Math.Pow(x - x1, 2) + Math.Pow(y - y1, 2));
            }
            
            // Determine the center row
            float centerRow = board.rows / 2;
            float centerCol = board.cols / 2;
            
            // Points added
            float points = Distance(centerRow, centerCol, 0, 0);

            for (int i = 0; i < board.rows; i++)
            {
                if (board.IsColumnFull(i)) continue;
                
                for (int j = 0; j < board.cols; j++)
                {
                    
                    // Play in the middle
                    // Get piece in current board position
                   Piece? enemyPiece = board[i, j];
                    
                    // Verify is there any opponent piece
                    if (enemyPiece.HasValue)
                    {
                        // If our color is white
                        if (color == PColor.White)
                        {
                            // if there is
                            // if the piece shape is != of ours 
                            if (enemyPiece.Value.shape != turn)
                            {
                                // New sequence 
                                score -= points -
                                         Distance(centerRow, centerCol, i, j);
                            }
                            // if there is
                            // if the piece shape == of our
                            if (enemyPiece.Value.shape == turn)
                            {
                                // if the p
                                score += points -
                                         Distance(centerRow, centerCol, i, j);
                            }
                        }
                        
                        // If our color is red 
                        if (color == PColor.Red)
                        {
                            if (enemyPiece.Value.shape != color.Shape())
                            {
                                
                                // New sequence 
                                score -= points -
                                         Distance(centerRow, centerCol, i, j);
                            }
                            // if there is
                            // if the piece shape == of our
                            if (enemyPiece.Value.shape == color.Shape())
                            {
                                // if the p
                                score += points -
                                         Distance(centerRow, centerCol, i, j);
                            }
                        }
                    }
                }
                    
            }

            return score;
        }
    }
}
