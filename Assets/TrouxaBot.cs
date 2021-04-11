using System;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using ColorShapeLinks.Common;
using ColorShapeLinks.Common.AI;



public class TrouxaBot : AbstractThinker
{
    /*
    private List<FutureMove> possibleMoves;
    private List<FutureMove> nonLosingMoves;
    private System.Random random;

    public override void Setup(string str)
    {
        possibleMoves = new List<FutureMove>();
        nonLosingMoves = new List<FutureMove>();
        random = new System.Random();
    }
    public override FutureMove Think(Board board, CancellationToken ct)
    {
        Winner winner;
        PColor AiColor = board.Turn;
        
        possibleMoves.Clear();
        nonLosingMoves.Clear();

        for (int collums = 0; collums < Cols; collums++)
        {
            // Skip, if collum is full
            if (board.IsColumnFull(collums))
                continue;
            // Loop through the two shapes
            for (int isShape = 0; isShape < 2; isShape++)
            {
                PShape shape = (PShape) isShape;
                // If there's no pieces of this shape skip this move
                if (board.PieceCount(AiColor, shape) == 0) 
                    continue;
                // Add move to possible move list
                possibleMoves.Add(new FutureMove(collums, shape));
                
                // Test move
                board.DoMove(shape, collums);
                
                // Verify if has a winner
                winner = board.CheckWinner();
                
                // Undo the move
                board.UndoMove();
                
                // The AI is the winner?
                if (winner.ToPColor() == AiColor)
                    // Return this move immeditely
                    return new FutureMove(collums, shape);
                // If our opponent is the winner
                else if (winner.ToPColor() != AiColor.Other())
                    // Add this move to non-losing move list
                    nonLosingMoves.Add(new FutureMove(collums, shape));
                
            }
        }
        // Where to change!!!!!
        
        // Do we have any moves on the non-losing move list?
        if (nonLosingMoves.Count > 0)
            // If so, return a random one
            return nonLosingMoves[(random.Next(nonLosingMoves.Count))];
        
        // If we get here, just return any valid move
        return possibleMoves[random.Next(possibleMoves.Count)];
        
        
    }*/
    // Maximum Negamax search depth
    private int maxDepth;
    // Default maximum search depth
    private const int defaultMaxDepth = 5;
    
    /// <summary>
    /// Setup this AI 
    /// </summary>
    /// <param name="str"></param>
    public override void Setup(string str)
    {
        if (!int.TryParse(str, out maxDepth))
        {
            maxDepth = defaultMaxDepth;
        }

        if (maxDepth < 1)
            maxDepth = defaultMaxDepth;
    }

    
    /// Returns the name of this AI which will include the maximum search depth
    public override string ToString()
    {
        return base.ToString() + "D" + maxDepth;
    }

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
        
        // Current board state
        Winner winner;
        
        // If a cancellation request was made 
        if (ct.IsCancellationRequested)
            // set a "no move" and skip the remaining part of algorith
            selectedMove = (FutureMove.NoMove, float.NaN);
    
        // Otherwise
        else if ((winner = board.CheckWinner()) !=  Winner.None)
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
            selectedMove = (FutureMove.NoMove,GetBoardScore(board, player));
        else
        {
            selectedMove = turn == player
                ? (FutureMove.NoMove, float.NegativeInfinity)
                : (FutureMove.NoMove, float.PositiveInfinity);
            
            
            // Test each column
            for (int i = 0; i < Cols; i++)
            {
                if (board.IsColumnFull(i)) continue;
                
                for (int j = 0; j < 2; j++)
                {
                    // Get current shape
                    PShape shape = (PShape)j;

                    // Use this variable to keep the current board's score
                    float eval;

                    // Skip unavailable shapes
                    if (board.PieceCount(turn, shape) == 0) continue;

                    // Test move, call Negamax and undo move
                    board.DoMove(shape, i);
                    eval = -Negamax(
                        board, ct, player, turn.Other(), depth + 1, 
                        -float.NegativeInfinity, 
                        -float.PositiveInfinity).score;
                    board.UndoMove();
                    

                    // If we're maximizing, is this the best move so far?
                    if (eval > selectedMove.score)
                    {
                        // If so, keep it
                        selectedMove = (new FutureMove(i, shape), eval);
                        Debug.Log("Hello_1");
                    }
                    // Otherwise, if we're minimizing, is this the worst
                    // move so far?
                    if (eval > alpha)
                    {
                        // If so, keep it
                        alpha = eval;
                        Debug.Log("Hello_2");
                       
                    }
                    if (alpha >= beta)
                    {
                        return selectedMove;
                        Debug.Log("Hello_3");
                    }
                }
            }
        }
        return selectedMove;
    }

    /// Apply an Heuristic for winning
    public static float GetBoardScore(Board board, PColor turn)
    {
        return GetBoardScoreFor(board, turn) - GetBoardScore(board,
          turn.Other());
    }
    
    private static float GetBoardScoreFor(Board board, PColor turn)
    {
        // Current heuristic value
        float score = 0;
        bool lineAvailable = true;
        
        for (int i = 0; i < board.rows; i++)
        {
            
            for (int j = 0; j < board.cols; j++)
            {
                Piece? piece = board[i, j];

                if (piece.HasValue)
                {
                    lineAvailable = false;
                    break;
                }
            }
            if (lineAvailable) 
                score++;
        }

        return score;
    }
}
