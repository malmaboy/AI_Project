using System;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using ColorShapeLinks.Common;
using ColorShapeLinks.Common.AI;



public class TrouxaBot : AbstractThinker
{
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
        
        
    }
}
