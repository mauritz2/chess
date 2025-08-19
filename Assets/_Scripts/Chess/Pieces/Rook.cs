using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : Piece
{
    void Start()
    {
        Type = PieceType.Rook;
    }

    public override List<Vector2Int> GetPossibleMoves(Vector2Int pos, Piece[,] boardState)
    {
        return GetHorizontalAndVerticalMoves(pos, boardState);
    }
}
