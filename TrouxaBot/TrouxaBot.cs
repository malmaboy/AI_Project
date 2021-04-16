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
                selectedMove = (FutureMove.NoMove, Heuristics(board, player));
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
        private  float Heuristics(Board board, PColor turn)
        {
            
            // Play in the middle > GetBoardScoreFor
            if (NeverPlayUnderWinningMove(board) >
                GetBoardScoreFor(board, turn) - GetBoardScoreFor(board,
                    turn.Other()))
            {
                // Return PlayInTheMiddle
                return NeverPlayUnderWinningMove(board);
            }
            // GetBoardScoreFor > PlayInTheMiddle
            if(GetBoardScoreFor(board, turn) - GetBoardScoreFor(board,
                turn.Other()) > NeverPlayUnderWinningMove(board))
            {
                // return GetBoardScoreFor
                return GetBoardScoreFor(board, turn) - GetBoardScoreFor(board,
                    turn.Other());
            }
            
            // GetBoardScoreFor > NeverPlayUnderWinningMove
            if(GetBoardScoreFor(board, turn) - GetBoardScoreFor(board,
                turn.Other()) > NeverPlayUnderWinningMove(board))
            {
                // Return GetBoardScoreFor
                return GetBoardScoreFor(board, turn) - GetBoardScoreFor(board,
                    turn.Other());
            }
            // NeverPlayUnderWinningMove > GetBoardScoreFor
            if (NeverPlayUnderWinningMove(board) >
                 GetBoardScoreFor(board, turn) - GetBoardScoreFor(board,
                     turn.Other()))
            {
                // Return NeverPlayUnderWinningMove
                return NeverPlayUnderWinningMove(board);
            }
            
            return 0;
        }

        private  float GetBoardScoreFor(Board board, PColor turn)
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
        private float NeverPlayUnderWinningMove(Board board)
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
        
    }
}
