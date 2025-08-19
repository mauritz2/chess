using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Pawn : Piece
{
    void Start()
    {
        Type = PieceType.Pawn;
    }

    public override List<Vector2Int> GetPossibleMoves(Vector2Int pos, Piece[,] boardState)
    {
        List<Vector2Int> moves = new();

        // Check movement
        Vector2Int posOneStep = new (pos.x, pos.y + Step);
        Vector2Int posTwoStep = new (posOneStep.x, posOneStep.y + Step);
        bool isStepValid = !IsOutOfBounds(posOneStep) && boardState[posOneStep.x, posOneStep.y] == null;
        bool isDoubleStepValid = !IsOutOfBounds(posTwoStep) &&
                                    !hasMoved &&
                                    isStepValid && 
                                    boardState[posTwoStep.x, posTwoStep.y] == null;
        if (isStepValid) moves.Add(posOneStep);
        if (isDoubleStepValid) moves.Add(posTwoStep);

        // Check captures
        List<int> captureX = new() { -1, 1 };
        foreach (int x in captureX)
        {
            Vector2Int diagPos = new(pos.x + x, pos.y + Step);
            if (IsOutOfBounds(diagPos)) continue;
            Piece target = boardState[diagPos.x, diagPos.y];
            if (target != null && target.Player != this.Player) moves.Add(diagPos);
        }

        return moves;
    }
}
